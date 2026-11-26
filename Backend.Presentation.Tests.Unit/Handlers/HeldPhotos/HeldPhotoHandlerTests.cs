using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using UCR.EB.BioMicroscopeAdmin.Backend.Application.Services;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.ValueObjects;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos.HeldPhotos;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos.Slides;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Handlers.HeldPhotos;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Tests.Unit.Handlers.HeldPhotos;

public class HeldPhotoHandlerTests
{
    [Fact]
    public async Task Upload_RejectsAFileOverTenMegabytesBeforeSavingIt()
    {
        var file = new Mock<IFormFile>();
        file.SetupGet(item => item.Length).Returns(HeldPhotoLayout.MaxFileBytes + 1);
        file.SetupGet(item => item.FileName).Returns("grande.png");
        var storage = new Mock<IFileStorageService>();
        var photos = new Mock<IHeldPhotoService>();

        var result = await UploadHeldPhotoHandler.HandleAsync(file.Object, storage.Object, photos.Object);

        Assert.IsType<BadRequest<ErrorResponse>>(result.Result);
        storage.Verify(item => item.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SaveLayout_RejectsAPileOfSeven()
    {
        var existing = Enumerable.Range(1, 7)
            .Select(id => new HeldPhoto(id, ImageUrl.Create($"/uploads/{id}.png"), ImageFileName.Create($"{id}.png"), DateTime.UtcNow, null, null))
            .ToList();
        var photos = new Mock<IHeldPhotoService>();
        photos.Setup(item => item.ListAsync()).ReturnsAsync(existing);
        var items = existing.Select(photo => new HeldPhotoLayoutItem(photo.Id, 1, photo.Id == 7 ? 6 : photo.Id)).ToList();

        var result = await SaveHeldPhotoLayoutHandler.HandleAsync(new SaveHeldPhotoLayoutRequest(items), photos.Object);

        Assert.IsType<BadRequest<ErrorResponse>>(result.Result);
        photos.Verify(item => item.SaveLayoutAsync(It.IsAny<IReadOnlyList<HeldPhotoPlacement>>()), Times.Never);
    }

    [Fact]
    public async Task Promote_CreatesTheSlideWithPositionsAsOrdersAndDeletesThePile()
    {
        var pile = new List<HeldPhoto>
        {
            new(4, ImageUrl.Create("/uploads/lejos.png"), ImageFileName.Create("lejos.png"), DateTime.UtcNow, 2, 1),
            new(5, ImageUrl.Create("/uploads/cerca.png"), ImageFileName.Create("cerca.png"), DateTime.UtcNow, 2, 2)
        };
        var photos = new Mock<IHeldPhotoService>();
        photos.Setup(item => item.ListAsync()).ReturnsAsync(pile);
        var boxes = new Mock<IBoxService>();
        boxes.Setup(item => item.FindByIdAsync(9)).ReturnsAsync(new Box(9, BoxNumber.Create(1), BoxName.Create("Caja"), 1));
        var orders = new List<int>();
        var images = new Mock<IImageService>();
        images.Setup(item => item.CreateAsync(It.IsAny<Image>()))
            .Callback<Image>(image => orders.Add(image.Order.Value))
            .ReturnsAsync((Image image) => image);
        var slides = new Mock<ISlideService>();
        slides.Setup(item => item.CreateAsync(It.IsAny<Slide>()))
            .ReturnsAsync((Slide slide) => new Slide(8, slide.Code, slide.Name, slide.BoxId, slide.Description, slide.Observations));

        var result = await PromoteHeldStackHandler.HandleAsync(
            2, new CreateSlideRequest("VISOR02", "Hoja", 9, null, null),
            photos.Object, slides.Object, images.Object, boxes.Object);

        var created = Assert.IsType<Created<SlideResponse>>(result.Result);
        Assert.Equal(8, created.Value!.Id);
        Assert.Equal(new[] { 1, 2 }, orders);
        photos.Verify(item => item.DeleteAsync(4), Times.Once);
        photos.Verify(item => item.DeleteAsync(5), Times.Once);
    }

    [Fact]
    public async Task Promote_WhenTheSecondImageThrows_DeletesTheSlideAndKeepsTheHeldPhotos()
    {
        var pile = new List<HeldPhoto>
        {
            new(4, ImageUrl.Create("/uploads/lejos.png"), ImageFileName.Create("lejos.png"), DateTime.UtcNow, 2, 1),
            new(5, ImageUrl.Create("/uploads/cerca.png"), ImageFileName.Create("cerca.png"), DateTime.UtcNow, 2, 2)
        };
        var photos = new Mock<IHeldPhotoService>();
        photos.Setup(item => item.ListAsync()).ReturnsAsync(pile);
        var boxes = new Mock<IBoxService>();
        boxes.Setup(item => item.FindByIdAsync(9)).ReturnsAsync(new Box(9, BoxNumber.Create(1), BoxName.Create("Caja"), 1));
        var calls = 0;
        var images = new Mock<IImageService>();
        images.Setup(item => item.CreateAsync(It.IsAny<Image>()))
            .ReturnsAsync((Image image) =>
            {
                calls++;
                if (calls == 2) throw new InvalidOperationException("disco");
                return image;
            });
        var slides = new Mock<ISlideService>();
        slides.Setup(item => item.CreateAsync(It.IsAny<Slide>()))
            .ReturnsAsync((Slide slide) => new Slide(8, slide.Code, slide.Name, slide.BoxId, slide.Description, slide.Observations));
        slides.Setup(item => item.DeleteAsync(8)).Returns(Task.CompletedTask);

        var result = await PromoteHeldStackHandler.HandleAsync(
            2, new CreateSlideRequest("VISOR02", "Hoja", 9, null, null),
            photos.Object, slides.Object, images.Object, boxes.Object);

        var bad = Assert.IsType<BadRequest<ErrorResponse>>(result.Result);
        Assert.Equal("No se pudo crear la lámina. Las fotos siguen en la lámina.", Assert.Single(bad.Value!.Errors).Message);
        slides.Verify(item => item.DeleteAsync(8), Times.Once);
        photos.Verify(item => item.DeleteAsync(It.IsAny<int>()), Times.Never);
    }
}