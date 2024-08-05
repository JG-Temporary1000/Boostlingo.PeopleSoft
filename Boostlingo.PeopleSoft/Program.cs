using Boostlingo.PeopleSoft;
using Boostlingo.PeopleSoft.Business.Helpers;
using Boostlingo.PeopleSoft.Business.Models;
using Boostlingo.PeopleSoft.Business.Services;
using Boostlingo.PeopleSoft.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // If needed, create the initial tables
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Configure strongly typed app settings
                services.Configure<AppSettings>(context.Configuration.GetSection("AppSettings"));

                // Register AppSettings to be resolved via IOptions
                services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSettings>>().Value);

                services.AddDbContext<ApplicationDbContext>(options => 
                    options.UseSqlServer(context.Configuration.GetConnectionString("DBConnection")));

                // Register services
                services.AddScoped<IDataService, DataService>();
                services.AddHttpClient<IApiService, ApiService>(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddPolicyHandler(HttpClientRetryPolicy.GetRetryPolicy()) // Retry endpoints with element of randomness
                .AddPolicyHandler(HttpCircuitBreakerPolicy.GetCircuitBreakerPolicy()); // Eventually stop trying if fault not transient

                services.AddScoped<IConsoleService, ConsoleService>();
                services.AddScoped<ICultureService, CultureService>();
                services.AddScoped<IExceptionService, ExceptionService>();
                services.AddTransient<App>();

                // Register the hosted service
                services.AddHostedService<AppHost>();
            })
            .UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

    public class AppHost : IHostedService
    {
        private readonly App _app;
        public AppHost(App app)
        {
            _app = app;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _app.Run();
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
