using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Dtos.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Dtos.Slides;
using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Repositories;
using UCR.EB.BioMicroscopeAdmin.Frontend.Infrastructure.ApiClient;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Infrastructure.Repositories;

internal sealed class HeldPhotoRepository(BioMicroscopeApiClient client) : IHeldPhotoRepository
{
    private static HeldPhotoDto Map(HeldPhotoResponse photo) =>
        new(photo.Id, photo.FileUrl, photo.FileName, photo.UploadedAt, photo.Stack, photo.Position);

    public async Task<List<HeldPhotoDto>> ListAsync()
    {
        var result = await client.GetHeldPhotosAsync() ?? [];
        return result.Select(Map).ToList();
    }

    public async Task<HeldPhotoDto> UploadAsync(Stream fileStream, string fileName) =>
        Map(await client.UploadHeldPhotoAsync(fileStream, fileName));

    public Task SaveLayoutAsync(IReadOnlyList<HeldPhotoLayoutItemDto> placements) =>
        client.SaveHeldPhotoLayoutAsync(new SaveHeldPhotoLayoutRequest(
            placements.Select(item => new HeldPhotoLayoutItemRequest(item.Id, item.Stack, item.Position)).ToList()));

    public Task DeleteAsync(int id) => client.DeleteHeldPhotoAsync(id);

    public async Task<SlideDto> PromoteAsync(int stack, CreateSlideDto dto)
    {
        var created = await client.PromoteHeldStackAsync(
            stack, new CreateSlideRequest(dto.Code, dto.Name, dto.BoxId, dto.Description, dto.Observations));
        return new SlideDto(created.Id, created.Code, created.Name, created.BoxId, created.Description, created.Observations);
    }
}