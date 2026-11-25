namespace UCR.EB.BioMicroscopeAdmin.Frontend.Application.Dtos.HeldPhotos;

public record HeldPhotoDto(int Id, string FileUrl, string FileName, DateTime UploadedAt, int? Stack, int? Position);

public record HeldPhotoLayoutItemDto(int Id, int? Stack, int? Position);