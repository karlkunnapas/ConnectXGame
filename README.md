# ConnectX (C# / .NET 9, ASP.NET Core Razor Pages, EF Core + SQLite)

## Description

ConnectX is a customizable Connect Four implementation with two frontends:
- a console application
- a web application (Razor Pages)

The game supports configurable board sizes, win conditions, and an optional cylindrical (wrap-around) mode. The WebApp also includes a real-time multiplayer mode implemented with SignalR/WebSockets (see [`GameHub`](WebApp/Hubs/GameHub.cs)). Games and configurations can be persisted either to SQLite (via EF Core) or to JSON files.

### Web application homepage
![Web homepage screenshot](./Homepage.png)

### Web application gameplay page after winning
![Web gameplay screenshot](./Gameplay.png)


## Requirements

- .NET SDK 9.0
- (Optional) Rider / Visual Studio / VS Code with C# extension

## Solution Structure

ConnectX follows a clean multi-layer architecture with clear separation of concerns.

Key projects and entry points:
- Business logic: [`GameBrain`](BLL/GameBrain.cs), configuration: [`GameConfiguration`](BLL/GameConfiguration.cs)
- Data access: repository abstraction [`IRepository<TData>`](DAL/IRepository.cs), EF Core context [`AppDbContext`](DAL/AppDbContext.cs)
- Console app entry point: [`Program`](ConsoleApp/Program.cs)
- Web app entry point: [`Program`](WebApp/Program.cs)

## Running the Application

### Console application

1. From repository root, run:
   ```bash
   cd ConsoleApp
   dotnet run
   ```
2. On startup, choose storage method:
   - Press `j` for JSON file storage
   - Press any other key for EF Core / SQLite storage

### Web application (Razor Pages)

1. From repository root, run:
   ```bash
   cd WebApp
   dotnet run
   ```
2. Open in a browser:
   - `https://localhost:5001` or
   - `http://localhost:5000`

## Storage Locations

All paths are relative to the user home directory (`~`).

### JSON file storage

- Configurations: `~/ConnectX/configs/`
- Saved games: `~/ConnectX/savegames/`

JSON storage helpers: [`FilesystemHelpers`](DAL/FilesystemHelpers.cs) and JSON repositories [`GameRepositoryJson`](DAL/GameRepositoryJson.cs), [`ConfigRepositoryJson`](DAL/ConfigRepositoryJson.cs).

### SQLite database (EF Core)

- Database file: `~/ConnectX/database/app.db`

EF Core context: [`AppDbContext`](DAL/AppDbContext.cs). EF Core repositories [`GameRepositoryEf`](DAL/GameRepositoryEf.cs), [`ConfigRepositoryEf`](DAL/ConfigRepositoryEf.cs). Migrations are in [`DAL/Migrations`](DAL/Migrations/).

## Design Choices

- **Shared business logic**: both UIs use the same core engine ([`GameBrain`](BLL/GameBrain.cs)) to avoid duplicated rules and win-detection logic.
- **Repository abstraction**: persistence is behind [`IRepository<TData>`](DAL/IRepository.cs), allowing storage to be swapped without changing UI code.
- **Strict null-safety**: nullable reference types are enabled and key warnings are treated as errors via [`Directory.Build.props`](Directory.Build.props).

## Technical Highlights

- **Kilo Code usage**: this project is developed with Kilo Code assistance.
- **SignalR + WebSockets (real-time multiplayer)**: WebApp multiplayer is implemented via SignalR hub [`GameHub`](WebApp/Hubs/GameHub.cs).
- **Repository + interface pattern (dual storage)**: persistence is behind [`IRepository<TData>`](DAL/IRepository.cs) with two implementations per entity type:
  - EF Core repositories (SQLite): e.g. [`GameRepositoryEf`](DAL/GameRepositoryEf.cs), [`ConfigRepositoryEf`](DAL/ConfigRepositoryEf.cs)
  - JSON repositories (filesystem): e.g. [`GameRepositoryJson`](DAL/GameRepositoryJson.cs), [`ConfigRepositoryJson`](DAL/ConfigRepositoryJson.cs)
- **DTO usage for persistence**: [`GameData`](BLL/GameData.cs) serializes/deserializes game state and converts between 2D arrays and jagged arrays for JSON/EF compatibility.
- **Layering / clean architecture**: BLL contains rules and state, DAL contains persistence, and UIs focus on interaction and rendering.
- **Bootstrap UI (WebApp)**: Web UI is built with Bootstrap.
