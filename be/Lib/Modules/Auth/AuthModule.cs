using System.Text;
using Lib.Modules.Auth.Configurations;
using Lib.Modules.Auth.Entities;
using Lib.Modules.Auth.Repositories;
using Lib.Modules.Auth.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Lib.Modules.Auth;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services,
        IConfiguration config,
        IHostEnvironment env
    )
    {
        services.Configure<PasswordHasherOptions>(options =>
        {
            options.IterationCount = 300_000;
            options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
        });

        services.AddScoped<IPasswordHasher<UserCredentials>, PasswordHasher<UserCredentials>>();
        services.AddScoped<IJwtService, JwtService>();

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = "DynamicAuth";
                options.DefaultChallengeScheme = "DynamicAuth";
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JwtSettings:Access:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(config["JwtSettings:Access:Secret"]!)
                    )
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (
                            context.Request.Cookies.TryGetValue(
                                config["JwtSettings:Access:CookieName"]!,
                                out var token
                            )
                        ) context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                var isDev = env.IsDevelopment();
                options.Cookie.HttpOnly = !isDev;
                options.Cookie.SecurePolicy = isDev ? CookieSecurePolicy.SameAsRequest : CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;

                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            })
            .AddPolicyScheme("DynamicAuth", "JWT or Cookie router", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var header = context.Request.Headers.Authorization.ToString();
                    if (!string.IsNullOrEmpty(header) && header.StartsWith("Bearer "))
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }

                    return CookieAuthenticationDefaults.AuthenticationScheme;
                };
            });

        services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUserCredentialsService, UserCredentialsService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    public static void ApplyAuthConfigurations(this ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserCredentialsConfiguration());
        modelBuilder.ApplyConfiguration(new SessionConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}