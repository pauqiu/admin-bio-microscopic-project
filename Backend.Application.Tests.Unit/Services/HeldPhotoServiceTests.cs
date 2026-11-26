using Moq;
using UCR.EB.BioMicroscopeAdmin.Backend.Application.Services.Implementations;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Repositories;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Application.Tests.Unit.Services;

public class HeldPhotoServiceTests
{
    [Fact]
    public async Task SaveLayoutAsync_CallsTheRepositoryOnceAndDoesNotUpdateEachPhoto()
    {
        var placements = new List<HeldPhotoPlacement>
        {
            new(4, 2, 2),
            new(5, 2, 1),
        };
        var repo = new Mock<IHeldPhotoRepository>();
        var service = new HeldPhotoService(repo.Object);

        await service.SaveLayoutAsync(placements);

        repo.Verify(item => item.SaveLayoutAsync(placements), Times.Once);
        repo.Verify(
            item => item.UpdatePlacementAsync(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>()),
            Times.Never);
    }
}
