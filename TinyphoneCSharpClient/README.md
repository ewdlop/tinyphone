# Tinyphone C# Client

A modern C# client library for communicating with the Tinyphone softphone server.

## Overview

This project provides a complete C# client implementation for interacting with the Tinyphone softphone server running on port 6060. It supports all available API endpoints including account management, call control, audio device management, and more.

## Features

- ✅ **Complete API Coverage** - Supports all Tinyphone REST API endpoints
- ✅ **Modern Async Design** - Uses async/await patterns with CancellationToken support
- ✅ **Strongly Typed DTOs** - Complete data transfer object models
- ✅ **Error Handling** - Comprehensive exception handling and network error recovery
- ✅ **Dependency Injection Support** - Fully compatible with .NET DI containers
- ✅ **Flexible Configuration** - Supports appsettings.json configuration
- ✅ **Detailed Logging** - Uses Microsoft.Extensions.Logging

## Quick Start

### 1. Configuration

Edit the `appsettings.json` file:

```json
{
  "TinyphoneSettings": {
    "BaseUrl": "http://localhost:6060",
    "TimeoutSeconds": 30
  }
}
```

### 2. Dependency Injection Setup

```csharp
// Register services
services.AddHttpClient();
services.Configure<TinyphoneSettings>(
    configuration.GetSection(TinyphoneSettings.SectionName));
services.AddScoped<ITinyphoneService, TinyphoneService>();
```

### 3. Basic Usage

```csharp
var tinyphoneService = serviceProvider.GetRequiredService<ITinyphoneService>();

// Get version information
var version = await tinyphoneService.GetVersionAsync();
Console.WriteLine($"Tinyphone Version: {version?.Version}");

// Get accounts list
var accounts = await tinyphoneService.GetAccountsAsync();
Console.WriteLine($"Found {accounts?.Count ?? 0} accounts");
```

## API Endpoints

### System Information

| Endpoint | Method | C# Method | Description |
|----------|--------|-----------|-------------|
| `/` | GET | `GetVersionAsync()` | Get application version information |
| `/config` | GET | `GetConfigAsync()` | Get application configuration |
| `/devices` | GET | `GetDevicesAsync()` | Get audio devices list |

### Account Management

| Endpoint | Method | C# Method | Description |
|----------|--------|-----------|-------------|
| `/login` | POST | `LoginAsync(LoginRequest)` | Login account with provided details |
| `/logout` | POST | `LogoutAsync()` | Logout all accounts |
| `/accounts` | GET | `GetAccountsAsync()` | Get registered accounts list |
| `/accounts/{name}/logout` | POST | `LogoutAccountAsync(string)` | Logout specific account |
| `/accounts/{name}/reregister` | POST | `ReregisterAccountAsync(string)` | Re-register specific account |

### Call Management

| Endpoint | Method | C# Method | Description |
|----------|--------|-----------|-------------|
| `/dial` | POST | `DialAsync(DialRequest)` | Dial a call with specified SIP URI |
| `/calls` | GET | `GetCallsAsync()` | Get active calls list |
| `/calls/{id}/answer` | POST | `AnswerCallAsync(int)` | Answer specified call |
| `/calls/{id}/dtmf/{digits}` | POST | `SendDtmfAsync(int, string)` | Send DTMF digits |
| `/calls/{id}/hold` | PUT | `HoldCallAsync(int)` | Hold call |
| `/calls/{id}/hold` | DELETE | `UnholdCallAsync(int)` | Unhold call |
| `/calls/{id}/conference` | PUT | `CreateConferenceAsync(int)` | Create conference |
| `/calls/{id}/conference` | DELETE | `BreakConferenceAsync(int)` | Break conference |
| `/calls/{id}/transfer` | POST | `TransferCallAsync(int, TransferRequest)` | Transfer call |
| `/calls/{id}/attended-transfer/{destId}` | POST | `AttendedTransferAsync(int, int)` | Attended transfer |
| `/calls/{id}/hangup` | POST | `HangupCallAsync(int)` | Hangup specified call |
| `/hangup_all` | POST | `HangupAllCallsAsync()` | Hangup all calls |

### System Control

| Endpoint | Method | C# Method | Description |
|----------|--------|-----------|-------------|
| `/exit` | POST | `ExitApplicationAsync()` | Exit the application |

## Usage Examples

### Account Login

```csharp
var loginRequest = new LoginRequest
{
    Username = "your-username",
    Password = "your-password",
    Domain = "sip-provider.com",
    Proxy = "proxy-server.com"  // Optional
};

var result = await tinyphoneService.LoginAsync(loginRequest);
if (result.Success)
{
    Console.WriteLine("Login successful!");
}
else
{
    Console.WriteLine($"Login failed: {result.Message}");
}
```

### Making a Call

