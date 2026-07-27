# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A full-stack freelancer marketplace: ASP.NET 8 Web API + Angular 18 frontend, with SignalR real-time chat/presence, JWT auth, and role-based access (Admin / Client / Freelancer). All code lives under `FreelancerApp/`.

A second layer sits on top of the marketplace domain: a **performance/observability sandbox** (Serilog, OpenTelemetry+Jaeger, Redis, PerfView, BenchmarkDotNet, deliberately-buggy/slow endpoints) used for practicing debugging, tracing, and performance-tuning workflows. When touching `Sandbox/`, `*Sandbox*Controller.cs`, `BuggyController.cs`, or `Benchmarks/`, assume the "bug" is often intentional — don't reflexively "fix" it without checking whether it's the point of that scenario.

## Solution layout

`FreelancerApp/FreelancerApp.sln` contains:
- `API` — ASP.NET 8 Web API (the real app + the sandbox)
- `client/` — Angular 18 app (not in the .sln; separate npm project)
- `UnitTestsProject` — xUnit + Moq + FluentAssertions, EF Core InMemory, targets `net10.0`
- `IntegrationTestsProject` — xUnit + `WebApplicationFactory<Program>`, SQLite in-memory DB
- `Benchmarks` — BenchmarkDotNet project, targets `net10.0`
- `API.UnitTests` — present in the tree but currently empty; `UnitTestsProject` is the real unit test project

Note the target-framework mismatch: `API` is `net8.0`; `UnitTestsProject`, `Benchmarks` are `net10.0`. This is intentional/existing, not a bug to "fix" incidentally.

## Common commands

Run all commands from `FreelancerApp/` unless noted.

### API
```bash
cd API
dotnet restore
dotnet ef database update   # applies EF Core migrations
dotnet run                  # https://localhost:5001
```

### Angular client
```bash
cd client
npm install
ng serve --open              # http://localhost:4200
ng build
npm test                     # Karma/Jasmine, launches Chrome
```

### Tests
```bash
dotnet test UnitTestsProject
dotnet test IntegrationTestsProject
dotnet test UnitTestsProject --filter FullyQualifiedName~TokenServiceTests   # single test class
dotnet test UnitTestsProject --filter FullyQualifiedName~TokenServiceTests.MethodName  # single test
```

### Benchmarks
```bash
cd Benchmarks
dotnet run -c Release
```

### Observability stack (Jaeger + OTel Collector)
```bash
cd API/docker-compose
docker compose up
```
Jaeger UI: http://localhost:16686. The API exports OTLP to `http://localhost:4319` (collector's mapped gRPC port), which forwards to Jaeger. Serilog also writes to `logs/app-.txt` and to Seq at `http://localhost:5341` (must be running separately if you want Seq output — it isn't part of the docker-compose file above).

### ASP.NET environments
Controlled via `ASPNETCORE_ENVIRONMENT`. Three meaningful values, each with different seeding behavior in `Program.cs`:
- `Development` — resets `Connections` table, runs `Seed.SeedUsers` (real marketplace seed data from `Data/UserSeedData.json` / `SkillSeedData.json`)
- `Sandbox` — full DB reset (`SandboxDatabaseReset`) + `SandboxSeeder`, with a fixed Bogus random seed (`12345`) for reproducible fake data. Uses `appsettings.Sandbox.json` (points at `FreelancerMarketplaceSandboxDb`, disables EF SQL logging)
- `Testing` — used by `CustomWebApplicationFactory`; skips migrations/seeding in `Program.cs`, uses an in-memory SQLite connection instead

