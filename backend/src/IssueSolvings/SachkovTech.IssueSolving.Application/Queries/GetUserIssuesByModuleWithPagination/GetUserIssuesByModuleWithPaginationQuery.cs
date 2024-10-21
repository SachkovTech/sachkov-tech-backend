using SachkovTech.Core.Abstractions;

namespace SachkovTech.IssueSolving.Application.Commands.GetByModule;

public record GetUserIssuesByModuleWithPaginationQuery(Guid ModuleId) : IQuery;
