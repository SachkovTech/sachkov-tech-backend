using SachkovTech.Core.Abstractions;

namespace SachkovTech.Issues.Application.Queries.GetIssuesByModuleId;

public record GetIssuesByModuleIdQuery(Guid ModuleId) : IQuery;
