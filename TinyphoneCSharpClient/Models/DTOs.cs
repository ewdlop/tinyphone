using System.Text.Json.Serialization;

namespace TinyphoneCSharpClient.Models;

/// <summary>
/// Application version information response - GET /
/// </summary>
public class AppVersionResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}

/// <summary>
/// Config response - GET /config
/// </summary>
public class ConfigResponse
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
    
    [JsonPropertyName("config")]
    public object? Config { get; set; }
    
    [JsonPropertyName("sip-log-file")]
    public string? SipLogFile { get; set; }
    
    [JsonPropertyName("http-log-file")]
    public string? HttpLogFile { get; set; }
}

/// <summary>
/// Audio device information - from /devices response
/// </summary>
public class AudioDevice
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("driver")]
    public string Driver { get; set; } = string.Empty;
    
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("inputCount")]
    public int InputCount { get; set; }
    
    [JsonPropertyName("outputCount")]
    public int OutputCount { get; set; }
    
    [JsonPropertyName("pa-api")]
    public string? PaApi { get; set; }
}

/// <summary>
/// Devices response - GET /devices
/// </summary>
public class DevicesResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("devices")]
    public List<AudioDevice> Devices { get; set; } = new();
}

/// <summary>
/// Login request - POST /login
/// </summary>
public class LoginRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    
    [JsonPropertyName("login")]
    public string? Login { get; set; }
    
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
    
    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;
    
    [JsonPropertyName("proxy")]
    public string? Proxy { get; set; }
}

/// <summary>
/// Login response - POST /login (various scenarios)
/// </summary>
public class LoginResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }
    
    [JsonPropertyName("id")]
    public int? Id { get; set; }
    
    [JsonPropertyName("result")]
    public int Result { get; set; }
    
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>
/// Account information - from /accounts response
/// </summary>
public class Account
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("active")]
    public bool Active { get; set; }
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Accounts response - GET /accounts
/// </summary>
public class AccountsResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("accounts")]
    public List<Account> Accounts { get; set; } = new();
}

/// <summary>
/// Account reregister response - POST /accounts/{name}/reregister
/// </summary>
public class AccountReregisterResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("account_name")]
    public string AccountName { get; set; } = string.Empty;
}

/// <summary>
/// Account logout response - POST /accounts/{name}/logout
/// </summary>
public class AccountLogoutResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("account_name")]
    public string AccountName { get; set; } = string.Empty;
    
    [JsonPropertyName("result")]
    public int Result { get; set; }
    
    [JsonPropertyName("call_count")]
    public int? CallCount { get; set; }
}

/// <summary>
/// Dial request - POST /dial
/// </summary>
public class DialRequest
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
    
    [JsonPropertyName("account")]
    public string? Account { get; set; }
}

/// <summary>
/// Dial response - POST /dial
/// </summary>
public class DialResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int CallId { get; set; }
    
    [JsonPropertyName("sid")]
    public string Sid { get; set; } = string.Empty;
    
    [JsonPropertyName("party")]
    public string Party { get; set; } = string.Empty;
    
    [JsonPropertyName("account")]
    public string Account { get; set; } = string.Empty;
}

/// <summary>
/// Call information - from /calls response
/// </summary>
public class Call
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("account")]
    public string Account { get; set; } = string.Empty;
    
    [JsonPropertyName("sid")]
    public string Sid { get; set; } = string.Empty;
    
    [JsonPropertyName("party")]
    public string Party { get; set; } = string.Empty;
    
    [JsonPropertyName("callerId")]
    public string CallerId { get; set; } = string.Empty;
    
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;
    
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;
    
    [JsonPropertyName("direction")]
    public string Direction { get; set; } = string.Empty;
    
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
    
    [JsonPropertyName("hold")]
    public string Hold { get; set; } = string.Empty;
}

/// <summary>
/// Calls response - GET /calls
/// </summary>
public class CallsResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("calls")]
    public List<Call> Calls { get; set; } = new();
}

/// <summary>
/// Call answer response - POST /calls/{id}/answer
/// </summary>
public class CallAnswerResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int CallId { get; set; }
}

/// <summary>
/// Attended transfer response - POST /calls/{id}/attended-transfer/{dest_id}
/// </summary>
public class AttendedTransferResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int CallId { get; set; }
    
    [JsonPropertyName("dest_call_id")]
    public int DestCallId { get; set; }
}

/// <summary>
/// Hold/Unhold response - PUT/DELETE /calls/{id}/hold
/// </summary>
public class HoldResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int CallId { get; set; }
    
    [JsonPropertyName("status")]
    public bool Status { get; set; }
}

/// <summary>
/// Conference response - PUT/DELETE /calls/{id}/conference
/// </summary>
public class ConferenceResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int CallId { get; set; }
    
    [JsonPropertyName("status")]
    public object Status { get; set; } = new(); // Can be bool or string based on server logic
}

/// <summary>
/// Transfer request - POST /calls/{id}/transfer
/// </summary>
public class TransferRequest
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
}

/// <summary>
/// Transfer response - POST /calls/{id}/transfer
/// </summary>
public class TransferResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int CallId { get; set; }
    
    [JsonPropertyName("sid")]
    public string Sid { get; set; } = string.Empty;
    
    [JsonPropertyName("dest")]
    public string Dest { get; set; } = string.Empty;
    
    [JsonPropertyName("account")]
    public string Account { get; set; } = string.Empty;
}

/// <summary>
/// DTMF response - POST /calls/{id}/dtmf/{digits}
/// </summary>
public class DtmfResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int CallId { get; set; }
    
    [JsonPropertyName("dtmf")]
    public string Dtmf { get; set; } = string.Empty;
}

/// <summary>
/// Hangup response - POST /calls/{id}/hangup
/// </summary>
public class HangupResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int CallId { get; set; }
}

/// <summary>
/// Hangup all response - POST /hangup_all
/// </summary>
public class HangupAllResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Logout response - POST /logout
/// </summary>
public class LogoutResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("result")]
    public int Result { get; set; }
    
    [JsonPropertyName("accounts")]
    public List<LogoutAccountInfo> Accounts { get; set; } = new();
    
    [JsonPropertyName("failed_count")]
    public int? FailedCount { get; set; }
}

/// <summary>
/// Account info in logout response
/// </summary>
public class LogoutAccountInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Exit response - POST /exit
/// </summary>
public class ExitResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("result")]
    public int Result { get; set; }
    
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;
}

/// <summary>
/// Generic error response used across multiple endpoints
/// </summary>
public class ErrorResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int? CallId { get; set; }
    
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }
    
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
    
    [JsonPropertyName("result")]
    public int? Result { get; set; }
}

/// <summary>
/// Base API response class (for client use)
/// </summary>
public class ApiResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}

/// <summary>
/// Generic API response with data
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class ApiResponse<T> : ApiResponse
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}