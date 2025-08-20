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
/// Dial request
/// </summary>
public class DialRequest
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;
    
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
/// Account information
/// </summary>
public class Account
{
    [JsonPropertyName("account_name")]
    public string AccountName { get; set; } = string.Empty;
    
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    
    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;
    
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Call information
/// </summary>
public class Call
{
    [JsonPropertyName("call_id")]
    public string CallId { get; set; } = string.Empty;
    
    [JsonPropertyName("direction")]
    public string Direction { get; set; } = string.Empty;
    
    [JsonPropertyName("remote_uri")]
    public string RemoteUri { get; set; } = string.Empty;
    
    [JsonPropertyName("local_uri")]
    public string LocalUri { get; set; } = string.Empty;
    
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;
    
    [JsonPropertyName("duration")]
    public int Duration { get; set; }
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
