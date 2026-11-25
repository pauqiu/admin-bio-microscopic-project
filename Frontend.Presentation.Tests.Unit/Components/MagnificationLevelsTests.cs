using UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Tests.Unit.Components;

public class MagnificationLevelsTests
{
    [Fact]
    public void Objectives_AreTheUsualLabObjectives()
    {
        Assert.Equal([4, 10, 40, 100], MagnificationLevels.Objectives);
    }

    [Theory]
    [InlineData(0, "4x", 1.0)]
    [InlineData(1, "10x", 2.5)]
    [InlineData(2, "40x", 10.0)]
    [InlineData(3, "100x", 25.0)]
    public void Level_HasLabelAndScaleRelativeToFirstObjective(int level, string label, double scale)
    {
        Assert.Equal(label, MagnificationLevels.Label(level));
        Assert.Equal(scale, MagnificationLevels.Scale(level));
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(4, 3)]
    [InlineData(99, 3)]
    public void Clamp_KeepsLevelInRange(int level, int expected)
    {
        Assert.Equal(expected, MagnificationLevels.Clamp(level));
    }
}
