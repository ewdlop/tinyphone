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
            logger.LogInformation("Found {Count} accounts:", accounts.Count);
            foreach (var account in accounts)
            {
                logger.LogInformation("  - ID: {Id}, Name: {Name}, URI: {Uri}", 
                    account.Id, account.Name, account.Uri);
                logger.LogInformation("    Active: {Active}, Status: {Status}", 
                    account.Active, account.Status);
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
            logger.LogInformation("Found {Count} active calls:", calls.Count);
            foreach (var call in calls)
            {
                logger.LogInformation("  - ID: {Id}, Party: {Party}, State: {State}", 
                    call.Id, call.Party, call.State);
                logger.LogInformation("    Direction: {Direction}, Duration: {Duration}s, Hold: {Hold}", 
                    call.Direction, call.Duration, call.Hold);
                logger.LogInformation("    Caller ID: {CallerId}, Display Name: {DisplayName}", 
                    call.CallerId, call.DisplayName);
            }
        }
        else
        {
            logger.LogInformation("No active calls currently");
        }

        // 4. Get configuration
        logger.LogInformation("\n=== Get Configuration ===");
        var config = await service.GetConfigAsync(cancellationToken);
        if (config != null)
        {
            logger.LogInformation("Configuration - Version: {Version}", config.Version);
            logger.LogInformation("SIP Log File: {SipLogFile}", config.SipLogFile);
            logger.LogInformation("HTTP Log File: {HttpLogFile}", config.HttpLogFile);
        }
        else
        {
            logger.LogWarning("Unable to get configuration information");
        }

        // 5. Get audio devices
        logger.LogInformation("\n=== Get Audio Devices ===");
        var devices = await service.GetDevicesAsync(cancellationToken);
        if (devices != null && devices.Devices.Any())
        {
            logger.LogInformation("Found {Count} audio devices:", devices.Count);
            foreach (var device in devices.Devices)
            {
                logger.LogInformation("  - ID: {Id}, Name: {Name}, Driver: {Driver}", 
                    device.Id, device.Name, device.Driver);
                logger.LogInformation("    Input: {InputCount}, Output: {OutputCount}", 
                    device.InputCount, device.OutputCount);
                if (!string.IsNullOrEmpty(device.PaApi))
                    logger.LogInformation("    PA API: {PaApi}", device.PaApi);
            }
        }
        else
        {
            logger.LogInformation("No audio devices found or unable to retrieve devices");
        }

        // 6. Demonstrate login (using sample data - will likely fail)
        logger.LogInformation("\n=== Demonstrate Login (Sample Data) ===");
        logger.LogInformation("Note: This will likely fail as it uses sample credentials");
        var loginRequest = new LoginRequest
        {
            Username = "testuser",
            Password = "testpass",
            Domain = "example.com"
        };

        var loginResult = await service.LoginAsync(loginRequest, cancellationToken);
        logger.LogInformation("Login result: Success={Success}, Message={Message}", 
            loginResult.Success, loginResult.Message);

        // 7. Demonstrate dialing (using sample data - will likely fail)
        logger.LogInformation("\n=== Demonstrate Dialing (Sample Data) ===");
        logger.LogInformation("Note: This will likely fail as no valid account is logged in");
        var dialRequest = new DialRequest
        {
            Uri = "sip:test@example.com",
            Account = "testuser"
        };

        var dialResult = await service.DialAsync(dialRequest, cancellationToken);
        logger.LogInformation("Dial result: Success={Success}, Message={Message}", 
            dialResult.Success, dialResult.Message);
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
