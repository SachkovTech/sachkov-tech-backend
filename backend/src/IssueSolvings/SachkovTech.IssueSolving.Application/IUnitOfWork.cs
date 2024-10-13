using System.Data.Common;

namespace SachkovTech.IssueSolving.Application;

public interface IUnitOfWork
{
    Task<DbTransaction> BeginTransaction(CancellationToken cancellationToken = default);

    Task SaveChanges(CancellationToken cancellationToken = default, DbTransaction? transaction = null);
}