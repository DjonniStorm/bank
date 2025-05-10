using BankDatabase;
using BankTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text;
using System.Text.Json;

namespace BankTests.WebApiControllersTests;

internal class BaseWebApiControllerTest
{
    private WebApplicationFactory<Program> _webApplication;

    protected HttpClient HttpClient { get; private set; }

    protected BankDbContext BankDbContext { get; private set; }

    protected static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _webApplication = new CustomWebApplicationFactory<Program>();
        HttpClient = _webApplication
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    using var loggerFactory = new LoggerFactory();
                    loggerFactory.AddSerilog(new LoggerConfiguration()
                        .ReadFrom.Configuration(new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build())
                        .CreateLogger());
                    services.AddSingleton(loggerFactory);
                });
            })
            .CreateClient();

        var request = HttpClient.GetAsync("/login/user").GetAwaiter().GetResult();
        var data = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {data}");

        BankDbContext = _webApplication.Services.GetRequiredService<BankDbContext>();
        BankDbContext.Database.EnsureDeleted();
        BankDbContext.Database.EnsureCreated();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        BankDbContext?.Database.EnsureDeleted();
        BankDbContext?.Dispose();
        HttpClient?.Dispose();
        _webApplication?.Dispose();
    }

    protected static async Task<T?> GetModelFromResponseAsync<T>(HttpResponseMessage response) =>
        JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), JsonSerializerOptions);

    protected static StringContent MakeContent(object model) =>
        new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
}
