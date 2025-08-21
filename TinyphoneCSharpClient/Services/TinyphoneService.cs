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

    public async Task<LoginResponse?> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync("/login", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            var responseObj = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<LoginResponse>(responseObj, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new LoginResponse { Message = "Login successful", Result = 200 };
                }
            }
            
            try
            {
                var errorResponse = JsonSerializer.Deserialize<LoginResponse>(responseObj, _jsonOptions);
                return errorResponse;
            }
            catch (JsonException)
            {
                return new LoginResponse { Message = "Login failed", Result = (int)response.StatusCode };
            }
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during login: {ex.Message}", ex);
        }
    }

    public async Task<LogoutResponse?> LogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync("/logout", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<LogoutResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new LogoutResponse { Message = "Logout successful", Result = 200 };
                }
            }
            
            return new LogoutResponse { Message = "Logout failed", Result = (int)response.StatusCode };
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during logout: {ex.Message}", ex);
        }
    }

    public async Task<AccountsResponse?> GetAccountsAsync(CancellationToken cancellationToken = default)
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
                    return new AccountsResponse { Message = "No accounts", Accounts = new List<Account>() };
                }
                
                try
                {
                    return JsonSerializer.Deserialize<AccountsResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new AccountsResponse { Message = "Failed to parse accounts", Accounts = new List<Account>() };
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

    public async Task<AccountLogoutResponse?> LogoutAccountAsync(string accountName, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/accounts/{accountName}/logout", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<AccountLogoutResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new AccountLogoutResponse { Message = "Logged Out", AccountName = accountName, Result = 200 };
                }
            }
            
            try
            {
                return JsonSerializer.Deserialize<AccountLogoutResponse>(content, _jsonOptions);
            }
            catch (JsonException)
            {
                return new AccountLogoutResponse { Message = "Account logout failed", AccountName = accountName, Result = (int)response.StatusCode };
            }
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during account {accountName} logout: {ex.Message}", ex);
        }
    }

    public async Task<DialResponse?> DialAsync(DialRequest dialRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var json = JsonSerializer.Serialize(dialRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync("/dial", content, cancellationToken);
            
            var reponseString = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<DialResponse>(reponseString, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new DialResponse { Message = "Dialling", Party = dialRequest.Uri };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during dial: {ex.Message}", ex);
        }
    }

    public async Task<CallsResponse?> GetCallsAsync(CancellationToken cancellationToken = default)
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
                    return new CallsResponse { Message = "No calls", Count = 0, Calls = new List<Call>() };
                }
                
                try
                {
                    return JsonSerializer.Deserialize<CallsResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new CallsResponse { Message = "Failed to parse calls", Count = 0, Calls = new List<Call>() };
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

    public async Task<CallAnswerResponse?> AnswerCallAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/calls/{callId}/answer", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<CallAnswerResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new CallAnswerResponse { Message = "Answer Triggered", CallId = callId };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during answer call: {ex.Message}", ex);
        }
    }

    public async Task<DtmfResponse?> SendDtmfAsync(int callId, string digits, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/calls/{callId}/dtmf/{digits}", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<DtmfResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new DtmfResponse { Message = "DTMF Send", CallId = callId, Dtmf = digits };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during send DTMF: {ex.Message}", ex);
        }
    }

    public async Task<HoldResponse?> HoldCallAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PutAsync($"/calls/{callId}/hold", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<HoldResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new HoldResponse { Message = "Hold Triggered", CallId = callId, Status = true };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during hold call: {ex.Message}", ex);
        }
    }

    public async Task<HoldResponse?> UnholdCallAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.DeleteAsync($"/calls/{callId}/hold", cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<HoldResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new HoldResponse { Message = "UnHold Triggered", CallId = callId, Status = true };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during unhold call: {ex.Message}", ex);
        }
    }

    public async Task<ConferenceResponse?> CreateConferenceAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PutAsync($"/calls/{callId}/conference", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<ConferenceResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new ConferenceResponse { Message = "Conference Triggered", CallId = callId, Status = true };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during create conference: {ex.Message}", ex);
        }
    }

    public async Task<ConferenceResponse?> BreakConferenceAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.DeleteAsync($"/calls/{callId}/conference", cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<ConferenceResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new ConferenceResponse { Message = "BreakConference Triggered", CallId = callId, Status = true };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during break conference: {ex.Message}", ex);
        }
    }

    public async Task<TransferResponse?> TransferCallAsync(int callId, TransferRequest transferRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var json = JsonSerializer.Serialize(transferRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync($"/calls/{callId}/transfer", content, cancellationToken);
            
            var responeString = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<TransferResponse>(responeString, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new TransferResponse { Message = "Transfering Call", CallId = callId, Dest = transferRequest.Uri };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during transfer call: {ex.Message}", ex);
        }
    }

    public async Task<AttendedTransferResponse?> AttendedTransferAsync(int callId, int destCallId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/calls/{callId}/attended-transfer/{destCallId}", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<AttendedTransferResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new AttendedTransferResponse { Message = "Attended transfer Triggered", CallId = callId, DestCallId = destCallId };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during attended transfer: {ex.Message}", ex);
        }
    }

    public async Task<HangupResponse?> HangupCallAsync(int callId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/calls/{callId}/hangup", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<HangupResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new HangupResponse { Message = "Hangup Triggered", CallId = callId };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during hangup call: {ex.Message}", ex);
        }
    }

    public async Task<HangupAllResponse?> HangupAllCallsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync("/hangup_all", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<HangupAllResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new HangupAllResponse { Message = "Hangup All Calls Triggered" };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during hangup all calls: {ex.Message}", ex);
        }
    }

    public async Task<ExitResponse?> ExitApplicationAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync("/exit", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<ExitResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new ExitResponse { Message = "Server Shutdown Received", Result = 200 };
                }
            }
            
            try
            {
                return JsonSerializer.Deserialize<ExitResponse>(content, _jsonOptions);
            }
            catch (JsonException)
            {
                return new ExitResponse { Message = "Exit application failed", Result = (int)response.StatusCode };
            }
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during exit application: {ex.Message}", ex);
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

    public async Task<AccountReregisterResponse?> ReregisterAccountAsync(string accountName, CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = CreateHttpClient();
            var response = await httpClient.PostAsync($"/accounts/{accountName}/reregister", null, cancellationToken);
            
            var content = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<AccountReregisterResponse>(content, _jsonOptions);
                }
                catch (JsonException)
                {
                    return new AccountReregisterResponse { Message = "Re-Register Triggered", AccountName = accountName };
                }
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error during account {accountName} re-registration: {ex.Message}", ex);
        }
    }
}
