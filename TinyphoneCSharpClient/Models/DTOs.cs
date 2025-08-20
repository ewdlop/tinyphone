using System.Text.Json.Serialization;

namespace TinyphoneCSharpClient.Models;

/// <summary>
/// Application version information response
/// </summary>
public class AppVersionResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}

/// <summary>
/// Login request
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
/// Login response
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
}

/// <summary>
/// Dial request
/// </summary>
public class DialRequest
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
    
    [JsonPropertyName("account")]
    public string? Account { get; set; }
}

/// <summary>
/// Dial response
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
/// Transfer request
/// </summary>
public class TransferRequest
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
}

/// <summary>
/// Audio device information
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
/// Devices response
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
/// Account information
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
/// Call information
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
/// Base API response class
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
/// Generic API response
/// </summary>
/// <typeparam name="T">Data type</typeparam>
public class ApiResponse<T> : ApiResponse
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}

/// <summary>
/// Accounts API response
/// </summary>
public class AccountsResponse
{
    [JsonPropertyName("accounts")]
    public List<Account> Accounts { get; set; } = new();
    
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Calls API response
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
/// Generic operation response (for answer, hold, hangup, etc.)
/// </summary>
public class OperationResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    
    [JsonPropertyName("call_id")]
    public int? CallId { get; set; }
    
    [JsonPropertyName("account_name")]
    public string? AccountName { get; set; }
    
    [JsonPropertyName("result")]
    public int? Result { get; set; }
    
    [JsonPropertyName("status")]
    public bool? Status { get; set; }
    
    [JsonPropertyName("dest_call_id")]
    public int? DestCallId { get; set; }
    
    [JsonPropertyName("dtmf")]
    public string? Dtmf { get; set; }
}

/// <summary>
/// Config response
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
