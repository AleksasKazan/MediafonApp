using MediafonApp.Models;
using MediafonApp.Repositories;
using MediafonApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediafonApp
{
    class Program
    {
        // Method serves as the entry point for the application. It creates the host and starts it.
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        // Method configures the host and sets up the services
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => //various services are added to the service collection
                {
                    // Add configuration
                    IConfiguration configuration = hostContext.Configuration;
                    services.AddSingleton(configuration);

                    // Section from the configuration is bound to the SftpConfiguration class
                    services.Configure<SftpConfiguration>(configuration.GetSection("SftpConfiguration")); 

                    // The database context is added using the provided connection string
                    string connectionString = configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContextFactory<MediafonDbContext>(options =>
                    {
                        options.UseNpgsql(connectionString);
                    });

                    // Is added as a hosted service
                    services.AddHostedService<SftpFileCheckService>();

                    // Interface is registered with its implementation
                    services.AddScoped<IFileRecordRepository, FileRecordRepository>();

                    // Console logging provider is added to the logging builder
                    services.AddLogging(builder =>
                    {
                        builder.AddConsole();
                    });
                });
    }
}