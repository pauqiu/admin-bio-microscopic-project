using Microsoft.EntityFrameworkCore;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Exceptions;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;
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

    public async Task SaveLayoutAsync(IReadOnlyList<HeldPhotoPlacement> placements)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        var ids = placements.Select(item => item.Id).ToList();
        var rows = await context.HeldPhotos.Where(photo => ids.Contains(photo.Id)).ToListAsync();
        var found = rows.Select(photo => photo.Id).ToHashSet();
        var missing = ids.Where(id => !found.Contains(id)).Cast<int?>().FirstOrDefault();
        if (missing is int missingId)
            throw new NotFoundException($"Held photo {missingId} not found.");

        foreach (var row in rows)
        {
            row.Stack = null;
            row.Position = null;
        }

        await context.SaveChangesAsync();

        var byId = placements.ToDictionary(item => item.Id);
        foreach (var row in rows)
        {
            var placement = byId[row.Id];
            row.Stack = placement.Stack;
            row.Position = placement.Position;
        }

        await context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await context.HeldPhotos.Where(photo => photo.Id == id).ExecuteDeleteAsync();
        if (deleted == 0) throw new NotFoundException($"Held photo {id} not found.");
    }
}
