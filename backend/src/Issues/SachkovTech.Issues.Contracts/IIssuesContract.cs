using CSharpFunctionalExtensions;
using SachkovTech.Core.Dtos;
using SachkovTech.Issues.Contracts.Responses;
using SachkovTech.SharedKernel;

namespace SachkovTech.Issues.Contracts;

public interface IIssuesContract
{
    Task<Result<IssueResponse, ErrorList>> GetIssueById(
        Guid issueId, CancellationToken cancellationToken = default);

    Task<Result<IssueDto, ErrorList>> GetIssueByPosition(
        int position, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<IssueResponse>, ErrorList>> GetIssuesByModuleId(
        Guid moduleId, CancellationToken cancellationToken = default);

}