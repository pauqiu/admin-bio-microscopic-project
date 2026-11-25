using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Handlers.HeldPhotos;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Endpoints;

public static class HeldPhotoEndpoints
{
    public static IEndpointRouteBuilder MapHeldPhotoEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/held-photos", UploadHeldPhotoHandler.HandleAsync).DisableAntiforgery();
        builder.MapGet("/held-photos", ListHeldPhotosHandler.HandleAsync);
        builder.MapPut("/held-photos/layout", SaveHeldPhotoLayoutHandler.HandleAsync);
        builder.MapPost("/held-photos/stacks/{stack:int}/slide", PromoteHeldStackHandler.HandleAsync);
        return builder;
    }
}