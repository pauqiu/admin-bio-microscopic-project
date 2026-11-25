using UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Tests.Unit.Components;

public class PhotoStacksTests
{
    [Fact]
    public void Validate_AcceptsAnUnassignedPhotoAndAContiguousPile()
    {
        var errors = PhotoStacks.Validate(
        [
            new StackPlacement(1, null, null),
            new StackPlacement(2, 1, 1),
            new StackPlacement(3, 1, 2)
        ]);

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RejectsASeventhPhotoAGapADuplicateAndAHalfAssignment()
    {
        var seven = Enumerable.Range(1, 7).Select(i => new StackPlacement(i, 1, i == 7 ? 6 : i)).ToList();
        var gap = PhotoStacks.Validate([new StackPlacement(1, 2, 1), new StackPlacement(2, 2, 3)]);
        var repeated = PhotoStacks.Validate([new StackPlacement(1, 3, 1), new StackPlacement(1, 4, 1)]);
        var half = PhotoStacks.Validate([new StackPlacement(8, 1, null)]);

        Assert.Contains(PhotoStacks.Validate(seven), error => error.Contains("máximo", StringComparison.Ordinal));
        Assert.Contains(gap, error => error.Contains("sin huecos", StringComparison.Ordinal));
        Assert.Contains(repeated, error => error.Contains("repetida", StringComparison.Ordinal));
        Assert.Contains(half, error => error.Contains("juntas", StringComparison.Ordinal));
    }
}