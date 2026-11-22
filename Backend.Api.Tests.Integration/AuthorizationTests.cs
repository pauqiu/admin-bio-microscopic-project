using System.Net;
using System.Net.Http.Json;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Endpoints;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Api.Tests.Integration;

public sealed class AuthorizationTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private const string AnyId = "1";

    public static TheoryData<string> ReadRoutes => new()
    {
        "/api/groups", $"/api/groups/{AnyId}", $"/api/groups/{AnyId}/boxes",
        "/api/boxes", $"/api/boxes/{AnyId}", $"/api/boxes/{AnyId}/slides",
        "/api/slides", $"/api/slides/{AnyId}", $"/api/slides/{AnyId}/images",
        "/api/auth/me"
    };

    public static TheoryData<string, string> WriteRoutes => new()
    {
        { "POST", "/api/groups" }, { "PUT", $"/api/groups/{AnyId}" }, { "DELETE", $"/api/groups/{AnyId}" },
        { "POST", "/api/boxes" }, { "PUT", $"/api/boxes/{AnyId}" }, { "DELETE", $"/api/boxes/{AnyId}" },
        { "POST", "/api/slides" }, { "PUT", $"/api/slides/{AnyId}" }, { "DELETE", $"/api/slides/{AnyId}" },
        { "POST", $"/api/slides/{AnyId}/images" }, { "PUT", $"/api/images/{AnyId}" }, { "DELETE", $"/api/images/{AnyId}" }
    };

    [Theory]
    [MemberData(nameof(ReadRoutes))]
    public async Task Read_WithoutSession_Returns401(string route)
    {
        var response = await factory.CreateAnonymousClient().GetAsync(route);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(WriteRoutes))]
    public async Task Write_WithoutSession_Returns401(string method, string route)
    {
        var response = await factory.CreateAnonymousClient().SendAsync(new HttpRequestMessage(new HttpMethod(method), route));
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(WriteRoutes))]
    public async Task Write_AsReader_Returns403(string method, string route)
    {
        var client = await factory.CreateClientForAsync(ApiFactory.ReaderEmail);
        var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod(method), route));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Read_AsReader_Returns200()
    {
        var client = await factory.CreateClientForAsync(ApiFactory.ReaderEmail);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/groups")).StatusCode);
    }

    [Fact]
    public async Task Administrator_CanCreateReadAndDeleteGroup()
    {
        var client = await factory.CreateClientForAsync(ApiFactory.AdminEmail);

        var created = await client.PostAsJsonAsync("/api/groups", new { name = $"Grupo {Guid.NewGuid():N}"[..20] });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var id = (await created.Content.ReadFromJsonAsync<IdResponse>())!.Id;

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/groups/{id}")).StatusCode);
        Assert.True((await client.DeleteAsync($"/api/groups/{id}")).IsSuccessStatusCode);
    }

    [Fact]
    public async Task Write_WithoutCsrfHeader_Returns403()
    {
        var client = await factory.CreateClientForAsync(ApiFactory.AdminEmail);
        client.DefaultRequestHeaders.Remove("X-Requested-With");

        var response = await client.PostAsJsonAsync("/api/groups", new { name = "Sin header" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var response = await factory.CreateAnonymousClient().PostAsJsonAsync(
            "/api/auth/login", new { email = "nadie@test.local", password = "incorrecta" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_ReturnsRole_AndLogoutEndsSession()
    {
        var client = await factory.CreateClientForAsync(ApiFactory.ReaderEmail);

        var me = await client.GetFromJsonAsync<CurrentUserResponse>("/api/auth/me");
        Assert.Equal(ApiFactory.ReaderEmail, me!.Email);
        Assert.Equal(["Lector"], me.Roles);

        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync("/api/auth/logout", null)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/auth/me")).StatusCode);
    }

    [Fact]
    public async Task Health_IsAnonymous()
    {
        var response = await factory.CreateAnonymousClient().GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Swagger_IsNotExposedOutsideDevelopment()
    {
        var response = await factory.CreateAnonymousClient().GetAsync("/swagger/v1/swagger.json");
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("openapi", body, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record IdResponse(int Id);
}
