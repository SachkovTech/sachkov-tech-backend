using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SachkovTech.Core.Abstractions;
using SachkovTech.Issues.Contracts;
using SachkovTech.IssueSolving.Domain.Entities;
using SachkovTech.IssueSolving.Domain.ValueObjects;
using SachkovTech.SharedKernel;
using SachkovTech.SharedKernel.ValueObjects.Ids;

namespace SachkovTech.IssueSolving.Application.Commands.TakeOnWork;

public class TakeOnWorkHandler : ICommandHandler<Guid, TakeOnWorkCommand>
{
    private readonly IUserIssueRepository _userIssueRepository;
    private readonly IIssueSolvingReadDbContext _issueSolvingReadDbContext;
    private readonly IIssuesContract _issuesContract;
    private readonly ILogger<TakeOnWorkHandler> _logger;

    public TakeOnWorkHandler(
        IUserIssueRepository userIssueRepository,
        IIssueSolvingReadDbContext issueSolvingReadDbContext,
        IIssuesContract issuesContract,
        ILogger<TakeOnWorkHandler> logger)
    {
        _userIssueRepository = userIssueRepository;
        _issueSolvingReadDbContext = issueSolvingReadDbContext;
        _issuesContract = issuesContract;
        _logger = logger;
    }

    public async Task<Result<Guid, ErrorList>> Handle(
        TakeOnWorkCommand command,
        CancellationToken cancellationToken = default)
    {
        var issueResult = await _issuesContract.GetIssueById(command.IssueId, cancellationToken);

        if (issueResult.IsFailure)
            return issueResult.Error;

        if (issueResult.Value.Position > 1)
        {
            var previousIssueResult = await _issuesContract
                .GetIssueByPosition(issueResult.Value.Position - 1, cancellationToken);

            if (previousIssueResult.IsFailure)
                return previousIssueResult.Error;
        
            var previousUserIssue = await _issueSolvingReadDbContext.UserIssues
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

        var userIssue = new UserIssue(userIssueId, userId, command.IssueId);

        var result = await _userIssueRepository.Add(userIssue, cancellationToken);
        
        _logger.LogInformation("User took issue on work. A record was created with id {userIssueId}",
            userIssueId);

        return result;
    }
}