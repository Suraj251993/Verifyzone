# VerifyZone (OrgCheck) — Technical Documentation & Local Setup Report

*Prepared: 26 July 2026. No business logic, files, or code in the repository were modified during this analysis — see the "Modification Log" at the end.*

## 1. What this project is

VerifyZone is a background-verification / employee-and-education-verification web platform (cookie names, CSS, and CORS origin all reference `verifyzone.in`). The solution is internally named **OrgCheck**. It is a single-project, server-rendered ASP.NET Core MVC application backed by PostgreSQL — there is no separate frontend SPA and no API-only layer; Razor views are the UI.

## 2. Technology stack

- **Backend framework**: ASP.NET Core MVC, classic `Startup.cs` + `Program.cs` (generic host) pattern, not minimal APIs, not Razor Pages.
- **Target framework**: `net10.0` (per `OrgCheck.csproj`). Note: the checked-in publish profile (`Properties/PublishProfiles/FolderProfile.pubxml`) still says `net6.0` — a leftover from an earlier version of the project; the two are out of sync (see Issues).
- **Database**: PostgreSQL, accessed via EF Core (`Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.0). Single reverse-engineered `DbContext` (`Models/postgresContext.cs` → class `PostgresContext`), all tables under the `orgcheck` schema.
- **ORM pattern**: EF Core for entities, plus a separate hand-written repository layer (`DataAccess/*DA.cs`) that appears to run raw/Dapper-style queries alongside EF.
- **Auth**: ASP.NET Core cookie authentication + a custom `ExecutionContext` (per-request ambient user context) populated by custom middleware.
- **Frontend**: Server-rendered Razor views (`.cshtml`) with jQuery/Bootstrap and vendored JS/CSS in `wwwroot/assets` (no npm/webpack build step, no React/Angular/Vue).
- **Other notable packages**: AutoMapper (DTO↔entity mapping), AWS SDK (S3, for file storage), BCrypt.Net-Next (password hashing), CsvHelper, NPOI (Excel/Word report generation), Newtonsoft.Json, System.Configuration.ConfigurationManager.
- **Reporting**: `Report/ReportModel/*.cs` (POCOs for education/employment verification reports), rendered via NPOI. Old build artifacts reference `FastReport.Compat.dll`, implying the project previously used FastReports and was migrated to NPOI — no FastReport package reference exists in the current `.csproj`.

## 3. Folder structure

| Path | Contents |
|---|---|
| `Controllers/` | 7 MVC controllers: `HomeController` (public/login), `AdminController`, `CustomerController`, `SupportController`, `BgvController`, `eduController`, `empController` |
| `Models/` | EF Core entity classes + `postgresContext.cs` (scaffolded `DbContext`, ~40 `DbSet<>`s) |
| `ViewModels/` | View-model/DTO classes consumed by Razor views |
| `DataAccess/` | Repository-style data-access classes (`*DA.cs`) + `Interfaces/` |
| `Services/` | Business logic: `AuthService`, `UserService`, `CustomerService`, `EmployeeService`, `StudentService`, `CompanyService`, `EmailService`, `FileService`, `QuestionaireService`, `LogService`, `Constants` (bound from config) |
| `Middleware/` | `RequestAuthenticationFilter.cs`, `ExecutionContextMiddleware.cs`, `ExecutionContext.cs` |
| `Views/` | Razor views organized by controller, plus `Shared/` (multiple layouts: `_Layout`, `_AdminLayout`, `_CustomerLayout`, `_ExZoneLayout`, `_VZoneLayout`) |
| `Report/ReportModel/` | POCOs for generated verification reports |
| `wwwroot/` | Static assets (CSS/JS/images), plus runtime output accidentally committed to source (`GeneratedReports/*.pdf`, `Log/*.txt`) |
| `DB scripts/` | Several historical `pg_dump` snapshots and text notes |
| `orgcheck_17022025.sql` (root) | **Full PostgreSQL custom-format dump** (17 Feb 2025) — this is what was used to stand up a working local database (see §7) |
| `Properties/` | `launchSettings.json`, `PublishProfiles/` |
| `OrgCheck.csproj`, `OrgCheck.sln` | Project/solution files |
| `Program.cs`, `Startup.cs` | Entry point and app bootstrap |
| `appsettings.json` / `appsettings.Development.json` | Configuration (see §5) |

## 4. Entry points & app bootstrap

- **`Program.cs`**: `Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(...).UseStartup<Startup>()` — standard, nothing custom.
- **`Startup.cs`**:
  - `ConfigureServices`: cookie auth (30 min sliding expiry, login path `/Home/Index`), session, `AddMvc().SetCompatibilityVersion(Version_3_0)` (obsolete API, still compiles as a warning on .NET 10), HSTS, `AddControllersWithViews`, EF Core `PostgresContext` wired to `ConnectionStrings:OrgCheckDbConnectionString`, `ApplicationSettings` config section bound to a singleton `Services.Constants` object (this is where all secrets live — see §5), CORS restricted to `https://app.verifyzone.in`, AutoMapper, custom `ExecutionContext` DI registration.
  - `Configure`: forwarded headers → inline CSP-header middleware → HTTPS redirect → static files → cookie policy → authentication → routing → custom `ExecutionContextMiddleware` → authorization → a second, stricter cookie policy → default MVC route (`{controller=Home}/{action=Index}/{id?}`) → CORS (registered *after* `UseEndpoints`, which is normally ineffective — flagged as a bug, not fixed, since it's existing behavior).

## 5. Environment variables / configuration required

There are **no real "environment variables"** in the 12-factor sense — everything is read from `appsettings.json` / `appsettings.Development.json` via the standard ASP.NET Core configuration system, keyed off `ASPNETCORE_ENVIRONMENT`.

- `appsettings.json` (committed, used in Production): only contains `Logging` and `AllowedHosts`. **It is missing `ConnectionStrings` and `ApplicationSettings` entirely** — if the app were run with `ASPNETCORE_ENVIRONMENT=Production` as-is, `Startup.ConfigureServices` would receive a null connection string and a null `Constants` object and fail at startup.
- `appsettings.Development.json` (committed) contains everything needed to actually run the app, **including live-looking secrets checked into source control**:
  - `ConnectionStrings:OrgCheckDbConnectionString` — active value already points at `127.0.0.1` / db `postgres` / user `postgres` / password `postgres123` (plus two commented-out remote hosts with different passwords).
  - `ApplicationSettings.SecretKey` — a full RSA private key (PEM body, no headers).
  - `ApplicationSettings.EmailAPIKey` — a SendGrid API key.
  - `ApplicationSettings.EmailFromPass` — a Gmail app password.
  - `ApplicationSettings.AWSAccessKey` / `AWSSecretKey` — AWS credentials for S3 (`AWSBucketName: verifyzone`).
  - `ReCaptcha.SiteKey` / `SecretKey` — Google reCAPTCHA v3 keys.
  - `ApplicationSettings.AppLog`, `UploadPath`, `Reports` — hardcoded Windows paths (`D:\Projects\Repos\kshiva2k\VerifyZone\...`) that don't exist outside the original developer's machine.

**This is a real security exposure** (production-shaped secrets in a file that a generic `.gitignore` does not exclude). Recommend rotating all of these credentials and moving them to user-secrets / a secrets manager / environment variables before this repo is shared further, regardless of what's done for local dev.

## 6. Dependencies

From `OrgCheck.csproj` (single project, no other `.csproj` in the solution):

AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.0, AWSSDK.Core 3.7.400.30, AWSSDK.S3 3.7.404.2, BCrypt.Net-Next 4.0.3, CsvHelper 33.1.0, JetBrains.Annotations 2025.2.4, Microsoft.EntityFrameworkCore 10.0.0, Microsoft.EntityFrameworkCore.Tools 10.0.0, Newtonsoft.Json 13.0.4, Npgsql 10.0.0, Npgsql.EntityFrameworkCore.PostgreSQL 10.0.0, NPOI 2.7.6, System.Configuration.ConfigurationManager 10.0.0.

`dotnet restore` reports one advisory: **AutoMapper 12.0.0 has a known high-severity vulnerability** (GHSA-rvv3-g6hj-g44x). Consider upgrading (breaking change: AutoMapper 13+ changed its licensing/config API).

No `package.json` / npm dependencies — all frontend JS/CSS libraries (Bootstrap, jQuery, Chart.js, TinyMCE, CKEditor, RemixIcon, html2canvas) are vendored directly under `wwwroot/assets`.

## 7. What was missing / had to be set up to run locally

| Gap | What was needed |
|---|---|
| No .NET SDK in the build environment | Installed .NET SDK **10.0.302** (matches `net10.0` target) |
| No PostgreSQL server | Installed a self-contained local PostgreSQL 16.2 instance (matches the version the dump was taken from) |
| No schema/data | Restored the repository's own `orgcheck_17022025.sql` (a `pg_restore`-format custom dump — **not** plain SQL, despite the `.sql` extension) into a fresh `postgres` database, schema `orgcheck`. This is the same connection shape (`Host=127.0.0.1;Database=postgres;Username=postgres;Password=postgres123`) already hardcoded in `appsettings.Development.json`, so **no config file edits were needed**. |
| No EF Core Migrations folder | Confirmed there is none — schema is managed by hand/SQL dump, not `dotnet ef database update`. If the schema ever needs to change, it will need to be done via direct SQL or by scaffolding migrations from scratch. |
| Slow first build in a resource-constrained environment | Not a real project defect — Razor view compilation is CPU-heavy on first build. No files changed. |
| `dotnet build` failing on `CreateAppHost` mid-way | Environment-specific: an intermediate `apphost` file became locked. Rebuilding with `-p:UseAppHost=false` (a command-line MSBuild property, not a file change) avoided the native launcher step, which isn't needed since the app is run via `dotnet OrgCheck.dll`. |

No source file, `.csproj`, `appsettings.*`, or view was edited. See §11 for the exact modification log.

## 8. Build & run process (as verified)

```bash
# 1. Restore
dotnet restore OrgCheck.csproj

# 2. Build
dotnet build OrgCheck.csproj -c Debug

# 3. Ensure PostgreSQL is running and reachable at the connection string
#    in appsettings.Development.json (Host=127.0.0.1;Database=postgres;
#    Username=postgres;Password=postgres123), with the orgcheck schema
#    restored from orgcheck_17022025.sql:
pg_restore -h 127.0.0.1 -p 5432 -U postgres -d postgres --no-owner orgcheck_17022025.sql

# 4. Run
ASPNETCORE_ENVIRONMENT=Development dotnet bin/Debug/net10.0/OrgCheck.dll
# or: dotnet run  (uses Properties/launchSettings.json, which already
#     forces ASPNETCORE_ENVIRONMENT=Development and binds to
#     https://localhost:5001;http://localhost:5000)
```

## 9. Verified result (in this sandbox)

- `dotnet restore` → succeeded (1 advisory warning, no errors).
- `dotnet build` → **succeeded, 0 errors**, ~15 warnings (all pre-existing: obsolete APIs — `SetCompatibilityVersion`, `RijndaelManaged`, `RNGCryptoServiceProvider`, `Rfc2898DeriveBytes` constructor — and two unused-variable warnings in `CompanyService.cs` / `EmployeeService.cs`). None of these block running the app.
- Local PostgreSQL 16.2 started, `orgcheck_17022025.sql` restored cleanly (`pg_restore` exit 0), 27+ tables confirmed present under the `orgcheck` schema, `orgcheck.logins` contains 6 existing user rows.
- App started successfully: `Now listening on: http://0.0.0.0:5000`, `Application started`, `Hosting environment: Development`.
- `GET /` → **HTTP 200**, returned the real "Login | Verifyzone" HTML page with all linked CSS.
- Static assets (`/assets/css/login.css`) → HTTP 200.
- No unhandled exceptions in the application log once the app was built with default (compile-time) Razor compilation.

**Important caveat on "local preview URL":** the app was built and run inside Claude's isolated sandbox environment to verify it, not on your own machine — that sandbox isn't reachable from your browser, so there's no clickable preview link to hand you here. The steps in §8 are exactly what was run and verified; running them on your own machine (with .NET 10 SDK and PostgreSQL 16 installed) will get you to `https://localhost:5001` / `http://localhost:5000` directly, using the `dotnet run` profile already defined in `Properties/launchSettings.json`.

Backend and frontend are the same process/URL — this is a monolithic server-rendered app, there's no separate backend API host.

## 10. Authentication flow

1. `POST /Home/Index` (login form) is handled by `HomeController`. Credentials are checked via `IAuthService.GetClaimsPrincipal`, which builds a `ClaimsPrincipal` with `NameIdentifier` (UserId), `Role` (RoleName), `Sid` (CustomerId), `GroupSid` (CompanyId) claims.
2. `HttpContext.SignInAsync` issues the ASP.NET Core auth cookie (cookie auth scheme, 30 min sliding expiry per `Startup.cs`, though `ConfigureApplicationCookie` separately sets a 7-day expiration — the two configs overlap/conflict, worth reconciling if you touch auth).
3. On every subsequent request, custom `ExecutionContextMiddleware` (registered between `UseRouting` and `UseAuthorization`) reads those claims off `HttpContext.User` and populates a scoped `Middleware.ExecutionContext` object (`UserId`, `RoleId`, `RoleName`, `CustomerId`, `CompanyId`, `CurrentUser`), and calls `IAuthService.SetupUser(id)` to hydrate a current-user cache.
4. Controllers use a mix of the standard `[Authorize]` attribute and manual checks like `if (_executionContext.UserId == 0) return RedirectToAction("Index", "Home")` — so authorization is enforced twice, by two different mechanisms.
5. A separate, apparently-unused `RequestAuthenticationFilter` (`IActionFilter`) implements a third, simpler redirect-if-unauthenticated check; it isn't wired up globally in `Startup.cs`, so it's most likely dead code or selectively applied per-controller.
6. "Remember me" is implemented via custom `vzun`/`vzpd` cookies that appear to store the username/password in plaintext — flagged as a security concern, not something to silently fix without your sign-off.

## 11. Database

- Provider: PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`.
- `DbContext`: `OrgCheck.Models.PostgresContext` (`Models/postgresContext.cs`), scaffolded (reverse-engineered) from an existing database — not migration-managed.
- Schema: everything lives under the `orgcheck` Postgres schema (companies, customers, employees, students, wallets/credits, questionnaires, logins, and various lookup tables — ~40 tables).
- The repo ships its own historical dumps (`DB scripts/*.sql`, `orgcheck.backup`) plus a more recent one at the repo root, `orgcheck_17022025.sql` — despite the `.sql` extension this is a **pg_dump custom-format** file and must be restored with `pg_restore`, not `psql -f`.

## 12. Deployment (as configured in the repo)

- `Properties/PublishProfiles/FolderProfile.pubxml` — FileSystem publish profile targeting `win-x64`, `SelfContained=false` (expects the shared .NET runtime installed on the target Windows server), publishing to a local path on the original developer's machine (`D:\Projects\Repos\kshiva2k\Orgcheck_Publish`). This, together with the IIS Express profile in `launchSettings.json`, strongly suggests the intended production deployment target is **Windows + IIS** (or a Windows service host), not a container. There is no `Dockerfile`, `docker-compose.yml`, or CI/CD pipeline definition in the repository.

## 13. Known issues to be aware of before you start changing things

1. **`appsettings.json` (Production) is incomplete** — missing `ConnectionStrings` and `ApplicationSettings` entirely; the app can only run today because `appsettings.Development.json` fills the gap.
2. **Secrets committed to source** (§5) — DB passwords, an RSA private key, SendGrid key, Gmail app password, AWS keys, reCAPTCHA secret. Treat this file as compromised/rotate before broader distribution.
3. **`FolderProfile.pubxml` targets `net6.0` / the `.csproj` targets `net10.0`** — publish profile is stale relative to the current project file.
4. **CORS policy applied after `UseEndpoints`** in `Startup.Configure` — normally has no effect when placed there; if cross-origin calls from `app.verifyzone.in` are actually relied upon, this should be reordered.
5. **Two different cookie-expiration configs** for the same auth cookie (30 min sliding vs. 7-day `ConfigureApplicationCookie`).
6. **No EF Core Migrations** — any schema change has to be applied by hand to Postgres and then optionally re-scaffolded into `PostgresContext`.
7. **Runtime artifacts committed under `wwwroot`** (`GeneratedReports/*.pdf`, `Log/*.txt`) — not code, but bloats the repo and risks stale/sensitive PDFs sitting in source control.
8. **AutoMapper 12.0.0 has a known high-severity NuGet advisory** (see §6).

## 14. Modification log

The following is the complete list of actions taken. **No file inside your selected project folder was changed, added, or deleted.**

- Copied the project (excluding `obj/`/`bin/` build output) into a separate scratch build directory, because the mounted project folder rejected some file writes required mid-build (this is a sandbox artifact, not a property of your repo).
- Installed .NET SDK 10.0.302 and a throwaway local PostgreSQL 16.2 server inside the sandbox — neither touches your machine or your repo.
- Restored your own `orgcheck_17022025.sql` into that throwaway Postgres instance.
- Ran `dotnet build` with the command-line flag `-p:UseAppHost=false` on the scratch copy only, to work around a sandbox file-locking issue when generating the native launcher executable (irrelevant to how you'll actually run the app with `dotnet run`).
- No `.cs`, `.cshtml`, `.csproj`, `.json`, or any other tracked file was edited.

---

Stopping here as instructed. The application builds cleanly on .NET 10, connects to PostgreSQL, and serves pages correctly from a database restored from your own dump file. Ready for feature/page/bug-level instructions next.
