using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Serilog;

namespace FargateDemo;

internal sealed class ConsoleHostedService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly IHostApplicationLifetime _appLifetime;

    public ConsoleHostedService(
        ILogger logger,
        IConfiguration configuration,
        IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _configuration = configuration;
        _appLifetime = appLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Debug("Starting");

        _appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                try
                {
                    await RunConsoleApp(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex, "Unhandled exception!");
                }
                finally
                {
                    _appLifetime.StopApplication();
                }
            }, cancellationToken);
        });

        return Task.CompletedTask;
    }

    private async Task RunConsoleApp(CancellationToken cancellationToken)
    {
        var connectionString = _configuration["mongo:connectionString"];
        var mongoUrl = new MongoUrl(connectionString);

        var settings = MongoClientSettings.FromUrl(mongoUrl);

        settings.Credential = new MongoCredential(mongoUrl.AuthenticationMechanism, 
            new MongoInternalIdentity(mongoUrl.DatabaseName, Environment.GetEnvironmentVariable("MONGO_USERNAME")), 
            new PasswordEvidence(Environment.GetEnvironmentVariable("MONGO_PASSWORD")));
        
        var mongoClient = new MongoClient(settings);
        var db = mongoClient.GetDatabase("fargateDemoDb");

        var collection = db.GetCollection<User>("users");

        var allUsers = await (await collection.FindAsync(x => true, 
            cancellationToken: cancellationToken)).ToListAsync(cancellationToken);

        var usernames = string.Join(",", allUsers.Select(x => x.Name));
        _logger.Information("Users: {Usernames}", usernames);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}