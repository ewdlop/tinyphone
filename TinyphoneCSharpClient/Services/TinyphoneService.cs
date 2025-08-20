using System.Text;
using System.Text.Json;
using TinyphoneCSharpClient.Configuration;
using TinyphoneCSharpClient.Models;

namespace TinyphoneCSharpClient.Services;

/// <summary>
/// Tinyphone REST API service implementation
/// </summary>
public class TinyphoneService : ITinyphoneService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TinyphoneSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions;

    public TinyphoneService(IHttpClientFactory httpClientFactory, TinyphoneSettings settings)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    private HttpClient CreateHttpClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_settings.BaseUrl);
        client.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);
        client.DefaultRequestHeaders.Add("User-Agent", "TinyphoneCSharpClient/1.0");
        return client;
    }

    public async Task<AppVersionResponse?> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.GetAsync("/", cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                // Log the raw response for debugging
                Console.WriteLine($"Raw version response: {content}");
                
                // Handle simple string responses or JSON objects
                if (content.StartsWith('"') && content.EndsWith('"'))
                {
                    // Simple string response
                    var message = JsonSerializer.Deserialize<string>(content, _jsonOptions);
                    return new AppVersionResponse { Message = message ?? "", Version = "Unknown" };
                }
                
                try
                {
                    return JsonSerializer.Deserialize<AppVersionResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    // If JSON parsing fails, treat as plain text
                    return new AppVersionResponse { Message = content, Version = "Unknown" };
                }
            }
            
            Console.WriteLine($"Failed version response ({response.StatusCode}): {content}");
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error getting version information: {ex.Message}", ex);
        }
    }

    public async Task<ApiResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync("/login", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = "Login successful" };
            }
            
            return new ApiResponse { Success = false, Message = "Login failed", Error = responseContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during login", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> LogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync("/logout", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = "Logout successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Logout failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during logout", Error = ex.Message };
        }
    }

    public async Task<List<Account>?> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.GetAsync("/accounts", cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                // Log the raw response for debugging
                Console.WriteLine($"Raw accounts response: {content}");
                
                // Try to handle different response formats
                if (string.IsNullOrWhiteSpace(content) || content == "null")
                {
                    return new List<Account>();
                }
                
                try
                {
                    // Try the actual API format first
                    var accountsResponse = JsonSerializer.Deserialize<AccountsResponse>(content, _jsonOptions);
                    return accountsResponse?.Accounts ?? new List<Account>();
                }
                catch (JsonException)
                {

                }
            }
            
            Console.WriteLine($"Failed response ({response.StatusCode}): {content}");
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error getting accounts list: {ex.Message}", ex);
        }
    }

    public async Task<ApiResponse> LogoutAccountAsync(string accountName, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/accounts/{accountName}/logout", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Account {accountName} logout successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = $"Account {accountName} logout failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = $"Error during account {accountName} logout", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> DialAsync(DialRequest dialRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var json = JsonSerializer.Serialize(dialRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync("/dial", content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Dial {dialRequest.Uri} successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Dial failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during dial", Error = ex.Message };
        }
    }

    public async Task<List<Call>?> GetCallsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.GetAsync("/calls", cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                // Log the raw response for debugging
                Console.WriteLine($"Raw calls response: {content}");
                
                // Try to handle different response formats
                if (string.IsNullOrWhiteSpace(content) || content == "null")
                {
                    return new List<Call>();
                }
                
                try
                {
                    // Try the actual API format first (assuming similar to accounts)
                    var callsResponse = JsonSerializer.Deserialize<CallsResponse>(content, _jsonOptions);
                    return callsResponse?.Calls ?? new List<Call>();
                }
                catch (JsonException)
                {
                   
                }
            }
            
            Console.WriteLine($"Failed calls response ({response.StatusCode}): {content}");
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error getting calls list: {ex.Message}", ex);
        }
    }

    public async Task<ApiResponse> AnswerCallAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/calls/{callId}/answer", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Answer call {callId} successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Answer call failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during answer call", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> SendDtmfAsync(int callId, string digits, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/calls/{callId}/dtmf/{digits}", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Send DTMF {digits} to call {callId} successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Send DTMF failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during send DTMF", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> HoldCallAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PutAsync($"/calls/{callId}/hold", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Hold call {callId} successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Hold call failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during hold call", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> UnholdCallAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.DeleteAsync($"/calls/{callId}/hold", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Unhold call {callId} successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Unhold call failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during unhold call", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> CreateConferenceAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PutAsync($"/calls/{callId}/conference", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Create conference with call {callId} successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Create conference failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during create conference", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> BreakConferenceAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.DeleteAsync($"/calls/{callId}/conference", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Break call {callId} out of conference successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Break conference failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during break conference", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> TransferCallAsync(int callId, TransferRequest transferRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var json = JsonSerializer.Serialize(transferRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync($"/calls/{callId}/transfer", content, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Transfer call {callId} to {transferRequest.Uri} successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Transfer call failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during transfer call", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> AttendedTransferAsync(int callId, int destCallId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/calls/{callId}/attended-transfer/{destCallId}", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Attended transfer call {callId} to {destCallId} successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Attended transfer failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during attended transfer", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> HangupCallAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/calls/{callId}/hangup", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Hangup call {callId} successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Hangup call failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during hangup call", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> HangupAllCallsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync("/hangup_all", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = "Hangup all calls successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Hangup all calls failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during hangup all calls", Error = ex.Message };
        }
    }

    public async Task<ApiResponse> ExitApplicationAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync("/exit", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = "Exit application successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = "Exit application failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = "Error during exit application", Error = ex.Message };
        }
    }

    public async Task<DevicesResponse?> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.GetAsync("/devices", cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Raw devices response: {content}");
                
                if (string.IsNullOrWhiteSpace(content) || content == "null")
                {
                    return new DevicesResponse { Message = "No devices", Count = 0, Devices = new List<AudioDevice>() };
                }
                
                try
                {
                    return JsonSerializer.Deserialize<DevicesResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new DevicesResponse { Message = "Failed to parse devices response", Count = 0, Devices = new List<AudioDevice>() };
                }
            }
            
            Console.WriteLine($"Failed devices response ({response.StatusCode}): {content}");
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error getting devices list: {ex.Message}", ex);
        }
    }

    public async Task<ConfigResponse?> GetConfigAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.GetAsync("/config", cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Raw config response: {content}");
                
                if (string.IsNullOrWhiteSpace(content) || content == "null")
                {
                    return new ConfigResponse { Version = "Unknown" };
                }
                
                try
                {
                    return JsonSerializer.Deserialize<ConfigResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new ConfigResponse { Version = "Failed to parse" };
                }
            }
            
            Console.WriteLine($"Failed config response ({response.StatusCode}): {content}");
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error getting configuration: {ex.Message}", ex);
        }
    }

    public async Task<ApiResponse> ReregisterAccountAsync(string accountName, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/accounts/{accountName}/reregister", null, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse { Success = true, Message = $"Account {accountName} re-registration successful" };
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse { Success = false, Message = $"Account {accountName} re-registration failed", Error = errorContent };
        }
        catch (Exception ex)
        {
            return new ApiResponse { Success = false, Message = $"Error during account {accountName} re-registration", Error = ex.Message };
        }
    }
}
