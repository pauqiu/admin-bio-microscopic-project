using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Dtos.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Dtos.Slides;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Application.Services;

public interface IHeldPhotoService
{
    Task<List<HeldPhotoDto>> ListAsync();
    Task<HeldPhotoDto> UploadAsync(Stream fileStream, string fileName);
    Task SaveLayoutAsync(IReadOnlyList<HeldPhotoLayoutItemDto> placements);
    Task DeleteAsync(int id);
    Task<SlideDto> PromoteAsync(int stack, CreateSlideDto dto);
}