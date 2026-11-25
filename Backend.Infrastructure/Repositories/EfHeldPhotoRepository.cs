using Microsoft.EntityFrameworkCore;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Exceptions;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Repositories;
using UCR.EB.BioMicroscopeAdmin.Backend.Infrastructure.Persistence;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Infrastructure.Repositories;

internal sealed class EfHeldPhotoRepository(AppDbContext context) : IHeldPhotoRepository
{
    public async Task<HeldPhoto> CreateAsync(HeldPhoto photo)
    {
        context.HeldPhotos.Add(photo);
        await context.SaveChangesAsync();
        return photo;
    }

    public Task<List<HeldPhoto>> ListAsync() =>
        context.HeldPhotos.AsNoTracking().OrderBy(photo => photo.Id).ToListAsync();

    public async Task UpdatePlacementAsync(int id, int? stack, int? position)
    {
        var tracked = await context.HeldPhotos.FindAsync(id)
            ?? throw new NotFoundException($"Held photo {id} not found.");
        tracked.Stack = stack;
        tracked.Position = position;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await context.HeldPhotos.Where(photo => photo.Id == id).ExecuteDeleteAsync();
        if (deleted == 0) throw new NotFoundException($"Held photo {id} not found.");
    }
}
