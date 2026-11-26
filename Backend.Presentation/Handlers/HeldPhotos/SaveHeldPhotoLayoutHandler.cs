using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.EB.BioMicroscopeAdmin.Backend.Application.Services;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Exceptions;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos.HeldPhotos;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Handlers.HeldPhotos;

public static class SaveHeldPhotoLayoutHandler
{
    public static async Task<Results<NoContent, BadRequest<ErrorResponse>>> HandleAsync(
        SaveHeldPhotoLayoutRequest request,
        IHeldPhotoService photos)
    {
        var existing = await photos.ListAsync();
        var sentIds = request.Items.Select(item => item.Id).ToList();
        if (!existing.Select(photo => photo.Id).ToHashSet().SetEquals(sentIds) || sentIds.Distinct().Count() != sentIds.Count)
            return TypedResults.BadRequest(new ErrorResponse([new ValidationError("Layout", "El armado tiene que incluir cada foto guardada una sola vez.")]));

        var placements = request.Items.Select(item => new HeldPhotoPlacement(item.Id, item.Stack, item.Position)).ToList();
        var errors = HeldPhotoLayout.Validate(placements);
        if (errors.Count > 0)
            return TypedResults.BadRequest(new ErrorResponse(errors.Select(error => new ValidationError("Layout", error)).ToList()));

        await photos.SaveLayoutAsync(placements);
        return TypedResults.NoContent();
    }
}