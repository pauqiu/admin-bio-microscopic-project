using UCR.EB.BioMicroscopeAdmin.Backend.Domain.ValueObjects;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;

/// <summary>Fotografía guardada todavía sin lámina. La pila y la posición se asignan después.</summary>
public class HeldPhoto
{
    public int Id { get; }
    public ImageUrl FileUrl { get; }
    public ImageFileName FileName { get; }
    public DateTime UploadedAt { get; }
    public int? Stack { get; set; }
    public int? Position { get; set; }

    public HeldPhoto(int id, ImageUrl fileUrl, ImageFileName fileName, DateTime uploadedAt, int? stack, int? position)
    {
        Id = id;
        FileUrl = fileUrl;
        FileName = fileName;
        UploadedAt = uploadedAt;
        Stack = stack;
        Position = position;
    }

    public HeldPhoto(ImageUrl fileUrl, ImageFileName fileName)
    {
        FileUrl = fileUrl;
        FileName = fileName;
        UploadedAt = DateTime.UtcNow;
    }
}
