using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.EB.BioMicroscopeAdmin.Backend.Application.Services;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Handlers.HeldPhotos;

public static class DeleteHeldPhotoHandler
{
    public static async Task<Results<NoContent, NotFound<string>>> HandleAsync(
        int id,
        IHeldPhotoService photos,
        IFileStorageService storage)
    {
        var existing = (await photos.ListAsync()).FirstOrDefault(photo => photo.Id == id);
        if (existing is null)
            return TypedResults.NotFound($"Held photo {id} not found.");

        await photos.DeleteAsync(id);
        await storage.DeleteAsync(existing.FileUrl.Value);
        return TypedResults.NoContent();
    }
}
