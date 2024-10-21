using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SachkovTech.Accounts.Application;
using SachkovTech.Accounts.Domain;
using SachkovTech.Accounts.Infrastructure.DbContexts;
using SachkovTech.SharedKernel;

namespace SachkovTech.Accounts.Infrastructure.IdentityManagers;

public class RefreshSessionManager(AccountsWriteDbContext accountsWriteContext) : IRefreshSessionManager
{
    public async Task<Result<RefreshSession, Error>> GetByRefreshToken(
        Guid refreshToken, CancellationToken cancellationToken)
    {
        var refreshSession = await accountsWriteContext.RefreshSessions
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.RefreshToken == refreshToken, cancellationToken);

        if (refreshSession is null)
            return Errors.General.NotFound(refreshToken);

        return refreshSession;
    }

    public void Delete(RefreshSession refreshSession)
    {
        accountsWriteContext.RefreshSessions.Remove(refreshSession);
    }
}