Default `DefaultConnection` is SQL Server (`opt.UseSqlServer` in `ApplicationServiceExtensions`), not the `freelancer.db` SQLite file sitting in `API/` (that appears to be a leftover from an earlier SQLite-based setup — the README's "SQLite" mention is stale relative to current `Program.cs`).

### Fresh-clone config gotcha
`API/appsettings.json` is gitignored (`# Ignore appsettings.json`) and won't exist after a fresh clone. `Development` still boots because `appsettings.Development.json` supplies its own dev `TokenKey` (`IdentityServiceExtensions` throws at startup if no `TokenKey` is resolvable) — but `PhotosController`'s Cloudinary upload will fail without a local `appsettings.json` providing a `CloudinarySettings` section (`CloudName`/`ApiKey`/`ApiSecret`), since that section isn't duplicated anywhere tracked.

### EF Core migrations
```bash
cd API
dotnet ef migrations add <Name>
dotnet ef database update
```

## Architecture

### API request pipeline (`Program.cs`)
Order matters here: `RequestTimingMiddleware` → correlation-ID middleware (pushes `CorrelationId` onto the Serilog `LogContext`) → `UseSerilogRequestLogging` → forwarded headers → HTTPS redirect → `ExceptionMiddleware` (global error handler) → `RequestLoggingMiddleware` → CORS (locked to `localhost:4200`) → AuthN/AuthZ → Swagger → controllers/SignalR hubs.

Two Swagger docs are served: `v1` (real API, filtered to controllers in the `v1` API-version group) and `GeneralBugFixingSandbox` (only `GeneralBugFixingSandboxController` actions) — see the `DocInclusionPredicate` in `Program.cs`.

### Data layer
Repository + Unit-of-Work pattern: `IUserRepository`, `IProjectRepository`, `IProposalRepository`, `IPortfolioItemRepository`, `IMessageRepository`, all composed through `IUnitOfWork` (`Data/UnitOfWork.cs`). Controllers depend on `IUnitOfWork`/repositories, not `DataContext`, directly (except sandbox/debug controllers, which intentionally touch `DataContext` directly to demonstrate raw EF behavior). AutoMapper profiles live in `Helpers/AutoMapperProfiles.cs`. Pagination uses `Helpers/PagedList.cs` + per-entity `*Params` classes + `PaginationHeader`.

`ApplicationServiceExtensions.AddApplicationServices` wires up EF Core with a custom `LogTo` hook that detects slow queries (>100ms) and per-request query counts via `HttpContext.Items["QueryCount"]` — useful when diagnosing N+1s surfaced by the sandbox scenarios.

### Real-time (SignalR)
`SignalR/MessageHub` and `SignalR/PresenceHub`, mapped at `hubs/message` and `hubs/presence`. `PresenceTracker` is a singleton tracking online users for the admin panel's live indicator.

### Sandbox subsystem (`API/Sandbox/`)
- `Generators/` — Bogus-based fake data generators (users, projects, proposals)
- `Scenarios/` — named performance-bug scenarios (`NPlusOneScenario`, `SlowDashboardScenario`, `HeavyProposalScenario`, `HotClientScenario`) invoked to reproduce specific perf problems
- `Seeders/SandboxSeeder` + `Infrastructure/SandboxDatabaseReset` — full reset/reseed pipeline used only in the `Sandbox` environment
- Corresponding controllers (`PerformanceSandbox*Controller`, `MemorySandboxController`, `SqlBulkSeedingController`, `BuggyController`, `GeneralBuxFixingSandboxController`) expose these scenarios as HTTP endpoints for manual/tool-driven investigation (Jaeger traces, PerfView captures, dotnet-trace, etc.)
- Redis (`AddStackExchangeRedisCache`, registered twice — once in `Program.cs`, once in `ApplicationServiceExtensions`) is only actually injected (`IDistributedCache`) in `PerformanceSandbox6/7/8Controller`. It's not required to run the core marketplace app, only to exercise those specific endpoints.

### Angular client (`client/src/app`)
Standalone components (no NgModules), organized by feature rather than by layer:
- Role-specific nav bars: `nav/{adminnav,clientnav,freelancernav,publicnav}`
- Shared cross-cutting folders: `_services` (HTTP/domain services), `_guards` (route guards — unsaved changes, disabled-user), `_interceptors`/`interceptors` (JWT attach, error handling, loading spinner), `_models`/`_interfaces`/`_DTOs` (TypeScript types mirroring API DTOs), `_pipes` (e.g. time-ago), `_helpers`
- Feature components map directly to marketplace features: `portfolio-*` (freelancer portfolio CRUD), `profile-project-*`/`project-create`/`browse-projects` (client project CRUD + freelancer browsing), `create-proposal`/`submitted-proposals`/`proposal-inbox` (proposal workflow), `active-projects`, `messages`/`chat` (SignalR messaging), `admin-panel` (user management + presence)
- `environments/environment.ts` (dev) vs `environment.development.ts` — dev API base is `https://localhost:5001/api/`, hub base `https://localhost:5001/hubs/`; production build uses relative `api/`/`hubs/` (assumes same-origin deployment)

### Auth
JWT bearer tokens (`Services/TokenService`, configured in `Extensions/IdentityServiceExtensions.cs`), ASP.NET Identity (`AppUser`/`AppRole`), role-based `[Authorize]` on controllers/actions. Disabled user accounts are enforced both server-side and via an Angular route guard.
