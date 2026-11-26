using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Application.Services;

public interface IHeldPhotoService
{
    Task<HeldPhoto> CreateAsync(HeldPhoto photo);
    Task<List<HeldPhoto>> ListAsync();
    Task SaveLayoutAsync(IReadOnlyList<HeldPhotoPlacement> placements);
    Task DeleteAsync(int id);
}