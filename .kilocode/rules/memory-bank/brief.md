ConnectX - A Customizable Connect Four Game

OVERVIEW
--------
ConnectX is a .NET-based implementation of the classic Connect Four game with
extended customization options. The project provides both console and web
interfaces, allowing players to enjoy the game in their preferred environment.

MAIN OBJECTIVES
---------------
- Deliver a fully functional Connect Four game with configurable rules
- Support multiple user interfaces (Console and Web)
- Enable game persistence through save/load functionality
- Provide flexible board configurations including cylindrical board mode

KEY FEATURES
------------
- Customizable board dimensions (width and height up to 20x20)
- Configurable win condition (number of pieces in a row to win)
- Cylindrical board mode for wrap-around gameplay
- Two-player support with Red and Blue pieces
- Save and load game states
- Configuration management (create, edit, delete game configurations)
- Dual persistence options: SQLite database or JSON file storage

TECHNOLOGIES
------------
- Language: C# (.NET)
- Web Framework: ASP.NET Core Razor Pages
- Database: SQLite with Entity Framework Core
- Architecture: Multi-layer (BLL, DAL, UI separation)
- Console UI: Custom menu system with colored output

PROJECT STRUCTURE
-----------------
- BLL: Business Logic Layer (game rules, configuration, state management)
- DAL: Data Access Layer (repository pattern, EF Core, JSON storage)
- ConsoleApp: Console application entry point and game controller
- ConsoleUI: Console rendering and user interaction
- MenuSystem: Reusable menu navigation system
- WebApp: ASP.NET Core Razor Pages web application

SIGNIFICANCE
------------
This project demonstrates clean architecture principles with proper separation
of concerns, the repository pattern for data access abstraction, and support
for multiple presentation layers sharing the same business logic.