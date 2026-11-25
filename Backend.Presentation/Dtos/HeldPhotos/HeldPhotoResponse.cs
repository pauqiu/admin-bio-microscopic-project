namespace UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Dtos.HeldPhotos;

public record HeldPhotoResponse(int Id, string FileUrl, string FileName, DateTime UploadedAt, int? Stack, int? Position);