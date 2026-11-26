using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Repositories;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Application.Services.Implementations;

internal class HeldPhotoService(IHeldPhotoRepository repository) : IHeldPhotoService
{
    public Task<HeldPhoto> CreateAsync(HeldPhoto photo) => repository.CreateAsync(photo);

    public Task<List<HeldPhoto>> ListAsync() => repository.ListAsync();

    public Task SaveLayoutAsync(IReadOnlyList<HeldPhotoPlacement> placements) =>
        repository.SaveLayoutAsync(placements);

    public Task DeleteAsync(int id) => repository.DeleteAsync(id);
}