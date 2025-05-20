using BankContracts.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BankTests.Infrastructure;

internal class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var databaseConfig = services.SingleOrDefault(x => x.ServiceType == typeof(IConfigurationDatabase));
            if (databaseConfig is not null)
                services.Remove(databaseConfig);

            var loggerFactory = services.SingleOrDefault(x => x.ServiceType == typeof(LoggerFactory));
            if (loggerFactory is not null)
                services.Remove(loggerFactory);

            services.AddSingleton<IConfigurationDatabase, ConfigurationDatabase>();
        });

        builder.UseEnvironment("Development");

        base.ConfigureWebHost(builder);
    }
}
