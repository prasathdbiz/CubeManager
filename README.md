# CubeManager (CubeServer)

A Blazor Server application for managing concrete cube testing workflows: quotations, projects, cube casting/testing batches, and generated compliance reports (Daily, Monthly, Statistical, Cost Summary).

## Tech stack

- **.NET 8** / ASP.NET Core Blazor Server
- **MudBlazor 6.19** for UI components
- **Microsoft.Data.SqlClient** (raw ADO.NET, no ORM) against **SQL Server**
- **Scriban** templates for report HTML generation, converted to PDF via **Weasyprint.Wrapped**
- **MailKit** for report emailing, **QRCoder** / **TwoFactorAuth.Net** for 2FA, **ScottPlot** / **Plotly.Blazor** for charts

Source lives at [PC/CubeServer-v1.0.0/CubeServer](PC/CubeServer-v1.0.0/CubeServer).

## Prerequisites

- .NET 8 SDK
- Docker (for a local SQL Server instance), or an existing SQL Server reachable from your machine
- Windows (Weasyprint.Wrapped's bundled Python/GTK3 runtime under `weasyprinter/` is Windows-targeted in this project)

## Setup

1. **Start SQL Server** (local dev container):

   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<your-password>" \
     -p 1433:1433 --name cubemgr-sql -d mcr.microsoft.com/mssql/server:2022-latest
   ```

2. **Create the database and schema.** Create a `CubeMgr` database, then run:

   ```bash
   sqlcmd -S localhost,1433 -d CubeMgr -i PC/CubeServer-v1.0.0/CubeServer/Data/schema.sql
   sqlcmd -S localhost,1433 -d CubeMgr -i PC/CubeServer-v1.0.0/CubeServer/Data/seed_data.sql
   ```

   If you don't have `sqlcmd` installed, you can initialize the database using the included tool:

   ```bash
   dotnet run --project PC/CubeServer-v1.0.0/DbInit/DbInit.csproj -- --server localhost,1433 --database CubeMgr --user sa --password <your-password> --idempotent
   ```

   `schema.sql` creates all tables (idempotent — guarded by `IF NOT EXISTS`). `seed_data.sql` populates a default admin user plus sample Quotations, Projects, CubeSets, Batches, Cubes, and Reports for local testing.

3. **Configure the connection string.** [appsettings.json](PC/CubeServer-v1.0.0/CubeServer/appsettings.json) already contains `DefaultConnection` / `LocalConnection` pointing at `localhost,1433` with database `CubeMgr`. Update the `User Id`/`Password` to match your SQL Server login.

   Recommended (avoids committing passwords to source): set the connection string via user-secrets:

   ```bash
   dotnet user-secrets set "Data:DefaultConnection:ConnectionString" "Server=127.0.0.1,1433;TrustServerCertificate=True;Database=CubeMgr;User Id=sa;Password=<your-password>;" --project PC/CubeServer-v1.0.0/CubeServer/CubeServer.csproj
   dotnet user-secrets set "Data:LocalConnection:ConnectionString" "Server=127.0.0.1,1433;TrustServerCertificate=True;Database=CubeMgr;User Id=sa;Password=<your-password>;" --project PC/CubeServer-v1.0.0/CubeServer/CubeServer.csproj
   ```

4. **Run the app:**

   ```bash
   dotnet run --project PC/CubeServer-v1.0.0/CubeServer/CubeServer.csproj --urls http://localhost:5000
   ```

   (Same command is wired up in `.claude/launch.json` under the name `CubeServer` for editor/agent-driven previews.)

5. Browse to `http://localhost:5000` and log in.

### Default login

Seeded by `seed_data.sql`:

| Field | Value |
|---|---|
| Username | `admin` |
| Password | `Admin@123` |
| Privilege | 100 (Administrator) |

## How the app works

- **Auth**: cookie-based login (`Login.razor`), roles include Administrator / DataEntry / Reporting, enforced via `[Authorize(Roles=...)]` on each page.
- **Data flow**: Quotation → Project → CubeSet (a batch of cubes cast for a spec/grade) → Batches (grouped by test age, e.g. 7-day/28-day) → Cubes (individual specimens with measured dimensions, strength, weight). Barcodes are pre-allocated per project via `BarcodeAllocations`.
- **Reporting**: `GenerateReports.razor` lets a user pick a report type (Daily/Monthly/Statistical), a date (or date range) and a project, then calls `ReportGenerator` (`Data/ReportGenerator.cs`), which:
  1. Queries the relevant Suppliers/TestSpecs/CubeSets/Batches/Cubes data for the project and date window.
  2. Renders a Scriban template from `ScribanReports/*.tpl` to HTML.
  3. Runs `IReportHtmlPostProcessor`s over the HTML (e.g. `SignatureReportHtmlPostProcessor` injects the uploaded digital signature image for Daily/Monthly/Statistical reports).
  4. Converts the HTML to PDF via `Weasyprint.Wrapped` and saves it under `wwwroot/reports`.
  5. Persists a `Reports` row and displays the PDF in an embedded iframe.
- **Cost Summary** (`CostSummary.razor`) generates a separate cost report per project/date range using the same Scriban → HTML → Weasyprint pipeline.
- **Signatures**: users can upload a PNG/JPG signature (`Features/ReportSignature/`) which is stored and later stamped onto generated reports.
- **Settings** page controls SMTP, 2FA, and password-complexity toggles, backed by the `Settings` table.

### Page inventory

| Area | Pages |
|---|---|
| Quotations | `Quotations.razor`, `Quotations_Action.razor`, `QuotationSelect.razor` |
| Projects | `Projects.razor`, `Projects_Action.razor`, `Projects_New.razor` |
| Cube testing | `CubeSets.razor`, `CubeSets_Action.razor`, `CubeSetList.razor`, `CubeSetSelect.razor`, `Batches.razor`, `Batches_Action.razor`, `BatchList.razor`, `Cubes.razor`, `Cubes_Action.razor`, `CubeList.razor` |
| Barcodes / Specs / Suppliers | `BarcodeAllocations.razor`, `TestSpecList.razor`, `TestSpecDialog.razor`, `Suppliers.razor`, `Suppliers_Action.razor`, `SupplierSelect.razor` |
| Reporting | `GenerateReports.razor`, `Reports.razor`, `Reports_Action.razor`, `CostSummary.razor`, `ProjectSelect.razor` |
| Users / Auth | `Users.razor`, `Users_Action.razor`, `Login.razor`, `Logout.razor`, `ChangePasswordDialog.razor`, `NewPasswordDialog.razor`, `AccessDenied.razor` |
| Misc | `Dashboard.razor`, `Settings.razor`, `Index.razor`, `Debug.razor` |

## What's been fixed so far

- **Batches page crash fixed**: `Batches` (a SQL `FLOAT` column) was being read via `SafeGetInt`, which threw `InvalidCastException` whenever a batch had a non-integer or otherwise incompatible dimension value stored. Changed to `SafeGetDouble` at all 5 read sites in [Data/Database.cs](PC/CubeServer-v1.0.0/CubeServer/Data/Database.cs) so the `/Batches` page renders correctly (verified against seeded batch rows showing `150` mm dimensions with no crash).
- **Dummy/seed data**: `Data/seed_data.sql` seeds a default admin user plus sample Quotations, Projects, CubeSets, Batches, Cubes, and Report history rows so the app is usable end-to-end in a fresh local environment without manual data entry.
- **.gitignore**: local-only settings (`.claude/settings.local.json`) excluded from version control so personal permission overrides don't leak into the shared repo.

## Known open issues

- **"Error generating report." on some report types**: `Weasyprint.Wrapped`'s `PrintBaseResult.HasError` is set whenever the underlying `weasyprint` process writes *anything* to stderr — including non-fatal warnings (e.g. failing to resolve a relative `--base-url` for CSS/image assets). Some report-generation call sites in `ReportGenerator.cs` treat any `HasError` as a hard failure, while others also allow success when `ExitCode == 0`. This inconsistency, plus a possible base-url mismatch when locating `cost_summary_report.css` / logo assets, is the leading suspect for intermittent Cost Summary / report generation failures and hasn't been fixed yet.
- **Daily report project lookup** matches an exact single date (`GetProjectsWithTestedCubes(date, date, ...)`), so a project won't appear in the picker unless a cube was tested on that *exact* day — worth seeding more test dates or relaxing the match if this trips users up.
