using UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Tests.Unit.Components;

public class UploadPlanTests
{
    private static UploadCandidate File(int index, string name, long length) => new(index, name, length);

    [Fact]
    public void Plan_OnAnEmptySlide_AssignsSelectionOrderFromOne()
    {
        var files = new List<UploadCandidate>
        {
            File(0, "lejos.png", 100),
            File(1, "medio.png", 100),
            File(2, "cerca.png", 100)
        };

        var result = UploadPlan.Plan([], files);

        Assert.Equal(new[] { 1, 2, 3 }, result.Accepted.Select(a => a.Order));
        Assert.Equal(new[] { "lejos.png", "medio.png", "cerca.png" }, result.Accepted.Select(a => a.FileName));
        Assert.Empty(result.Rejected);
    }

    [Fact]
    public void Plan_FillsOnlyFreeOrders()
    {
        var files = new List<UploadCandidate> { File(0, "a.png", 100), File(1, "b.png", 100) };

        var result = UploadPlan.Plan([1, 2], files);

        Assert.Equal(new[] { 3, 4 }, result.Accepted.Select(a => a.Order));
    }

    [Fact]
    public void Plan_RejectsFilesPastSixWithoutTakingASlotFromEarlierRejections()
    {
        var files = new List<UploadCandidate>
        {
            File(0, "vacia.png", 0),
            File(1, "grande.png", UploadPlan.MaxFileBytes + 1),
            File(2, "cabe.png", UploadPlan.MaxFileBytes)
        };

        var result = UploadPlan.Plan([1, 2, 3, 4, 5], files);

        Assert.Equal("El archivo está vacío.", result.Rejected[0].Reason);
        Assert.Equal("Supera 10 MB.", result.Rejected[1].Reason);
        Assert.Equal([(2, "cabe.png", 6)], result.Accepted.Select(a => (a.Index, a.FileName, a.Order)));
    }

    [Fact]
    public void Plan_OnAFullSlide_RejectsEveryFile()
    {
        var result = UploadPlan.Plan([1, 2, 3, 4, 5, 6], [File(0, "otra.png", 100)]);

        Assert.Empty(result.Accepted);
        Assert.Equal("La lámina ya llega a 6 fotos.", result.Rejected.Single().Reason);
    }

    [Fact]
    public void Plan_KeepsTheFirstSixOfALongerSelection()
    {
        var files = Enumerable.Range(0, 7).Select(i => File(i, $"f{i}.png", 100)).ToList();

        var result = UploadPlan.Plan([], files);

        Assert.Equal(Enumerable.Range(1, 6), result.Accepted.Select(a => a.Order));
        Assert.Equal("La lámina ya llega a 6 fotos.", result.Rejected.Single().Reason);
        Assert.Equal(6, result.Rejected.Single().Index);
    }
}