using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UCR.EB.BioMicroscopeAdmin.Backend.Application.Security;
using UCR.EB.BioMicroscopeAdmin.Backend.Infrastructure.Persistence;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Infrastructure.Identity;

/// <summary>Configures ASP.NET Core Identity with a same-origin session cookie.</summary>
public static class IdentitySetup
{
    public const string CookieName = "biomicroscope.auth";

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        services.AddIdentityCore<IdentityUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 10;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager();

        // Permite que desactivar a un usuario cierre sus sesiones abiertas en poco tiempo.
        services.Configure<SecurityStampValidatorOptions>(options =>
            options.ValidationInterval = TimeSpan.FromMinutes(1));

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = CookieName;
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.Events = new CookieAuthenticationEvents
            {
                // Un API responde con códigos de estado en lugar de redirigir a una página.
                OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                },
                OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>Creates the roles and the users defined through configuration.</summary>
    public static async Task SeedIdentityAsync(this IServiceProvider rootServices)
    {
        using var scope = rootServices.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();
        var environment = services.GetRequiredService<IHostEnvironment>();
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeed");
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        var users = services.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var role in AuthRoles.All)
        {
            if (!await roles.RoleExistsAsync(role))
            {
                EnsureSucceeded(await roles.CreateAsync(new IdentityRole(role)), $"role {role}");
            }
        }

        var adminEmail = configuration["Auth:AdminEmail"];
        var adminPassword = configuration["Auth:AdminPassword"];
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            if (!(await users.GetUsersInRoleAsync(AuthRoles.Administrator)).Any())
            {
                logger.LogWarning("No administrator exists. Set Auth:AdminEmail and Auth:AdminPassword.");
            }
        }
        else
        {
            await EnsureUserAsync(users, adminEmail, adminPassword, AuthRoles.Administrator);
        }

        // El Lector de prueba solo existe en desarrollo y en los tests.
        if (!environment.IsProduction()
            && configuration["Auth:ReaderEmail"] is { Length: > 0 } readerEmail
            && configuration["Auth:ReaderPassword"] is { Length: > 0 } readerPassword)
        {
            await EnsureUserAsync(users, readerEmail, readerPassword, AuthRoles.Reader);
        }
    }

    private static async Task EnsureUserAsync(
        UserManager<IdentityUser> users, string email, string password, string role)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            EnsureSucceeded(await users.CreateAsync(user, password), "initial user");
        }

        if (!await users.IsInRoleAsync(user, role))
        {
            EnsureSucceeded(await users.AddToRoleAsync(user, role), "initial user role");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string target)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Could not create {target}: {errors}");
        }
    }
}
