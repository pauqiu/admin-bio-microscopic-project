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

    [Fact]
    public void Pack_SplitsThirteenPhotosIntoPilesOfSixSixAndOne()
    {
        var uploaded = Enumerable.Range(0, 13)
            .Select(index => new UploadedSelection(index + 1, index))
            .ToList();

        var packed = PhotoStacks.Pack(uploaded, 1);

        Assert.Equal(6, packed.Count(photo => photo.Stack == 1));
        Assert.Equal(6, packed.Count(photo => photo.Stack == 2));
        Assert.Equal(1, packed.Count(photo => photo.Stack == 3));
        Assert.Equal(Enumerable.Range(1, 6), Positions(packed, 1));
        Assert.Equal(Enumerable.Range(1, 6), Positions(packed, 2));
        Assert.Equal(1, packed.Single(photo => photo.Stack == 3).Position);
    }

    [Fact]
    public void Pack_KeepsTheNextPileWhenASelectionIndexIsMissing()
    {
        var uploaded = Enumerable.Range(0, 7)
            .Where(index => index != 2)
            .Select(index => new UploadedSelection(100 + index, index))
            .ToList();

        var packed = PhotoStacks.Pack(uploaded, 1);
        var first = packed.Where(photo => photo.Stack == 1).OrderBy(photo => photo.Position).ToList();

        Assert.Equal(5, first.Count);
        Assert.Equal(Enumerable.Range(1, 5), first.Select(photo => photo.Position!.Value));
        Assert.Equal(new[] { 100, 101, 103, 104, 105 }, first.Select(photo => photo.Id));
        var next = Assert.Single(packed, photo => photo.Id == 106);
        Assert.Equal(2, next.Stack);
        Assert.Equal(1, next.Position);
    }

    private static IEnumerable<int> Positions(IReadOnlyList<StackPlacement> packed, int stack) =>
        packed.Where(photo => photo.Stack == stack).OrderBy(photo => photo.Position).Select(photo => photo.Position!.Value);
}