using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using UCR.EB.BioMicroscopeAdmin.Backend.Application.Security;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Endpoints;

public record LoginRequest(string Email, string Password);

public record CurrentUserResponse(string Email, IReadOnlyList<string> Roles);

/// <summary>Maps the session endpoints.</summary>
public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder builder)
    {
        var auth = builder.MapGroup("/auth");
        auth.MapPost("/login", LoginAsync).AllowAnonymous();
        auth.MapPost("/logout", LogoutAsync).RequireAuthorization(AuthPolicies.Reader);
        auth.MapGet("/me", GetCurrentUser).RequireAuthorization(AuthPolicies.Reader);
        return builder;
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        [FromServices] UserManager<IdentityUser> users,
        [FromServices] SignInManager<IdentityUser> signIn)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return Results.Unauthorized();
        }

        var user = await users.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            return Results.Unauthorized();
        }

        var result = await signIn.PasswordSignInAsync(
            user, request.Password, isPersistent: false, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return Results.Unauthorized();
        }

        var roles = await users.GetRolesAsync(user);
        return Results.Ok(new CurrentUserResponse(user.Email!, roles.ToList()));
    }

    private static async Task<IResult> LogoutAsync([FromServices] SignInManager<IdentityUser> signIn)
    {
        await signIn.SignOutAsync();
        return Results.NoContent();
    }

    private static IResult GetCurrentUser(ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(ClaimTypes.Email) ?? user.Identity?.Name ?? string.Empty;
        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        return Results.Ok(new CurrentUserResponse(email, roles));
    }
}
