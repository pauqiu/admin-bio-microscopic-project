using Microsoft.AspNetCore.Http;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Security;

/// <summary>
/// Rejects state-changing requests without the custom header. A cross-site form
/// cannot set it, which complements the SameSite session cookie.
/// </summary>
public sealed class CsrfHeaderFilter : IEndpointFilter
{
    public const string HeaderName = "X-Requested-With";
    public const string HeaderValue = "BioMicroscope";

    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.HttpContext.Request;
        var isSafe = HttpMethods.IsGet(request.Method)
            || HttpMethods.IsHead(request.Method)
            || HttpMethods.IsOptions(request.Method);

        if (!isSafe && request.Headers[HeaderName] != HeaderValue)
        {
            return ValueTask.FromResult<object?>(Results.StatusCode(StatusCodes.Status403Forbidden));
        }

        return next(context);
    }
}
