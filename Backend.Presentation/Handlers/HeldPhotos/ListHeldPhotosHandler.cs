using UCR.EB.BioMicroscopeAdmin.Backend.Application.Services;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos.HeldPhotos;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Handlers.HeldPhotos;

public static class ListHeldPhotosHandler
{
    public static async Task<List<HeldPhotoResponse>> HandleAsync(IHeldPhotoService photos)
    {
        var items = await photos.ListAsync();
        return items.Select(photo => new HeldPhotoResponse(
            photo.Id, photo.FileUrl.Value, photo.FileName.Value, photo.UploadedAt, photo.Stack, photo.Position)).ToList();
    }
}