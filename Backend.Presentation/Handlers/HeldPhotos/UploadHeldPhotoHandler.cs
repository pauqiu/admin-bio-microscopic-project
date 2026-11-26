using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.EB.BioMicroscopeAdmin.Backend.Application.Services;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Exceptions;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.ValueObjects;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos.HeldPhotos;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Handlers.HeldPhotos;

public static class UploadHeldPhotoHandler
{
    public static async Task<Results<Created<HeldPhotoResponse>, BadRequest<ErrorResponse>>> HandleAsync(
        IFormFile file,
        IFileStorageService fileStorageService,
        IHeldPhotoService photos)
    {
        if (file is null || file.Length == 0)
            return TypedResults.BadRequest(new ErrorResponse([new ValidationError("File", "El archivo está vacío.")]));
        if (file.Length > HeldPhotoLayout.MaxFileBytes)
            return TypedResults.BadRequest(new ErrorResponse([new ValidationError("File", "Supera 10 MB.")]));
        if (!ImageFileName.TryCreate(file.FileName, out var fileName))
            return TypedResults.BadRequest(new ErrorResponse([new ValidationError("File", $"File name must be at most {ImageFileName.MaxLength} characters.")]));

        var fileUrl = await fileStorageService.SaveAsync(file.OpenReadStream(), file.FileName);
        if (!ImageUrl.TryCreate(fileUrl, out var url))
            return TypedResults.BadRequest(new ErrorResponse([new ValidationError("File", "Generated file URL is invalid.")]));

        var created = await photos.CreateAsync(new HeldPhoto(url!, fileName!));
        var response = new HeldPhotoResponse(created.Id, created.FileUrl.Value, created.FileName.Value, created.UploadedAt, created.Stack, created.Position);
        return TypedResults.Created($"/held-photos/{created.Id}", response);
    }
}