using FargateDemo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

await Host.CreateDefaultBuilder()
    .UseEnvironment(environmentName)
    .ConfigureAppConfiguration((_, builder) =>
    {
        builder.AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json");
        builder.Build();
    })
    .ConfigureServices((_, services) =>
    {
        services.AddHostedService<ConsoleHostedService>();
        
        services.AddSingleton<ILogger>(new LoggerConfiguration().WriteTo.Console().CreateLogger());
    })
    .RunConsoleAsync();