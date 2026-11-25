using UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Tests.Unit.Components;

public class ZoomStepsTests
{
    [Theory]
    [InlineData(-1, 3, 0)]
    [InlineData(1, 3, 1)]
    [InlineData(5, 3, 2)]
    [InlineData(2, 0, 0)]
    public void Clamp_KeepsIndexInsideTheImages(int index, int count, int expected)
    {
        Assert.Equal(expected, ZoomSteps.Clamp(index, count));
    }

    [Fact]
    public void CanZoomIn_StopsAtTheClosestPhoto()
    {
        Assert.True(ZoomSteps.CanZoomIn(0, 3));
        Assert.True(ZoomSteps.CanZoomIn(1, 3));
        Assert.False(ZoomSteps.CanZoomIn(2, 3));
        Assert.False(ZoomSteps.CanZoomIn(0, 1));
    }

    [Fact]
    public void CanZoomOut_StopsAtTheFarthestPhoto()
    {
        Assert.False(ZoomSteps.CanZoomOut(0));
        Assert.True(ZoomSteps.CanZoomOut(1));
    }

    [Fact]
    public void LayerScale_EnlargesPassedPhotosAndShrinksUpcomingOnes()
    {
        Assert.Equal(1, ZoomSteps.LayerScale(0));
        Assert.Equal(ZoomSteps.TransitionScale, ZoomSteps.LayerScale(-1));
        Assert.Equal(1 / ZoomSteps.TransitionScale, ZoomSteps.LayerScale(2));
    }

    [Theory]
    [InlineData(0, 1, "Una sola foto")]
    [InlineData(0, 3, "Aumento 1 de 3 · la más lejana")]
    [InlineData(1, 3, "Aumento 2 de 3")]
    [InlineData(2, 3, "Aumento 3 de 3 · la más cercana")]
    public void Describe_SaysWhichPhotoIsShowing(int index, int count, string expected)
    {
        Assert.Equal(expected, ZoomSteps.Describe(index, count));
    }
}
