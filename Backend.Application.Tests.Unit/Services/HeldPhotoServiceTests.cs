using UCR.EB.BioMicroscopeAdmin.Backend.Application.Services.Implementations;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Repositories;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Application.Tests.Unit.Services;

public class HeldPhotoServiceTests
{
    [Fact]
    public async Task SaveLayoutAsync_SwapPositionsOnSameStack_PersistsBothPlacements()
    {
        var repo = new UniqueIndexHeldPhotoRepository();
        repo.Seed(4, 2, 1);
        repo.Seed(5, 2, 2);
        var service = new HeldPhotoService(repo);

        await service.SaveLayoutAsync(
        [
            new HeldPhotoPlacement(4, 2, 2),
            new HeldPhotoPlacement(5, 2, 1),
        ]);

        Assert.Equal((2, 2), repo.GetPlacement(4));
        Assert.Equal((2, 1), repo.GetPlacement(5));
    }

    private sealed class UniqueIndexHeldPhotoRepository : IHeldPhotoRepository
    {
        private readonly Dictionary<int, (int? stack, int? position)> _placements = new();

        public void Seed(int id, int stack, int position) =>
            _placements[id] = (stack, position);

        public (int? stack, int? position) GetPlacement(int id) => _placements[id];

        public Task UpdatePlacementAsync(int id, int? stack, int? position)
        {
            if (stack is not null && position is not null)
            {
                foreach (var (otherId, placement) in _placements)
                {
                    if (otherId != id && placement.stack == stack && placement.position == position)
                        throw new InvalidOperationException();
                }
            }

            _placements[id] = (stack, position);
            return Task.CompletedTask;
        }

        public Task<HeldPhoto> CreateAsync(HeldPhoto photo) =>
            throw new NotImplementedException();

        public Task<List<HeldPhoto>> ListAsync() =>
            throw new NotImplementedException();

        public Task DeleteAsync(int id) =>
            throw new NotImplementedException();
    }
}