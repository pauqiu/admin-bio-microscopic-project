namespace UCR.EB.BioMicroscopeAdmin.Frontend.Presentation.Components;

/// <summary>Builds the browser address of an uploaded image.</summary>
public static class ImageUrls
{
    public const string DefaultApiBaseUrl = "http://localhost:5138/";

    public static string Resolve(string fileUrl, string? apiBaseUrl)
    {
        // On Unix runtimes (incl. WASM) "/uploads/x.png" parses as an absolute file:// URI,
        // so only treat real web URLs as already-absolute.
        if (Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
            return fileUrl;
        }

        var encodedPath = string.Join('/', fileUrl.Split('/').Select(Uri.EscapeDataString));
        var baseUrl = (string.IsNullOrWhiteSpace(apiBaseUrl) ? DefaultApiBaseUrl : apiBaseUrl).TrimEnd('/');
        var separator = encodedPath.StartsWith('/') ? string.Empty : "/";
        return $"{baseUrl}{separator}{encodedPath}";
    }
}
