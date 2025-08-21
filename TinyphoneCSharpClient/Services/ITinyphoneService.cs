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
    Task<LoginResponse?> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout all accounts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Logout result</returns>
    Task<LogoutResponse?> LogoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of registered accounts
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Accounts response</returns>
    Task<AccountsResponse?> GetAccountsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout account with specified name
    /// </summary>
    /// <param name="accountName">Account name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account logout result</returns>
    Task<AccountLogoutResponse?> LogoutAccountAsync(string accountName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dial a call with specified SIP URI
    /// </summary>
    /// <param name="dialRequest">Dial request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dial result</returns>
    Task<DialResponse?> DialAsync(DialRequest dialRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of active calls
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Calls response</returns>
    Task<CallsResponse?> GetCallsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Answer call with specified ID
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Answer result</returns>
    Task<CallAnswerResponse?> AnswerCallAsync(int callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send DTMF digits to specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="digits">DTMF digits</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Send result</returns>
    Task<DtmfResponse?> SendDtmfAsync(int callId, string digits, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hold specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Hold result</returns>
    Task<HoldResponse?> HoldCallAsync(int callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unhold specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unhold result</returns>
    Task<HoldResponse?> UnholdCallAsync(int callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create conference by merging other running calls with specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Conference creation result</returns>
    Task<ConferenceResponse?> CreateConferenceAsync(int callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Break specified call out of conference
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Break result</returns>
    Task<ConferenceResponse?> BreakConferenceAsync(int callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Transfer call to specified URI
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="transferRequest">Transfer request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transfer result</returns>
    Task<TransferResponse?> TransferCallAsync(int callId, TransferRequest transferRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiate attended call transfer
    /// </summary>
    /// <param name="callId">Call ID to be transferred</param>
    /// <param name="destCallId">Call ID to be replaced</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transfer result</returns>
    Task<AttendedTransferResponse?> AttendedTransferAsync(int callId, int destCallId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hangup specified call
    /// </summary>
    /// <param name="callId">Call ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Hangup result</returns>
    Task<HangupResponse?> HangupCallAsync(int callId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hangup all calls
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Hangup result</returns>
    Task<HangupAllResponse?> HangupAllCallsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Exit the application
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Exit result</returns>
    Task<ExitResponse?> ExitApplicationAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get audio devices list
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Audio devices list</returns>
    Task<DevicesResponse?> GetDevicesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get application configuration
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Configuration information</returns>
    Task<ConfigResponse?> GetConfigAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Re-register account with specified name
    /// </summary>
    /// <param name="accountName">Account name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Re-registration result</returns>
    Task<AccountReregisterResponse?> ReregisterAccountAsync(string accountName, CancellationToken cancellationToken = default);
}
