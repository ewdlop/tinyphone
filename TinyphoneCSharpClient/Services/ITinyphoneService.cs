using TinyphoneCSharpClient.Models;

namespace TinyphoneCSharpClient.Services;

/// <summary>
/// Tinyphone REST API service interface
/// </summary>
public interface ITinyphoneService
{
    /// <summary>
    /// Get application version information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Version information</returns>
    Task<AppVersionResponse?> GetVersionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Login to account with provided details
    /// </summary>
    /// <param name="loginRequest">Login request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Login result</returns>
    Task<ApiResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout all accounts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logout result</returns>
    Task<ApiResponse> LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of registered accounts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of accounts</returns>
    Task<List<Account>?> GetAccountsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout account with specified name
    /// </summary>
    /// <param name="accountName">Account name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logout result</returns>
    Task<ApiResponse> LogoutAccountAsync(string accountName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dial a call with specified SIP URI
    /// </summary>
    /// <param name="dialRequest">Dial request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dial result</returns>
    Task<ApiResponse> DialAsync(DialRequest dialRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of active calls
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of calls</returns>
    Task<List<Call>?> GetCallsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Answer call with specified ID
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Answer result</returns>
    Task<ApiResponse> AnswerCallAsync(string callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send DTMF digits to specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="digits">DTMF digits</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Send result</returns>
    Task<ApiResponse> SendDtmfAsync(string callId, string digits, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hold specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Hold result</returns>
    Task<ApiResponse> HoldCallAsync(string callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unhold specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unhold result</returns>
    Task<ApiResponse> UnholdCallAsync(string callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create conference by merging other running calls with specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Conference creation result</returns>
    Task<ApiResponse> CreateConferenceAsync(string callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Break specified call out of conference
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Break result</returns>
    Task<ApiResponse> BreakConferenceAsync(string callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Transfer call to specified URI
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="transferRequest">Transfer request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transfer result</returns>
    Task<ApiResponse> TransferCallAsync(string callId, TransferRequest transferRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiate attended call transfer
    /// </summary>
    /// <param name="callId">Call ID to be transferred</param>
    /// <param name="destCallId">Call ID to be replaced</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transfer result</returns>
    Task<ApiResponse> AttendedTransferAsync(string callId, string destCallId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hangup specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Hangup result</returns>
    Task<ApiResponse> HangupCallAsync(string callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hangup all calls
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Hangup result</returns>
    Task<ApiResponse> HangupAllCallsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Exit the application
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exit result</returns>
    Task<ApiResponse> ExitApplicationAsync(CancellationToken cancellationToken = default);
}
