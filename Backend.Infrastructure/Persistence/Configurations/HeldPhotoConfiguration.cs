using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.Entities;
using UCR.EB.BioMicroscopeAdmin.Backend.Domain.ValueObjects;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Infrastructure.Persistence.Configurations;

internal sealed class HeldPhotoConfiguration : IEntityTypeConfiguration<HeldPhoto>
{
    public void Configure(EntityTypeBuilder<HeldPhoto> builder)
    {
        builder.ToTable("foto_pendiente");
        builder.HasKey(photo => photo.Id);
        builder.Property(photo => photo.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(photo => photo.FileUrl)
            .HasColumnName("url_archivo")
            .HasMaxLength(ImageUrl.MaxLength)
            .IsRequired()
            .HasConversion(new ValueConverter<ImageUrl, string>(vo => vo.Value, value => ImageUrl.Create(value)));

        builder.Property(photo => photo.FileName)
            .HasColumnName("nombre_archivo")
            .HasMaxLength(ImageFileName.MaxLength)
            .IsRequired()
            .HasConversion(new ValueConverter<ImageFileName, string>(vo => vo.Value, value => ImageFileName.Create(value)));

        builder.Property(photo => photo.UploadedAt).HasColumnName("fecha_carga").IsRequired();
        builder.Property(photo => photo.Stack).HasColumnName("pila");
        builder.Property(photo => photo.Position).HasColumnName("posicion");

        builder.ToTable("foto_pendiente", table => table.HasCheckConstraint(
            "CK_foto_pendiente_posicion",
            "posicion IS NULL OR (posicion >= 1 AND posicion <= 6)"));

        builder.HasIndex(photo => new { photo.Stack, photo.Position })
            .IsUnique()
            .HasFilter("pila IS NOT NULL AND posicion IS NOT NULL");
    }
}
