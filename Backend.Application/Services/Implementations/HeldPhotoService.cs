using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Repositories;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Application.Services.Implementations;

internal class HeldPhotoService(IHeldPhotoRepository repository) : IHeldPhotoService
{
    public Task<HeldPhoto> CreateAsync(HeldPhoto photo) => repository.CreateAsync(photo);

    public Task<List<HeldPhoto>> ListAsync() => repository.ListAsync();

    public async Task SaveLayoutAsync(IReadOnlyList<HeldPhotoPlacement> placements)
    {
        foreach (var item in placements)
            await repository.UpdatePlacementAsync(item.Id, item.Stack, item.Position);
    }

    public Task DeleteAsync(int id) => repository.DeleteAsync(id);
}