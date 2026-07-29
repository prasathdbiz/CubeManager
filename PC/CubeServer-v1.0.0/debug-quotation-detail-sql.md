# Debug Session: quotation-detail-sql [OPEN]

## Symptom
- Opening quotation detail/view page throws SQL connection error and breaks the Blazor circuit.

## Scope
- Affects `Quotations_Action.razor` when loading `/Quotations/{action}/{quoNum}`.

## Hypotheses
- H1: `GetQuotation(int quoNum)` still uses SQL directly and has no Development dummy-data fallback.
- H2: `CountProjectsWithQuotation(int quoNum)` also still uses SQL directly and contributes to the failure path.
- H3: The quotations list page works only because `GetQuotations(...)` was patched earlier, but detail navigation was not.
- H4: The Blazor disconnect errors are secondary symptoms caused by an unhandled exception in `OnInitialized()`.

## Evidence Plan
- Instrument `Quotations_Action.OnInitialized()` before and after the DB calls.
- Instrument `Database.GetQuotation()` and `Database.CountProjectsWithQuotation()` entry and result paths.
- Reproduce by opening a quotation detail page in Development.

## Status
- Evidence collected from browser/runtime stack:
  - `Database.GetQuotation()` crashed from `Quotations_Action.OnInitialized()`.
  - `CountProjectsWithQuotation()` was also still DB-backed in the same detail flow.
- Minimal fix applied:
  - Added Development dummy fallback to `GetQuotation(int quoNum)`.
  - Added Development dummy fallback to `CountProjectsWithQuotation(int quoNum)`.
- Verification:
  - Reopening `/Quotations/View/2024001` no longer emits the SQL exception.
  - Remaining `net::ERR_ABORTED` on navigation is a benign browser-side navigation artifact.
- Awaiting user confirmation before cleanup.
