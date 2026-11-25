using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Domain.Repositories;

public interface IHeldPhotoRepository
{
    Task<HeldPhoto> CreateAsync(HeldPhoto photo);
    Task<List<HeldPhoto>> ListAsync();
    Task UpdatePlacementAsync(int id, int? stack, int? position);
    Task SaveLayoutAsync(IReadOnlyList<HeldPhotoPlacement> placements);
    Task DeleteAsync(int id);
}
