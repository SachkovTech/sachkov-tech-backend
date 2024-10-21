using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Contracts;
using SachkovTech.IssueSolving.Domain.Entities;
using SachkovTech.IssueSolving.Domain.Enums;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects.Ids;

namespace SachkovTech.IssueSolving.Application.Commands.TakeOnWork;

public class TakeOnWorkHandler : ICommandHandler<Guid, TakeOnWorkCommand>
{
    private readonly IUserIssueRepository _userIssueRepository;
    private readonly IReadDbContext _readDbContext;
    private readonly IIssuesContract _issuesContract;
    private readonly ILogger<TakeOnWorkHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TakeOnWorkHandler(
        IUserIssueRepository userIssueRepository,
        IReadDbContext readDbContext,
        IIssuesContract issuesContract,
        ILogger<TakeOnWorkHandler> logger,
        [FromKeyedServices(Modules.IssueSolving)] IUnitOfWork unitOfWork)
    {
        _userIssueRepository = userIssueRepository;
        _readDbContext = readDbContext;
        _issuesContract = issuesContract;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        TakeOnWorkCommand command,
        CancellationToken cancellationToken = default)
    {
        var issueResult = await _issuesContract.GetIssueById(command.IssueId, cancellationToken);

        if (issueResult.IsFailure)
            return issueResult.Error;

        var userIssueExisting =
            await _readDbContext.UserIssues.FirstOrDefaultAsync(ui => ui.IssueId == command.IssueId, cancellationToken);

        if (userIssueExisting is not null)
        {
            return Errors.General.ValueIsInvalid().ToErrorList();
        }

        if (issueResult.Value.Position > 1)
        {
            var previousIssueResult = await _issuesContract
                .GetIssueByPosition(issueResult.Value.Position - 1, cancellationToken);

            if (previousIssueResult.IsFailure)
                return previousIssueResult.Error;

            var previousUserIssue = await _readDbContext.UserIssues
                .FirstOrDefaultAsync(u => u.UserId == command.UserId &&
                                          u.IssueId == previousIssueResult.Value.Id, cancellationToken);

            if (previousUserIssue is null)
                return Errors.General.NotFound(null, "Previous solved issue").ToErrorList();

            var previousUserIssueStatus = Enum.Parse<IssueStatus>(previousUserIssue.Status);

            if (previousUserIssueStatus != IssueStatus.Completed)
                return Error.Failure("prev.issue.not.solved", "previous issue not solved").ToErrorList();
        }

        var userIssueId = UserIssueId.NewIssueId();
        var userId = UserId.Create(command.UserId);

        var userIssue = new UserIssue(userIssueId, userId, command.IssueId, command.ModuleId);

        var result = await _userIssueRepository.Add(userIssue, cancellationToken);

        await _unitOfWork.SaveChanges(cancellationToken);

        _logger.LogInformation("User took issue on work. A record was created with id {userIssueId}",
            userIssueId);

        return result;
    }
}