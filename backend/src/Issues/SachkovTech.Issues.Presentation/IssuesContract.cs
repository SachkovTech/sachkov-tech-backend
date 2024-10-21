using CSharpFunctionalExtensions;
using SachkovTech.Core.Dtos;
using SachkovTech.Issues.Application.Queries.GetIssueById;
using SachkovTech.Issues.Application.Queries.GetIssueByPosition;
using SachkovTech.Issues.Application.Queries.GetIssuesByModuleId;
using SachkovTech.Issues.Contracts;
using SachkovTech.Issues.Contracts.Responses;
using SachkovTech.SharedKernel;

namespace SachkovTech.Issues.Presentation;

public class IssuesContract : IIssuesContract
{
    private readonly GetIssueByIdHandler _getIssueByIdHandler;
    private readonly GetIssueByPositionHandler _getIssueByPositionHandler;
    private readonly GetIssuesByModuleIdHandler _getIssuesByModuleIdHandler;

    public IssuesContract(
        GetIssueByIdHandler getIssueByIdHandler,
        GetIssueByPositionHandler getIssueByPositionHandler,
        GetIssuesByModuleIdHandler getIssuesByModuleIdHandler)
    {
        _getIssueByIdHandler = getIssueByIdHandler;
        _getIssueByPositionHandler = getIssueByPositionHandler;
        _getIssuesByModuleIdHandler = getIssuesByModuleIdHandler;
    }

    public async Task<Result<IssueResponse, ErrorList>> GetIssueById(
        Guid issueId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetIssueByIdQuery(issueId);

        return await _getIssueByIdHandler.Handle(query, cancellationToken);
    }

    public async Task<Result<IssueDto, ErrorList>> GetIssueByPosition(
        int position,
        CancellationToken cancellationToken = default)
    {
        var query = new GetIssueByPositionQuery(position);

        return await _getIssueByPositionHandler.Handle(query, cancellationToken);
    }

    public async Task<Result<IEnumerable<IssueResponse>, ErrorList>> GetIssuesByModuleId(
        Guid moduleId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetIssuesByModuleIdQuery(moduleId);

        return await _getIssuesByModuleIdHandler.Handle(query, cancellationToken);
    }
}