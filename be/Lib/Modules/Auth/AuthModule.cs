using Lib.Modules.Auth.Configurations;
using Lib.Modules.Auth.Entities;
using Lib.Modules.Auth.Repositories;
using Lib.Modules.Auth.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lib.Modules.Auth;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services)
    {
        services.Configure<PasswordHasherOptions>(options =>
        {
            options.IterationCount = 300_000;
            options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
        });
        services.AddScoped<IPasswordHasher<UserCredentials>, PasswordHasher<UserCredentials>>();
        
        services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        services.AddScoped<IUserCredentialsService, UserCredentialsService>();
        
        return services;
    }
    
    public static void ApplyAuthConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserCredentialsConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}