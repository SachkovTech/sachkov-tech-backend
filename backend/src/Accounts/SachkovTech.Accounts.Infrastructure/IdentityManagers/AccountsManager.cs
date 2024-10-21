using SachkovTech.Accounts.Application;
using SachkovTech.Accounts.Domain;
using SachkovTech.Accounts.Infrastructure.DbContexts;

namespace SachkovTech.Accounts.Infrastructure.IdentityManagers;

public class AccountsManager(AccountsWriteDbContext accountsWriteContext) : IAccountsManager
{
    public async Task CreateAdminAccount(AdminAccount adminAccount)
    {
        await accountsWriteContext.AdminAccounts.AddAsync(adminAccount);
        await accountsWriteContext.SaveChangesAsync();
    }

    public async Task CreateParticipantAccount(
        ParticipantAccount participantAccount,
        CancellationToken cancellationToken = default)
    {
        await accountsWriteContext.ParticipantAccounts.AddAsync(participantAccount, cancellationToken);
    }

    public async Task CreateStudentAccount(
        StudentAccount studentAccount,
        CancellationToken cancellationToken = default)
    {
        await accountsWriteContext.StudentAccounts.AddAsync(studentAccount, cancellationToken);
    }
}