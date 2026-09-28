# Work Items

A small work-tracking application built with ASP.NET Core 8, Angular 20, EF Core, and SQL Server. Data is stored in SQL Server and survives application restarts.

## Prerequisites

- .NET SDK 8+
- Node.js 20+
- SQL Server (the default configuration uses the local default instance)

## Run locally

```powershell
dotnet restore
dotnet run --project src/WorkItems.Api --urls http://127.0.0.1:5062
```

In another terminal:

```powershell
cd src/work-items-ui
npm install
npm start
```

Open `http://localhost:4200`. The API applies its included EF Core migration at startup. To use another SQL Server, override `ConnectionStrings__WorkItems`.

## Test and build

```powershell
dotnet test
cd src/work-items-ui
npm test
npm run build
```

The API returns validation problems as 400, missing items as 404, and invalid state transitions as 409. Search uses RxJS `switchMap`, which cancels the previous HTTP subscription whenever filters or pagination change, preventing stale responses from rendering.
