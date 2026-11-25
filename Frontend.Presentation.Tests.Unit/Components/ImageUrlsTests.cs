using UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Tests.Unit.Components;

public class ImageUrlsTests
{
    [Fact]
    public void Resolve_RelativePath_PrefixesApiBaseUrl()
    {
        var url = ImageUrls.Resolve("/uploads/a.png", "http://api.test/");

        Assert.Equal("http://api.test/uploads/a.png", url);
    }

    [Fact]
    public void Resolve_PathWithoutLeadingSlash_AddsSeparator()
    {
        var url = ImageUrls.Resolve("uploads/a.png", "http://api.test");

        Assert.Equal("http://api.test/uploads/a.png", url);
    }

    [Theory]
    [InlineData("http://cdn.test/a.png")]
    [InlineData("https://cdn.test/a.png")]
    public void Resolve_AbsoluteWebUrl_ReturnsItUnchanged(string fileUrl)
    {
        Assert.Equal(fileUrl, ImageUrls.Resolve(fileUrl, "http://api.test/"));
    }

    [Fact]
    public void Resolve_FileNameWithSpaces_EncodesEachSegment()
    {
        var url = ImageUrls.Resolve("/uploads/mi lámina.png", "http://api.test/");

        Assert.Equal("http://api.test/uploads/mi%20l%C3%A1mina.png", url);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Resolve_MissingApiBaseUrl_UsesDefault(string? apiBaseUrl)
    {
        var url = ImageUrls.Resolve("/uploads/a.png", apiBaseUrl);

        Assert.Equal("http://localhost:5138/uploads/a.png", url);
    }
}