```csharp
var dialRequest = new DialRequest
{
    Uri = "sip:1234@domain.com",
    Account = "account-name"  // Optional, use specific account
};

var result = await tinyphoneService.DialAsync(dialRequest);
if (result.Success)
{
    Console.WriteLine("Dial successful!");
}
```

### Call Operations

```csharp
// Get calls list
var calls = await tinyphoneService.GetCallsAsync();

if (calls?.Any() == true)
{
    var call = calls.First();
    int callId = call.Id;
    
    // Answer call
    await tinyphoneService.AnswerCallAsync(callId);
    
    // Hold call
    await tinyphoneService.HoldCallAsync(callId);
    
    // Send DTMF
    await tinyphoneService.SendDtmfAsync(callId, "123");
    
    // Unhold call
    await tinyphoneService.UnholdCallAsync(callId);
    
    // Hangup call
    await tinyphoneService.HangupCallAsync(callId);
}
```

### Call Transfer

```csharp
// Blind transfer
var transferRequest = new TransferRequest
{
    Uri = "sip:5678@domain.com"
};
await tinyphoneService.TransferCallAsync(callId, transferRequest);

// Attended transfer (between two calls)
await tinyphoneService.AttendedTransferAsync(sourceCallId, destCallId);
```

### Conference Calls

```csharp
// Add call to conference
await tinyphoneService.CreateConferenceAsync(callId);

// Remove call from conference
await tinyphoneService.BreakConferenceAsync(callId);
```

### Devices and Configuration

```csharp
// Get audio devices
var devices = await tinyphoneService.GetDevicesAsync();
if (devices != null)
{
    Console.WriteLine($"Found {devices.Count} audio devices:");
    foreach (var device in devices.Devices)
    {
        Console.WriteLine($"- {device.Name} ({device.Driver})");
        Console.WriteLine($"  Input: {device.InputCount}, Output: {device.OutputCount}");
    }
}

// Get configuration
var config = await tinyphoneService.GetConfigAsync();
if (config != null)
{
    Console.WriteLine($"App Version: {config.Version}");
    Console.WriteLine($"SIP Log: {config.SipLogFile}");
}
```

## Data Models

### Request Models

- **LoginRequest**: Login request (`username`, `password`, `domain`, `proxy?`, `login?`)
- **DialRequest**: Dial request (`uri`, `account?`)
- **TransferRequest**: Transfer request (`uri`)

### Response Models

- **Account**: Account information (`id`, `uri`, `name`, `active`, `status`)
- **Call**: Call information (`id`, `account`, `party`, `state`, `direction`, `duration`, etc.)
- **AudioDevice**: Audio device (`id`, `name`, `driver`, `inputCount`, `outputCount`)
- **ApiResponse**: Generic API response (`success`, `message`, `error?`)

## Error Handling

```csharp
try
{
    var result = await tinyphoneService.LoginAsync(loginRequest);
    // Handle successful result
}
catch (HttpRequestException ex)
{
    // Network connection error
    Console.WriteLine($"Network error: {ex.Message}");
    Console.WriteLine("Please ensure Tinyphone application is running on http://localhost:6060");
}
catch (OperationCanceledException)
{
    // Operation canceled (timeout or user cancellation)
    Console.WriteLine("Operation timed out or was canceled");
}
catch (Exception ex)
{
    // Other errors
    Console.WriteLine($"Unknown error: {ex.Message}");
}
```

## Configuration Options

### TinyphoneSettings

```csharp
public class TinyphoneSettings
{
    public string BaseUrl { get; set; } = "http://localhost:6060";
    public int TimeoutSeconds { get; set; } = 30;
}
```

## Requirements

- **.NET 6.0** or higher
- **Tinyphone Application** running on configured URL (default `http://localhost:6060`)

## Development and Testing

1. **Start Tinyphone Application**
2. **Run the client**:
   ```bash
   dotnet run --project TinyphoneCSharpClient
   ```
3. **Check log output** to confirm connection and functionality

## Notes

- All async methods support `CancellationToken` for timeout and cancellation handling
- Network errors throw `HttpRequestException`
- Some operations (dial, login) may require valid SIP account configuration
- Consider proper error handling and retry logic in production environments

## WebSocket Events

The Tinyphone server also exposes a WebSocket endpoint at `/events` for real-time events. While this C# client focuses on REST API interactions, you can implement WebSocket client functionality separately if needed.

## Request/Response Payload Examples

### Login Payload
```json
{
  "username": "string",
  "login": "optional-string",
"password": "string",
"domain": "string",
  "proxy": "optional-string"
}
```

### Dial Payload
```json
{
"uri": "sip-uri",
  "account": "account_name"
}
```

### Transfer Payload
```json
{
  "uri": "sip-uri"
}
```

## License

This project uses the same license as the main Tinyphone project.