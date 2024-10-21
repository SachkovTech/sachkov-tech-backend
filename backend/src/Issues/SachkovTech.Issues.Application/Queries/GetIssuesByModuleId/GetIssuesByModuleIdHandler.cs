using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Core.Dtos;
using SachkovTech.Issues.Contracts.Responses;
using SachkovTech.SharedKernel;

namespace SachkovTech.Issues.Application.Queries.GetIssuesByModuleId;

public class GetIssuesByModuleIdHandler
{
    private readonly IReadDbContext _readDbContext;

    public GetIssuesByModuleIdHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<Result<IEnumerable<IssueResponse>, ErrorList>> Handle(
        GetIssuesByModuleIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var moduleDto = await _readDbContext.Modules
            .SingleOrDefaultAsync(m => m.Id == query.ModuleId, cancellationToken);

        if (moduleDto == null)
            return Errors.General.NotFound(query.ModuleId).ToErrorList();

        return await _readDbContext.Issues
            .Where(i => i.Id == query.ModuleId)
            .Select(i => new IssueResponse(
                i.Id,
                i.ModuleId,
                i.Title,
                i.Description,
                i.Position,
                i.LessonId,
                Array.Empty<FileResponse>()
            )).ToListAsync(cancellationToken);
    }
}
