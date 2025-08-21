# API Endpoint to DTO Mapping

This document shows the exact mapping between Tinyphone server endpoints and C# client DTOs based on the actual server.cpp implementation.

## Complete Endpoint Mapping

| Endpoint | Method | Request DTO | Response DTO | Server Response Fields |
|----------|--------|-------------|--------------|------------------------|
| `/` | GET | - | `AppVersionResponse` | `message`, `version` |
| `/config` | GET | - | `ConfigResponse` | `version`, `config`, `sip-log-file`, `http-log-file` |
| `/devices` | GET | - | `DevicesResponse` | `message`, `count`, `devices[]` |
| `/login` | POST | `LoginRequest` | `LoginResponse` | `message`, `account_name?`, `id?`, `result`, `reason?` |
| `/logout` | POST | - | `LogoutResponse` | `message`, `result`, `accounts[]`, `failed_count?` |
| `/accounts` | GET | - | `AccountsResponse` | `message`, `accounts[]` |
| `/accounts/{name}/reregister` | POST | - | `AccountReregisterResponse` | `message`, `account_name` |
| `/accounts/{name}/logout` | POST | - | `AccountLogoutResponse` | `message`, `account_name`, `result`, `call_count?` |
| `/dial` | POST | `DialRequest` | `DialResponse` | `message`, `call_id`, `sid`, `party`, `account` |
| `/calls` | GET | - | `CallsResponse` | `message`, `count`, `calls[]` |
| `/calls/{id}/answer` | POST | - | `CallAnswerResponse` | `message`, `call_id` |
| `/calls/{id}/attended-transfer/{dest_id}` | POST | - | `AttendedTransferResponse` | `message`, `call_id`, `dest_call_id` |
| `/calls/{id}/hold` | PUT/DELETE | - | `HoldResponse` | `message`, `call_id`, `status` |
| `/calls/{id}/conference` | PUT/DELETE | - | `ConferenceResponse` | `message`, `call_id`, `status` |
| `/calls/{id}/transfer` | POST | `TransferRequest` | `TransferResponse` | `message`, `call_id`, `sid`, `dest`, `account` |
| `/calls/{id}/dtmf/{digits}` | POST | - | `DtmfResponse` | `message`, `call_id`, `dtmf` |
| `/calls/{id}/hangup` | POST | - | `HangupResponse` | `message`, `call_id` |
| `/hangup_all` | POST | - | `HangupAllResponse` | `message` |
| `/exit` | POST | - | `ExitResponse` | `message`, `result`, `source` |

## Nested Object Structures

### AudioDevice (in DevicesResponse.devices[])
```json
{
  "name": "string",
  "driver": "string", 
  "id": number,
  "inputCount": number,
  "outputCount": number,
  "pa-api": "string?" // Windows only
}
```

### Account (in AccountsResponse.accounts[])
```json
{
  "id": number,
  "uri": "string",
  "name": "string", 
  "active": boolean,
  "status": "string"
}
```

### Call (in CallsResponse.calls[])
```json
{
  "id": number,
  "account": "string",
  "sid": "string",
  "party": "string",
  "callerId": "string",
  "displayName": "string", 
  "state": "string",
  "direction": "INCOMING" | "OUTGOING",
  "duration": number,
  "hold": "string"
}
```

### LogoutAccountInfo (in LogoutResponse.accounts[])
```json
{
  "id": number,
  "name": "string"
}
```

## Request Payloads

### LoginRequest
```json
{
  "username": "string",
  "login": "string?",
  "password": "string", 
  "domain": "string",
  "proxy": "string?"
}
```

### DialRequest
```json
{
  "uri": "string",
  "account": "string?"
}
```

### TransferRequest
```json
{
  "uri": "string"
}
```

## Error Response Patterns

Most endpoints return error responses with these common fields:
```json
{
  "message": "string",
  "call_id": number?, // For call-related errors
  "account_name": "string?", // For account-related errors
  "reason": "string?", // Additional error details
  "result": number? // HTTP status or operation result
}
```

## Special Response Behaviors

### Login Endpoint (`/login`)
- Returns different response structures based on scenario:
  - Account already exists: `200` with `id` field
  - Login in progress: `202` with `account_name` 
  - Max accounts reached: `403` with just `message`
  - JSON parsing error: `400` with `reason` field
  - System error: `500` with `result: 503`

### Conference Endpoint (`/calls/{id}/conference`)
- PUT: Creates conference, blocks if call is on hold
- DELETE: Breaks call out of conference
- `status` field can be boolean or string depending on operation

### Hold Endpoint (`/calls/{id}/hold`)
- PUT: Holds call, returns `status` boolean
- DELETE: Unholds call, returns `status` boolean

### Exit Endpoint (`/exit`)
- Requires authentication via IP (localhost) or security header
- Returns `result: 401` for unauthorized requests
- Returns `result: 200` for successful shutdown

## HTTP Status Codes Used

- `200`: Success
- `202`: Accepted/In Progress (login, dial, call operations)
- `400`: Bad Request (not found, invalid payload)
- `403`: Forbidden (max accounts/calls reached)
- `429`: Too Many Requests (max concurrent calls)
- `500`: Internal Server Error

## Notes

1. All responses include CORS headers (`Access-Control-Allow-Origin: *`)
2. All responses have `Content-Type: application/json`
3. Integer IDs are used for calls and accounts (not strings)
4. The server uses nlohmann::json for JSON serialization
5. Some fields are optional and only present in specific scenarios
6. Error handling follows a consistent pattern across endpoints
