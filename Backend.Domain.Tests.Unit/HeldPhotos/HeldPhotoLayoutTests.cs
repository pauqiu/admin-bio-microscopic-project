using UCR.EB.BioMicroscopeAdmin.Backend.Domain.HeldPhotos;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Domain.Tests.Unit.HeldPhotos;

public class HeldPhotoLayoutTests
{
    [Fact]
    public void Validate_AcceptsAnUnassignedPhotoAndAContiguousPile()
    {
        var errors = HeldPhotoLayout.Validate(
        [
            new HeldPhotoPlacement(1, null, null),
            new HeldPhotoPlacement(2, 1, 1),
            new HeldPhotoPlacement(3, 1, 2)
        ]);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RejectsASeventhPhotoAGapADuplicateAndAHalfAssignment()
    {
        var seven = Enumerable.Range(1, 7).Select(i => new HeldPhotoPlacement(i, 1, i == 7 ? 6 : i)).ToList();
        var gap = HeldPhotoLayout.Validate([new HeldPhotoPlacement(1, 2, 1), new HeldPhotoPlacement(2, 2, 3)]);
        var repeated = HeldPhotoLayout.Validate([new HeldPhotoPlacement(1, 3, 1), new HeldPhotoPlacement(1, 4, 1)]);
        var half = HeldPhotoLayout.Validate([new HeldPhotoPlacement(8, 1, null)]);

        Assert.Contains(HeldPhotoLayout.Validate(seven), error => error.Contains("máximo", StringComparison.Ordinal));
        Assert.Contains(gap, error => error.Contains("sin huecos", StringComparison.Ordinal));
        Assert.Contains(repeated, error => error.Contains("repetida", StringComparison.Ordinal));
        Assert.Contains(half, error => error.Contains("juntas", StringComparison.Ordinal));
    }

    [Fact]
    public void MaxFileBytes_IsTenMegabytes()
    {
        Assert.Equal(10L * 1024 * 1024, HeldPhotoLayout.MaxFileBytes);
    }
}