namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos.HeldPhotos;

public record HeldPhotoLayoutItem(int Id, int? Stack, int? Position);

public record SaveHeldPhotoLayoutRequest(IReadOnlyList<HeldPhotoLayoutItem> Items);