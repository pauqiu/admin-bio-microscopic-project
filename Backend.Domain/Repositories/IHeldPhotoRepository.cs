using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Domain.Repositories;

public interface IHeldPhotoRepository
{
    Task<HeldPhoto> CreateAsync(HeldPhoto photo);
    Task<List<HeldPhoto>> ListAsync();
    Task UpdatePlacementAsync(int id, int? stack, int? position);
    Task DeleteAsync(int id);
}
