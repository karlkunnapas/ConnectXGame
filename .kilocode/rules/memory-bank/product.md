# ConnectX - Product Documentation

## Why This Project Exists

ConnectX is an educational/demonstration project that implements a customizable version of the classic Connect Four game. It showcases clean architecture principles, separation of concerns, and the ability to support multiple user interfaces (Console and Web) sharing the same business logic.

## Problems It Solves

1. **Customizable Gameplay**: Unlike standard Connect Four (7x6 board, 4-in-a-row), ConnectX allows players to configure:
   - Board dimensions (1-20 for both width and height)
   - Win condition (number of pieces in a row needed to win)
   - Cylindrical board mode (wrap-around gameplay)

2. **Game Persistence**: Players can save and resume games at any point, with support for both database (SQLite) and file-based (JSON) storage.

3. **Configuration Management**: Users can create, save, edit, and delete game configurations for reuse.

4. **Multi-Platform Access**: The same game can be played via:
   - Console application with colored ASCII board rendering
   - Web application with interactive HTML tables

## How It Works

### Game Flow

1. **Configuration Selection**: User selects or creates a game configuration
2. **Player Setup**: Enter names for Player 1 (Red) and Player 2 (Blue)
3. **Gameplay**: Players take turns dropping pieces into columns
4. **Win Detection**: Game checks for winning condition after each move
5. **Save/Load**: Games can be saved at any point and resumed later

### User Experience Goals

#### Console Application
- Clear, colored ASCII board display
- Intuitive menu navigation with keyboard shortcuts
- Support for both standard and cylindrical board visualization
- Red and Blue colored pieces for easy identification

#### Web Application
- Clean, responsive Bootstrap-based interface
- Click-to-play interaction on board cells
- Easy navigation between game management pages
- Form validation for configuration inputs

## Key User Workflows

### Starting a New Game
1. Navigate to "New Game"
2. Select a configuration from dropdown
3. Enter game name and player names
4. Click "Start" to begin playing

### Managing Configurations
1. Navigate to "Configurations"
2. Create new configuration with custom settings
3. Edit existing configurations
4. Delete unwanted configurations

### Saving and Loading Games
1. During gameplay, exit to save (Console) or game auto-saves (Web)
2. Navigate to "Saved Games"
3. Select a game to continue or delete

## Game Rules

- Two players: Red (Player 1) and Blue (Player 2)
- Red always moves first
- Pieces drop to the lowest available position in a column
- Win by connecting the required number of pieces (default: 4)
- Connections can be horizontal, vertical, or diagonal
- In cylindrical mode, horizontal connections can wrap around the board edges