namespace TinyphoneCSharpClient.Configuration;

/// <summary>
/// Tinyphone configuration settings
/// </summary>
public class TinyphoneSettings
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "TinyphoneSettings";
    
    /// <summary>
    /// Base URL for the Tinyphone API
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:6060";
    
    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}
