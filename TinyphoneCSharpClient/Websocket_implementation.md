# WebSocket Implementation for Tinyphone C# Client

ased on `server.cpp` analysis:

```cpp
// Server WebSocket configuration (lines 102-142)
if (tp::ApplicationConfig.enableWSEvents) {
    CROW_ROUTE(app, "/events")
        .websocket()
        .onopen([&](crow::websocket::connection& conn) {
            json message = {
                { "message", "welcome" },
                { "subcription", "created" }  // Note: typo in server code
            };
            conn.send_text(message.dump());
        })
        .onclose([&](crow::websocket::connection& conn, const std::string& reason) {
            // Connection cleanup
        })
        .onmessage([&](crow::websocket::connection& conn, const std::string& data, bool is_binary) {
            json message = {
                { "message", "nothing here" },
            };
            conn.send_text(message.dump());
        });
}
```

### 1. Account Events (`type: "ACCOUNT"`)

**Triggered by**: Account registration state changes (`OnRegStateParam`)

**Server Code** (lines 30-47):
```cpp
void publishEvent(AccountInfo ai, OnRegStateParam &prm) {
    json event = {
        { "type","ACCOUNT" },
        { "account", ai.uri},
        { "presence", ai.onlineStatusText },
        { "status", ai.regStatusText },
        { "id", ai.id },
        { "code",  prm.code },
    };

### 2. Call State Events (`type: "CALL"`)

**Triggered by**: Call state changes (`OnCallStateParam`)

**Server Code** (lines 49-65):
```cpp
void publishEvent(CallInfo ci, OnCallStateParam &prm) {
    tp::SIPUri uri;
    tp::ParseSIPURI(ci.remoteUri, &uri);
    json event = {
        { "type","CALL" },
        { "id", ci.id },
        { "party", ci.remoteUri },
        { "callerId", uri.user },
        { "displayName", uri.name },
        { "state", ci.stateText },
        { "sid", ci.callIdString },
    };
    chan->push(event.dump());
}
```

