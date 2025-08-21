using TinyphoneCSharpClient.Models;

namespace TinyphoneCSharpClient.Services;

/// <summary>
/// Tinyphone WebSocket service interface for real-time events
/// </summary>
public interface ITinyphoneWebSocketService : IDisposable
{
    /// <summary>
    /// Current connection status
    /// </summary>
    WebSocketConnectionStatus Status { get; }
    
    /// <summary>
    /// Event triggered when connection status changes
    /// </summary>
    event EventHandler<WebSocketConnectionStatus>? StatusChanged;
    
    /// <summary>
    /// Event triggered when a message is received
    /// </summary>
    event EventHandler<string>? MessageReceived;
    
    /// <summary>
    /// Event triggered when an account event is received
    /// </summary>
    event EventHandler<AccountEvent>? AccountEventReceived;
    
    /// <summary>
    /// Event triggered when a call event is received
    /// </summary>
    event EventHandler<CallEvent>? CallEventReceived;
    
    /// <summary>
    /// Event triggered when the welcome message is received
    /// </summary>
    event EventHandler<WebSocketWelcomeMessage>? WelcomeReceived;
    
    /// <summary>
    /// Event triggered when an error occurs
    /// </summary>
    event EventHandler<Exception>? ErrorOccurred;
    
    /// <summary>
    /// Connect to the WebSocket endpoint
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when connected</returns>
    Task ConnectAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Disconnect from the WebSocket endpoint
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when disconnected</returns>
    Task DisconnectAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send a message to the server
    /// </summary>
    /// <param name="message">Message to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that completes when message is sent</returns>
    Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Start listening for events (auto-reconnect enabled)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task that runs until cancelled</returns>
    Task StartListeningAsync(CancellationToken cancellationToken = default);
}
