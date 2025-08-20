using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TinyphoneCSharpClient.Configuration;
using TinyphoneCSharpClient.Models;
using TinyphoneCSharpClient.Services;

// Create host builder
var builder = Host.CreateDefaultBuilder(args);

// Configure services
builder.ConfigureServices((context, services) =>
{
    // Bind configuration
    var tinyphoneSettings = new TinyphoneSettings();
    context.Configuration.GetSection(TinyphoneSettings.SectionName).Bind(tinyphoneSettings);
    services.AddSingleton(tinyphoneSettings);

    // Configure HttpClientFactory
    services.AddHttpClient();

    // Register services
    services.AddScoped<ITinyphoneService, TinyphoneService>();
});

// Build host
var host = builder.Build();

// Get services
using var scope = host.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var tinyphoneService = scope.ServiceProvider.GetRequiredService<ITinyphoneService>();

logger.LogInformation("Tinyphone C# Client started");

// Log configuration for debugging
var settings = scope.ServiceProvider.GetRequiredService<TinyphoneSettings>();
logger.LogInformation("Using Base URL: {BaseUrl}", settings.BaseUrl);
logger.LogInformation("Timeout: {TimeoutSeconds} seconds", settings.TimeoutSeconds);

// Create cancellation token source with timeout and console cancellation
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

// Allow cancellation via Ctrl+C
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // Don't terminate immediately
    cts.Cancel(); // Signal cancellation
    logger.LogInformation("Cancellation requested by user");
};

try
{
    // Demonstrate API calls
    await DemonstrateApiCalls(tinyphoneService, logger, cts.Token);
}
catch (OperationCanceledException)
{
    logger.LogWarning("Operation was cancelled due to timeout or user request");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred during execution");
}

logger.LogInformation("Program execution completed, press any key to exit...");
Console.ReadKey();

/// <summary>
/// Demonstrate API calls
/// </summary>
/// <param name="service">Tinyphone service</param>
/// <param name="logger">Logger</param>
/// <param name="cancellationToken">Cancellation token</param>
static async Task DemonstrateApiCalls(ITinyphoneService service, ILogger logger, CancellationToken cancellationToken)
{
    try
    {
        // 1. Get version information
        logger.LogInformation("=== Get Version Information ===");
        var version = await service.GetVersionAsync(cancellationToken);
        if (version != null)
        {
            logger.LogInformation("Application version: {Message} - {Version}", version.Message, version.Version);
        }
        else
        {
            logger.LogWarning("Unable to get version information");
        }

        // 2. Get accounts list
        logger.LogInformation("\n=== Get Accounts List ===");
        var accounts = await service.GetAccountsAsync(cancellationToken);
        if (accounts != null && accounts.Any())
        {
            foreach (var account in accounts)
            {
                logger.LogInformation("Account: {AccountName} - {Username}@{Domain} [{Status}]", account.AccountName, account.Username, account.Domain, account.Status);
            }
        }
        else
        {
            logger.LogInformation("No registered accounts currently");
        }

        // 3. Get calls list
        logger.LogInformation("\n=== Get Calls List ===");
        var calls = await service.GetCallsAsync(cancellationToken);
        if (calls != null && calls.Any())
        {
            foreach (var call in calls)
            {
                logger.LogInformation("Call: {CallId} - {RemoteUri} [{State}] ({Duration}s)", call.CallId, call.RemoteUri, call.State, call.Duration);
            }
        }
        else
        {
            logger.LogInformation("No active calls currently");
        }

        // 4. Demonstrate login (using sample data)
        logger.LogInformation("\n=== Demonstrate Login ===");
        var loginRequest = new LoginRequest
        {
            Username = "testuser",
            Password = "testpass",
            Domain = "example.com"
        };

        var loginResult = await service.LoginAsync(loginRequest, cancellationToken);
        logger.LogInformation("Login result: {Message}", loginResult.Message);

        // 5. Demonstrate dialing (using sample data)
        logger.LogInformation("\n=== Demonstrate Dialing ===");
        var dialRequest = new DialRequest
        {
            Uri = "sip:test@example.com",
            Account = "testuser"
        };

        var dialResult = await service.DialAsync(dialRequest, cancellationToken);
        logger.LogInformation("Dial result: {Message}", dialResult.Message);

    }
    catch (HttpRequestException ex)
    {
        logger.LogWarning("Network connection error: {Message}", ex.Message);
        logger.LogInformation("Please ensure Tinyphone application is running on http://localhost:6060");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while executing API calls");
    }
}
