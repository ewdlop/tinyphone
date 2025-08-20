using System.Text;
using System.Text.Json;
using TinyphoneCSharpClient.Models;

namespace TinyphoneCSharpClient.Services;

/// <summary>
/// Tinyphone REST API service implementation
/// </summary>
public class TinyphoneService : ITinyphoneService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public TinyphoneService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<AppVersionResponse?> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AppVersionResponse>(content, _jsonOptions);
            }
            
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
            var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/login", content, cancellationToken);
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
            var response = await _httpClient.PostAsync("/logout", null, cancellationToken);
            
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
            var response = await _httpClient.GetAsync("/accounts", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Account>>(content, _jsonOptions);
            }
            
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
            var response = await _httpClient.GetAsync($"/accounts/{accountName}/logout", cancellationToken);
            
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
            var json = JsonSerializer.Serialize(dialRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/dial", content, cancellationToken);
            
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
            var response = await _httpClient.GetAsync("/calls", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Call>>(content, _jsonOptions);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error getting calls list: {ex.Message}", ex);
        }
    }

    public async Task<ApiResponse> AnswerCallAsync(string callId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/calls/{callId}/answer", null, cancellationToken);
            
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

    public async Task<ApiResponse> SendDtmfAsync(string callId, string digits, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/calls/{callId}/dtmf/{digits}", null, cancellationToken);
            
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

    public async Task<ApiResponse> HoldCallAsync(string callId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/calls/{callId}/hold", null, cancellationToken);
            
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

    public async Task<ApiResponse> UnholdCallAsync(string callId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/calls/{callId}/hold", cancellationToken);
            
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

    public async Task<ApiResponse> CreateConferenceAsync(string callId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/calls/{callId}/conference", null, cancellationToken);
            
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

    public async Task<ApiResponse> BreakConferenceAsync(string callId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/calls/{callId}/conference", cancellationToken);
            
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

    public async Task<ApiResponse> TransferCallAsync(string callId, TransferRequest transferRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(transferRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/calls/{callId}/transfer", content, cancellationToken);
            
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

    public async Task<ApiResponse> AttendedTransferAsync(string callId, string destCallId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/calls/{callId}/attended-transfer/{destCallId}", null, cancellationToken);
            
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

    public async Task<ApiResponse> HangupCallAsync(string callId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/calls/{callId}/hangup", null, cancellationToken);
            
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
            var response = await _httpClient.PostAsync("/hangup_all", null, cancellationToken);
            
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
            var response = await _httpClient.PostAsync("/exit", null, cancellationToken);
            
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
}
