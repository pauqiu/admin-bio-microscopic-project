using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Dtos.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Dtos.Slides;
using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Repositories;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Application.Services.Implementations;

internal class HeldPhotoService(IHeldPhotoRepository repository) : IHeldPhotoService
{
    public Task<List<HeldPhotoDto>> ListAsync() => repository.ListAsync();
    public Task<HeldPhotoDto> UploadAsync(Stream fileStream, string fileName) => repository.UploadAsync(fileStream, fileName);
    public Task SaveLayoutAsync(IReadOnlyList<HeldPhotoLayoutItemDto> placements) => repository.SaveLayoutAsync(placements);
    public Task DeleteAsync(int id) => repository.DeleteAsync(id);
    public Task<SlideDto> PromoteAsync(int stack, CreateSlideDto dto) => repository.PromoteAsync(stack, dto);
}