The softphone exposes the following resources on port 6060.

Resource	Method	Payload	Description
/	GET		Returns hi and the app version
/events	WS		WebSocket endpoint for realtime events
/login	POST	
{
"username": "string" ,
"login": "optional-string**" ,
"password": "string",
"domain": "string",
"proxy": "optional-string**"
}
Account login with the provided details
/logout	POST		Logout of all accounts
/accounts	GET		Returns list of registed accounts
/accounts/{account_name}/logout	GET		Logout of account with provided account_name
/dial	POST	
{
"uri": "sip-uri",
"account": "account_name**" 
}
Dial a call with provided sip-uri
/calls	GET		Returns list of active calls
/calls/{call_id}/answer	POST		answer call with specified call_id
/calls/{call_id}/dtmf/{digits}	POST		Send dtmf digits digits to call with specified call_id
/calls/{call_id}/hold	PUT		Hold call with specified call_id
/calls/{call_id}/hold	DELETE		UnHold call with specified call_id
/calls/{call_id}/conference	PUT		Create conference by merging other running calls with given call_id
/calls/{call_id}/conference	DELETE		Break specified call_id out of conference
/calls/{call_id}/transfer	POST	
{
"uri": "sip-uri",
}
transfer call_id to specified uri
/calls/{call_id}/attended-transfer/{dest_call_id}	POST		Initiate attended call transfer
call_id=The call id to be transfered
dest_call_id=The call id to be replaced
/calls/{call_id}/hangup	POST		hangup call with specified call_id
/hangup_all	POST		Hangup all calls
/exit	POST		Exit the application