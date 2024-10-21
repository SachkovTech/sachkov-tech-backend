using Microsoft.EntityFrameworkCore;
using SachkovTech.Accounts.Domain;
using SachkovTech.Accounts.Infrastructure.DbContexts;

namespace SachkovTech.Accounts.Infrastructure.IdentityManagers;

public class RolePermissionManager(AccountsWriteDbContext accountsWriteContext)
{
    public async Task AddRangeIfExist(Guid roleId, IEnumerable<string> permissions)
    {
        foreach (var permissionCode in permissions)
        {
            var permission = await accountsWriteContext.Permissions.FirstOrDefaultAsync(p => p.Code == permissionCode);
            if (permission == null)
                throw new ApplicationException($"Permission code {permissionCode} is not found.");

            var rolePermissionExist = await accountsWriteContext.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id);

            if (rolePermissionExist)
                continue;

            accountsWriteContext.RolePermissions.Add(new RolePermission
            {
                RoleId = roleId,
                PermissionId = permission!.Id
            });
        }

        await accountsWriteContext.SaveChangesAsync();
    }
}