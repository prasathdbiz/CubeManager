# Debug Session: server-startup-fail
- **Status**: [OPEN]
- **Issue**: `CubeServer` does not start successfully on this machine; earlier runs failed on missing log path and then on SQL Server connectivity during startup.
- **Debug Server**: http://127.0.0.1:7777/event
- **Log File**: `.dbg/trae-debug-log-server-startup-fail.ndjson`

## Reproduction Steps
1. Run `CubeServer` from `c:\Users\ChandrukhasanRamacha\Downloads\PC\CubeServer-v1.0.0\CubeServer`.
2. Wait for ASP.NET startup/build to complete.
3. Verify whether the app binds to `http://localhost:5000` or exits during startup.

## Hypotheses & Verification
| ID | Hypothesis | Likelihood | Effort | Evidence |
|----|------------|------------|--------|----------|
| A | Startup still exits before Kestrel binds because one or more database calls are executed eagerly in app initialization. | High | Low | Pending |
| B | A hosted/background service blocks or crashes startup after `Program` begins building services. | High | Low | Pending |
| C | The app reaches `app.Run()` but binds to a different URL/profile than expected, so the health probe is checking the wrong endpoint. | Medium | Low | Pending |
| D | A secondary file/path dependency under the runtime root still throws during startup even after the log-path fix. | Medium | Low | Pending |
| E | Startup succeeds but requests stall because the first request path triggers a database dependency instead of returning a basic page. | Medium | Low | Pending |

## Log Evidence
- Pre-fix: startup reached `Program.cs:62` and then terminated with `Unable to configure HTTPS endpoint. No server certificate was specified...`.
- Pre-fix: `CubeServerApp` and `SchedulerService` both logged SQL connectivity failures, but instrumentation showed startup still progressed through host build.
- Pre-fix: `http://localhost:5000/` returned `500` with `System.NullReferenceException` in `AuthorizeViewCore.OnParametersSetAsync()`.
- Post-fix: startup again showed SQL connectivity failures, but progressed through `CubeServerApp` completion, host build, and `app.Run()` without the HTTPS endpoint exception.
- Post-fix: `http://localhost:5000/` returned `200` and served the app HTML shell.

## Verification Conclusion
- Hypothesis A: Rejected as fatal. DB access fails, but startup continues because the guarded initialization path no longer terminates the host.
- Hypothesis B: Rejected as fatal. `SchedulerService` still logs DB errors, but the host remains alive.
- Hypothesis C: Confirmed. The Development launch profile's HTTPS binding failed on this machine because no valid dev certificate was available.
- Hypothesis D: Rejected as current blocker. The log-path/root-path issue was fixed earlier and no longer stops startup.
- Hypothesis E: Confirmed for the second failure. The initial page `500` was caused by `CustomAuthenticationStateProvider.GetAuthenticationStateAsync()` returning `null`, which broke `AuthorizeView`.
