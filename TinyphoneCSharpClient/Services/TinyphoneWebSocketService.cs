using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TinyphoneCSharpClient.Configuration;
using TinyphoneCSharpClient.Models;

namespace TinyphoneCSharpClient.Services;

/// <summary>
/// Tinyphone WebSocket service implementation for real-time events
/// </summary>
public class TinyphoneWebSocketService : ITinyphoneWebSocketService
{
    private readonly TinyphoneSettings _settings;
    private readonly ILogger<TinyphoneWebSocketService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private ClientWebSocket? _webSocket;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _receiveTask;
    private bool _disposed;

    public WebSocketConnectionStatus Status { get; private set; } = WebSocketConnectionStatus.Disconnected;

    public event EventHandler<WebSocketConnectionStatus>? StatusChanged;
    public event EventHandler<string>? MessageReceived;
    public event EventHandler<AccountEvent>? AccountEventReceived;
    public event EventHandler<CallEvent>? CallEventReceived;
    public event EventHandler<WebSocketWelcomeMessage>? WelcomeReceived;
    public event EventHandler<Exception>? ErrorOccurred;

    public TinyphoneWebSocketService(TinyphoneSettings settings, ILogger<TinyphoneWebSocketService> logger)
    {
        _settings = settings;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TinyphoneWebSocketService));

        if (Status == WebSocketConnectionStatus.Connected || Status == WebSocketConnectionStatus.Connecting)
            return;

        try
        {
            UpdateStatus(WebSocketConnectionStatus.Connecting);
            
            _webSocket?.Dispose();
            _webSocket = new ClientWebSocket();
            
            // Convert HTTP URL to WebSocket URL
            var wsUrl = _settings.BaseUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            var uri = new Uri($"{wsUrl}/events");
            
            _logger.LogInformation("Connecting to WebSocket: {Uri}", uri);
            
            await _webSocket.ConnectAsync(uri, cancellationToken);
            
            UpdateStatus(WebSocketConnectionStatus.Connected);
            _logger.LogInformation("WebSocket connected successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to WebSocket");
            UpdateStatus(WebSocketConnectionStatus.Failed);
            ErrorOccurred?.Invoke(this, ex);
            throw;
        }
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_webSocket?.State == WebSocketState.Open)
        {
            try
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnect", cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error during WebSocket close");
            }
        }

        _cancellationTokenSource?.Cancel();
        
        if (_receiveTask != null)
        {
            try
            {
                await _receiveTask;
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelling
            }
        }

        UpdateStatus(WebSocketConnectionStatus.Disconnected);
        _logger.LogInformation("WebSocket disconnected");
    }

    public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TinyphoneWebSocketService));

        if (_webSocket?.State != WebSocketState.Open)
            throw new InvalidOperationException("WebSocket is not connected");

        var buffer = Encoding.UTF8.GetBytes(message);
        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
        
        _logger.LogDebug("Sent WebSocket message: {Message}", message);
    }

    public async Task StartListeningAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TinyphoneWebSocketService));

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = _cancellationTokenSource.Token;

        while (!token.IsCancellationRequested)
        {
            try
            {
                if (Status != WebSocketConnectionStatus.Connected)
                {
                    await ConnectAsync(token);
                }

                _receiveTask = ReceiveMessagesAsync(token);
                await _receiveTask;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in WebSocket listener, will attempt reconnect");
                ErrorOccurred?.Invoke(this, ex);
                
                UpdateStatus(WebSocketConnectionStatus.Reconnecting);
                
                // Wait before reconnecting
                try
                {
                    await Task.Delay(5000, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        if (_webSocket == null)
            return;

        var buffer = new byte[4096];
        var messageBuffer = new List<byte>();

        try
        {
            while (_webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    messageBuffer.AddRange(buffer.Take(result.Count));

                    if (result.EndOfMessage)
                    {
                        var messageText = Encoding.UTF8.GetString(messageBuffer.ToArray());
                        messageBuffer.Clear();
                        
                        _logger.LogDebug("Received WebSocket message: {Message}", messageText);
                        
                        // Trigger raw message event
                        MessageReceived?.Invoke(this, messageText);
                        
                        // Try to parse as structured events
                        await ProcessReceivedMessage(messageText);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogInformation("WebSocket close received: {Status} - {Description}", 
                        result.CloseStatus, result.CloseStatusDescription);
                    break;
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Expected when cancelling
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, "WebSocket error occurred");
            throw;
        }
        finally
        {
            if (_webSocket.State != WebSocketState.Closed)
            {
                UpdateStatus(WebSocketConnectionStatus.Disconnected);
            }
        }
    }

    private async Task ProcessReceivedMessage(string messageText)
    {
        try
        {
            // Try to parse as welcome message first (has specific "subcription" field)
            if (messageText.Contains("subcription"))
            {
                var welcomeMessage = JsonSerializer.Deserialize<WebSocketWelcomeMessage>(messageText, _jsonOptions);
                if (welcomeMessage != null)
                {
                    _logger.LogInformation("Received welcome message: {Message}", welcomeMessage.Message);
                    WelcomeReceived?.Invoke(this, welcomeMessage);
                    return;
                }
            }

            // Parse the JSON to check the type field
            using var doc = JsonDocument.Parse(messageText);
            if (doc.RootElement.TryGetProperty("type", out var typeElement))
            {
                var eventType = typeElement.GetString();
                
                switch (eventType?.ToUpperInvariant())
                {
                    case "ACCOUNT":
                        var accountEvent = JsonSerializer.Deserialize<AccountEvent>(messageText, _jsonOptions);
                        if (accountEvent != null)
                        {
                            _logger.LogInformation("Received account event: {Account} - {Status}", 
                                accountEvent.Account, accountEvent.Status);
                            AccountEventReceived?.Invoke(this, accountEvent);
                        }
                        break;
                        
                    case "CALL":
                        var callEvent = JsonSerializer.Deserialize<CallEvent>(messageText, _jsonOptions);
                        if (callEvent != null)
                        {
                            _logger.LogInformation("Received call event: Call {Id} - {State}", 
                                callEvent.Id, callEvent.State);
                            CallEventReceived?.Invoke(this, callEvent);
                        }
                        break;
                        
                    default:
                        _logger.LogDebug("Received unknown event type: {EventType}", eventType);
                        break;
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogDebug(ex, "Could not parse message as structured JSON, treating as raw text");
            // Message is treated as raw text, which was already handled
        }
        
        await Task.CompletedTask; // Make method async for consistency
    }

    private void UpdateStatus(WebSocketConnectionStatus newStatus)
    {
        if (Status != newStatus)
        {
            var oldStatus = Status;
            Status = newStatus;
            _logger.LogDebug("WebSocket status changed: {OldStatus} -> {NewStatus}", oldStatus, newStatus);
            StatusChanged?.Invoke(this, newStatus);
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            
            _webSocket?.Dispose();
            
            _receiveTask?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error during WebSocket service disposal");
        }

        _logger.LogDebug("TinyphoneWebSocketService disposed");
    }
}
