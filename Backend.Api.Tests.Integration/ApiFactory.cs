using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using UCR.EB.BioMicroscopeAdmin.Backend.Presentation.Security;

namespace UCR.EB.BioMicroscopeAdmin.Backend.Api.Tests.Integration;

/// <summary>Runs the real API against a disposable PostgreSQL container.</summary>
public sealed class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string AdminEmail = "admin@test.local";
    public const string ReaderEmail = "lector@test.local";
    public static readonly string TestPassword = $"T3st{Guid.NewGuid():N}";

    private readonly PostgreSqlContainer _database = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .Build();

    public Task InitializeAsync() => _database.StartAsync();

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _database.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", _database.GetConnectionString());
        builder.UseSetting("DisableHttpsRedirect", "true");
        builder.UseSetting("Auth:AdminEmail", AdminEmail);
        builder.UseSetting("Auth:AdminPassword", TestPassword);
        builder.UseSetting("Auth:ReaderEmail", ReaderEmail);
        builder.UseSetting("Auth:ReaderPassword", TestPassword);
    }

    public HttpClient CreateAnonymousClient()
    {
        var client = CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        client.DefaultRequestHeaders.Add(CsrfHeaderFilter.HeaderName, CsrfHeaderFilter.HeaderValue);
        return client;
    }

    public async Task<HttpClient> CreateClientForAsync(string email)
    {
        var client = CreateAnonymousClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password = TestPassword });
        response.EnsureSuccessStatusCode();
        return client;
    }
}
