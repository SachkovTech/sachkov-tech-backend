using SachkovTech.Core.Dtos;
using SachkovTech.Core.Models;
using SachkovTech.Issues.Contracts;
using SachkovTech.IssueSolving.Application.Commands.GetByModule;
using SachkovTech.IssueSolving.Domain.Enums;

namespace SachkovTech.IssueSolving.Application.Queries.GetByModule;

public class GetUserIssuesByModuleWithPaginationHandler
{
    private readonly IReadDbContext _readDbContext;
    private readonly IIssuesContract _issuesContract;

    public GetUserIssuesByModuleWithPaginationHandler(
        IReadDbContext readDbContext,
        IIssuesContract issuesContract)
    {
        _readDbContext = readDbContext;
        _issuesContract = issuesContract;
    }

    public async Task<PagedList<UserIssueDto> Handle(
        GetUserIssuesByModuleWithPaginationQuery query,
        CancellationToken cancellationToken)
    {
        var userIssuesQuery = _readDbContext.UserIssues
            .Where(u => u.ModuleId == query.ModuleId)
            .OrderBy(u => Enum.Parse<IssueStatus>(u.Status));


    }
}
