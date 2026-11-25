using UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Tests.Unit.Components;

public class UploadBatchTests
{
    private static PlannedUpload Item(int index, int order) => new(index, $"f{index}.png", order);

    [Fact]
    public async Task RunAsync_UploadsEveryFileInSelectionOrder()
    {
        var seen = new List<int>();

        var results = await UploadBatch.RunAsync(
            [Item(0, 1), Item(1, 2), Item(2, 3)],
            item =>
            {
                seen.Add(item.Order);
                return Task.CompletedTask;
            });

        Assert.Equal(new[] { 1, 2, 3 }, seen);
        Assert.All(results, result => Assert.True(result.Succeeded));
        Assert.All(results, result => Assert.Null(result.Error));
    }

    [Fact]
    public async Task RunAsync_KeepsGoingAfterOneFileFails()
    {
        var results = await UploadBatch.RunAsync(
            [Item(0, 1), Item(1, 2), Item(2, 3)],
            item => item.Order == 2
                ? throw new InvalidOperationException("disco lleno")
                : Task.CompletedTask);

        Assert.True(results[0].Succeeded);
        Assert.False(results[1].Succeeded);
        Assert.Equal("disco lleno", results[1].Error);
        Assert.Equal(1, results[1].Index);
        Assert.True(results[2].Succeeded);
    }

    [Fact]
    public async Task RunAsync_WithNothingPlanned_ReturnsAnEmptyList()
    {
        var results = await UploadBatch.RunAsync([], _ => throw new InvalidOperationException("no debía llamarse"));

        Assert.Empty(results);
    }
}