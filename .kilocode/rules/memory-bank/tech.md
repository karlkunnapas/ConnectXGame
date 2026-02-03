# ConnectX - Technology Documentation

## Technology Stack

### Core Framework
- **.NET 9.0** - Target framework for all projects
- **C# (latest)** - Primary programming language
- **Nullable Reference Types** - Enabled project-wide for null safety

## Project Dependencies

### BLL (Business Logic Layer)
```xml
<TargetFramework>net9.0</TargetFramework>
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.10" />
<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="9.0.0" />
```

### DAL (Data Access Layer)
```xml
<TargetFramework>net9.0</TargetFramework>
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.10" />
<ProjectReference Include="..\BLL\BLL.csproj" />
```

### ConsoleApp
```xml
<TargetFramework>net9.0</TargetFramework>
<OutputType>Exe</OutputType>
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.10" />
<ProjectReference Include="..\BLL\BLL.csproj" />
<ProjectReference Include="..\ConsoleUI\ConsoleUI.csproj" />
<ProjectReference Include="..\DAL\DAL.csproj" />
<ProjectReference Include="..\MenuSystem\MenuSystem.csproj" />
```

### ConsoleUI
```xml
<TargetFramework>net9.0</TargetFramework>
<ProjectReference Include="..\BLL\BLL.csproj" />
```

### MenuSystem
```xml
<TargetFramework>net9.0</TargetFramework>
<ProjectReference Include="..\DAL\DAL.csproj" />
```

### WebApp
```xml
<TargetFramework>net9.0</TargetFramework>
<PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="9.0.9" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="9.0.9" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="9.0.9" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.10" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.9" />
<ProjectReference Include="..\DAL\DAL.csproj" />
```

## Database Configuration

### SQLite Connection
- Connection string: `DataSource=<db_file>;Cache=Shared`
- Database location: `~/ConnectX/database/app.db`
- Migrations managed via EF Core

### Entity Framework Core Setup
- [`AppDbContext`](DAL/AppDbContext.cs:6) - Main DbContext
- Custom value converter for jagged arrays: [`ECellStateJaggedArrayConverter`](DAL/GameEntities.cs:55)
- Unique index on Configuration.Name
- Restrict delete behavior for foreign keys

## Development Setup

### Prerequisites
1. .NET 9.0 SDK
2. IDE: Visual Studio, Rider, or VS Code with C# extension

### Running the Console Application
```bash
cd ConsoleApp
dotnet run
```
On startup, choose storage method:
- `j` for JSON file storage
- Any other key for EF Core/SQLite

### Running the Web Application
```bash
cd WebApp
dotnet run
```
Access at: `https://localhost:5001` or `http://localhost:5000`

### Database Migrations
Migrations are stored in `/DAL/Migrations/`:
- `20251028100320_InitialCreate.cs`
- `20251029105426_InitialCreate2.cs`
- `20251029162217_InitialCreate3.cs`
- `20251031090530_InitialCreateFolderChange.cs`
- `20251031090748_InitialCreateFolderChange2.cs`

To apply migrations:
```bash
cd ConsoleApp
dotnet ef database update
```

To create new migration:
```bash
cd ConsoleApp
dotnet ef migrations add MigrationName
```

## Build Configuration

### Directory.Build.props
Global build settings applied to all projects:
```xml
<LangVersion>latest</LangVersion>
<Nullable>enable</Nullable>
<WarningsAsErrors>CS8600,CS8602,CS8603,CS8613,CS8618,CS8625</WarningsAsErrors>
```

Nullable warnings treated as errors:
- CS8600: Converting null literal or possible null value
- CS8602: Dereference of a possibly null reference
- CS8603: Possible null reference return
- CS8613: Nullability of reference types in return type doesn't match
- CS8618: Non-nullable field must contain a non-null value
- CS8625: Cannot convert null literal to non-nullable reference type

## File Storage Paths

All paths are relative to user home directory (`~`):

| Purpose | Path |
|---------|------|
| Configurations | `~/ConnectX/configs/` |
| Saved Games | `~/ConnectX/savegames/` |
| Database | `~/ConnectX/database/` |

Paths are managed by [`FilesystemHelpers`](DAL/FilesystemHelpers.cs:3) class.

## Web Application Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "DataSource=<db_file>;Cache=Shared"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Dependency Injection (WebApp)
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddScoped<IRepository<GameConfiguration>, ConfigRepositoryEf>();
builder.Services.AddScoped<IRepository<GameBrain>, GameRepositoryEf>();
```

## Frontend Technologies (WebApp)

- **ASP.NET Core Razor Pages** - Server-side rendering
- **Bootstrap** - CSS framework (via wwwroot/lib)
- **jQuery** - JavaScript library (via wwwroot/lib)
- **jQuery Validation** - Form validation

## Technical Constraints

1. **Board Size Limits**: 1-20 for both width and height
2. **Win Condition Limits**: 1-20 pieces in a row
3. **Player Name Length**: 1-32 characters (WebApp validation)
4. **Configuration Name Length**: Max 100 characters
5. **Game/Config Name in DB**: Max 255 characters

## Known Technical Patterns

### Array Conversion
2D arrays (`ECellState[,]`) are converted to jagged arrays (`ECellState[][]`) for:
- JSON serialization
- EF Core storage

Conversion methods in [`GameBrain`](BLL/GameBrain.cs:32) and [`GameData`](BLL/GameData.cs:31).

### Win Detection Algorithm
- Checks 4 directions: horizontal, vertical, diagonal-up, diagonal-down
- For each direction, counts consecutive pieces in both directions
- Cylindrical mode uses modulo arithmetic for X-axis wrapping