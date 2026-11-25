using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Dtos.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Frontend.Application.Dtos.Slides;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Application.Repositories;

public interface IHeldPhotoRepository
{
    Task<List<HeldPhotoDto>> ListAsync();
    Task<HeldPhotoDto> UploadAsync(Stream fileStream, string fileName);
    Task SaveLayoutAsync(IReadOnlyList<HeldPhotoLayoutItemDto> placements);
    Task<SlideDto> PromoteAsync(int stack, CreateSlideDto dto);
}