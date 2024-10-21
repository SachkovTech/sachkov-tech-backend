using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SachkovTech.Accounts.Application;
using SachkovTech.Accounts.Domain;
using SachkovTech.Accounts.Infrastructure.DbContexts;
using SachkovTech.Accounts.Infrastructure.IdentityManagers;
using SachkovTech.Accounts.Infrastructure.Options;
using SachkovTech.Accounts.Infrastructure.Seeding;
using SachkovTech.Core.Abstractions;
using SachkovTech.Core.Options;
using SachkovTech.SharedKernel;

namespace SachkovTech.Accounts.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ITokenProvider, JwtTokenProvider>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JWT));
        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.ADMIN));

        services.RegisterIdentity();

        services.AddScoped<AccountsWriteDbContext>();

        services.AddSingleton<AccountsSeeder>();
        services.AddScoped<AccountsSeederService>();
        
        services.AddKeyedScoped<IUnitOfWork, UnitOfWork>(Modules.Accounts);
        return services;
    }

    private static void RegisterIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<User, Role>(options => { options.User.RequireUniqueEmail = true; })
            .AddEntityFrameworkStores<AccountsWriteDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<PermissionManager>();
        services.AddScoped<RolePermissionManager>();
        services.AddScoped<IAccountsManager, AccountsManager>();
        services.AddScoped<AccountsManager>();
        services.AddScoped<IRefreshSessionManager, RefreshSessionManager>();
    }
}