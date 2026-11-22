using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using UCR.EB.BioMicroscopeAdmin.Backend.Application.Security;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Endpoints;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Security;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation;

/// <summary>Provides extension methods for configuring dependency injection in the presentation layer.</summary>
public static class DependencyInjection
{
    /// <summary>Registers presentation layer services into the dependency injection container.</summary>
    public static IServiceCollection AddPresentationLayerServices(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.Reader, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.Administrator, AuthRoles.Reader))
            .AddPolicy(AuthPolicies.Administrator, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(AuthRoles.Administrator));
        return services;
    }

    /// <summary>Maps all presentation layer endpoints under /api.</summary>
    public static IEndpointRouteBuilder MapPresentationLayerEndpoints(this IEndpointRouteBuilder builder)
    {
        var api = builder.MapGroup("/api");
        api.AddEndpointFilter<CsrfHeaderFilter>();
        api.MapAuthEndpoints();

        var content = api.MapGroup(string.Empty);
        content.RequireAuthorization(AuthPolicies.Reader);
        RequireAdministratorForWrites(content);

        content.MapGroupEndpoints();
        content.MapBoxEndpoints();
        content.MapSlideEndpoints();
        content.MapImageEndpoints();
        return builder;
    }

    /// <summary>
    /// Applies the rule to the whole group, so a new write endpoint is protected
    /// even if whoever adds it forgets to configure authorization.
    /// </summary>
    private static void RequireAdministratorForWrites(IEndpointConventionBuilder group) =>
        group.Add(endpoint =>
        {
            var methods = endpoint.Metadata.OfType<IHttpMethodMetadata>().SelectMany(m => m.HttpMethods);
            if (methods.Any(method => !HttpMethods.IsGet(method) && !HttpMethods.IsHead(method)))
            {
                endpoint.Metadata.Add(new AuthorizeAttribute(AuthPolicies.Administrator));
            }
        });
}
