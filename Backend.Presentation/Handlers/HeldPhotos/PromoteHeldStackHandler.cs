using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.EB.BioMicroscopeAdmin.Backend.Application.Services;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Exceptions;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.ValueObjects;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos.Slides;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Mappers;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Handlers.HeldPhotos;

public static class PromoteHeldStackHandler
{
    public static async Task<Results<Created<SlideResponse>, BadRequest<ErrorResponse>, NotFound<string>>> HandleAsync(
        int stack,
        CreateSlideRequest request,
        IHeldPhotoService photos,
        ISlideService slides,
        IImageService images,
        IBoxService boxes)
    {
        if (await boxes.FindByIdAsync(request.BoxId) is null)
            return TypedResults.NotFound($"Box {request.BoxId} not found.");

        var pile = (await photos.ListAsync()).Where(photo => photo.Stack == stack).OrderBy(photo => photo.Position).ToList();
        if (pile.Count == 0)
            return TypedResults.BadRequest(new ErrorResponse([new ValidationError("Stack", "La lámina no tiene fotos.")]));

        var errors = HeldPhotoLayout.Validate(pile.Select(photo => new HeldPhotoPlacement(photo.Id, photo.Stack, photo.Position)).ToList());
        if (errors.Count > 0)
            return TypedResults.BadRequest(new ErrorResponse(errors.Select(error => new ValidationError("Stack", error)).ToList()));

        Slide created;
        try
        {
            created = await slides.CreateAsync(SlideMapper.ToEntity(request));
        }
        catch (ValidationException ex)
        {
            return TypedResults.BadRequest(new ErrorResponse(ex.Errors));
        }

        try
        {
            foreach (var photo in pile)
            {
                var image = new Image(photo.FileUrl, photo.FileName, ImageOrder.Create(photo.Position!.Value), created.Id);
                await images.CreateAsync(image);
            }
        }
        catch (Exception)
        {
            await slides.DeleteAsync(created.Id);
            return TypedResults.BadRequest(new ErrorResponse([
                new ValidationError("Stack", "No se pudo crear la lámina. Las fotos siguen en la lámina.")
            ]));
        }

        foreach (var photo in pile)
            await photos.DeleteAsync(photo.Id);

        return TypedResults.Created($"/slides/{created.Id}", SlideMapper.ToResponse(created));
    }
}