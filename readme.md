# PowerShellServer

Minimal .NET 9 API that executes PowerShell scripts received from an n8n AI agent.

[![Watch the video](https://github.com/prax78/PowerShellServer_n8n/blob/master/ps_server.png)](https://www.youtube.com/watch?v=Zt08JPVtcLc)



## Overview
This project is built with the .NET minimal API template and exposes a single POST route:
- `POST /executecode`

The API expects a JSON payload containing a PowerShell script. For each request it:
1. Saves the script to a `.ps1` file named with a GUID.
2. Executes that file with PowerShell on the host.
3. Captures stdout/stderr and returns the execution result to the caller (the n8n AI agent).

This project is intentionally small and focused — it does not include production-ready authentication, sandboxing, or advanced safety controls.

## Requirements
- .NET 9 SDK
- PowerShell installed on the host machine (Windows PowerShell or PowerShell Core)
- Visual Studio 2022 (recommended) or `dotnet` CLI

## Files of interest
- `Program.cs` — minimal API routing and startup.
- `ExecutePS.cs` — implementation that writes GUID-named `.ps1` file and runs PowerShell.
- `RecCode.cs` — models for incoming requests / responses.
- `appsettings.json` — configuration values (if present).
- `PowerShellServer.http` — example HTTP requests for use with REST client tooling.

## Route
POST `/executecode`

Expected request body (JSON):
- `script` — string containing the PowerShell script to execute.

Example request body:
{
  "code": "Write-Output 'Hello from n8n'; Get-Process | Select-Object -First 3"
}

Example response (typical):
{
  "result":"<PS Script output>
}

(Exact response fields depend on the implementation in `RecCode.cs` / `ExecutePS.cs`.)

## Example usages

Curl:

````````bash
curl -X POST http://localhost:5000/executecode -H "Content-Type: application/json" -d "{\"code\":\"Write-Output 'Hello from n8n'; Get-Process | Select-Object -First 3\"}"
````````

VSCode / REST client (`PowerShellServer.http`):
````````http
POST http://localhost:5000/executecode
Content-Type: application/json

{
  "code": "Write-Output 'Hello from n8n'; Get-Process | Select-Object -First 3"
}
````````

# Response
```json
{
  "result":"<PS Script output>
}
```
Make sure to run on an endpoint accessible to your n8n instance (use HTTPS in production).

## Configuration
Check `appsettings.json` for any configurable settings (ports, logging). If you modify ports or URLs, update the n8n workflow accordingly.

## Security & Safety (Important)
This service executes arbitrary PowerShell code supplied by the caller. That is inherently dangerous.

Recommendations before using in production:
- Never expose the service publicly without strict authentication and authorization.
- Run the service in an isolated environment (container, VM) with minimal privileges.
- Apply network restrictions (firewall, private subnets).
- Use TLS (HTTPS) and validate requests (API key, JWT, IP allow-list).
- Add input validation, execution time limits, and resource constraints.
- Consider sandboxing or using a restricted PowerShell session configuration (ConstrainedLanguageMode) or custom run-as account.
- Log and monitor usage and maintain retention policies for generated `.ps1` files — or delete them immediately after execution.

## Troubleshooting
- If execution fails, check that PowerShell is in PATH and accessible by the process running the API.
- Check the application output / logs inside Visual Studio Output window or the console when using `dotnet run`.
- If using Visual Studio, view logs via the __Output__ window panes (select the relevant pane).

## Contribution & License
This repository is a focused utility. Treat it as a starting point and harden before production use.


