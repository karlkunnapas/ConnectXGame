using BLL;
using DAL;
using MenuSystem;

namespace ConsoleApp;

public static class MenuFactory
{
    public static void CreateMenu(IRepository<GameConfiguration> configRepo, IRepository<GameBrain> gameRepo)
    {
        var menu0 = new Menu("CONNECTX Main Menu", EMenuLevel.Root);
        var config = new GameConfiguration();

        menu0.AddMenuItem("n", "New game", () =>
        {
            Console.Write("Enter first player name, leave empty for default 'Player 1': ");
            var nameChoice1 = Console.ReadLine();
                
            Console.Write("Enter second player name, leave empty for default 'Player 2': ");
            var nameChoice2 = Console.ReadLine();
            
            var controller = new GameController(config, nameChoice1, nameChoice2);
            GameLoop(controller);
            Console.WriteLine();
            return "abc";

        });

        // ----------------SAVED GAMES----------------
        
        var savedGames = new Menu("ConnectX Saved Games", EMenuLevel.Second);

        savedGames.AddMenuItem("l", "Load", () =>
        {
            bool gameLoaded = false;
            
            Console.Clear();
            do
            {
                var data = gameRepo.List();
                foreach (var (id, (gameGuid, gameName)) in data)
                {
                    Console.WriteLine(id+ ": " + gameName);
                }
                Console.Write("Select game to load, 0 to skip:");
                
                var userChoice = Console.ReadLine();
                if (userChoice == "0")
                {
                    Console.Clear();
                    break;
                } 
                else if (int.TryParse(userChoice, out var gameId) && gameId <= data.Count && gameId > 0)
                {
                    var savedConfig = gameRepo.Load(data[gameId].Item1);
                    var controller = new GameController(savedConfig);
                    GameLoop(controller);
                    gameLoaded = true;
                } else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please try again.");
                }
            } while (!gameLoaded);
            Console.WriteLine();
            return "";
        });

        savedGames.AddMenuItem("d", "Delete", () =>
        {
            bool gameDeleted = false;
            do
            {
                Console.Clear();
                var data = gameRepo.List();
                foreach (var (id, (gameGuid, gameName)) in data)
                {
                    Console.WriteLine(id + ": " + gameName);
                }
                Console.Write("Select saved game to delete, 0 to skip:");
                
                var userChoice = Console.ReadLine();
                if (userChoice == "0")
                {
                    Console.Clear();
                    break;
                } 
                else if (int.TryParse(userChoice, out var gameId) && gameId <= data.Count && gameId > 0)
                {
                    gameRepo.Delete(data[gameId].Item1);
                    gameDeleted = true;
                } else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please try again.");
                }
                
            } while (!gameDeleted);
            Console.Clear();
            Console.WriteLine("Saved game deleted.");
            return "";
        });
        menu0.AddMenuItem("s", "Saved Games", savedGames.Run);

        
        // ----------------CONFIGURATIONS----------------
        
        var menuConfig = new Menu("ConnectX Configurations", EMenuLevel.Second);
        menuConfig.AddMenuItem("s", "Save", ()  =>
        {
            configRepo.Save(config);
            Console.Clear();
            Console.WriteLine("Config saved.");
            return "";
        });
        menuConfig.AddMenuItem("l", "Load", () =>
        {
            bool configLoaded = false;
            
            do
            {
                Console.Clear();
                var data = configRepo.List();
                foreach (var (id, (configGuid, configName)) in data)
                {
                    Console.WriteLine(id+ ": " + configName);
                }
                Console.Write("Select config to load, 0 to skip:");
                
                var userChoice = Console.ReadLine();
                if (userChoice == "0")
                {
                    Console.Clear();
                    break;
                } else if (int.TryParse(userChoice, out var configId) && configId <= data.Count && configId > 0)
                {
                    config = configRepo.Load(data[configId].Item1);
                    configLoaded = true;
                } else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please try again.");
                }
            } while (!configLoaded);
            Console.Clear();
            Console.WriteLine("Config loaded.");
            return "";
        });
        menuConfig.AddMenuItem("e", "Edit", ()  =>
        {
            GameConfigEditWithoutName(config);
            configRepo.Save(config);
            Console.Clear();
            Console.WriteLine("Config edited and saved.");
            return "";
        });
        menuConfig.AddMenuItem("c", "Create", () =>
        {
            var newConfig = new GameConfiguration();
            GameConfigEdit(newConfig);
            configRepo.Save(newConfig);
            Console.Clear();
            Console.WriteLine("Config created.");
            return "";
        });

        menuConfig.AddMenuItem("d", "Delete", () =>
        {
            bool configDeleted = false;
            do
            {
                Console.Clear();
                var data = configRepo.List();
                foreach (var (id, (configGuid, configName)) in data)
                {
                    Console.WriteLine(id+ ": " + configName);
                }
                Console.Write("Select config to delete, 0 to skip:");
                
                var userChoice = Console.ReadLine();
                if (userChoice == "0")
                {
                    Console.Clear();
                    break;
                } else if (int.TryParse(userChoice, out var configId) && configId <= data.Count && configId > 0)
                {
                    configRepo.Delete(data[configId].Item1);
                    configDeleted = true;
                } else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input. Please try again.");
                }
            } while (!configDeleted);
            Console.Clear();
            Console.WriteLine("Config deleted.");
            return "";
        });

        menu0.AddMenuItem("c", "Game Configurations", menuConfig.Run);


        menu0.Run();

        Console.WriteLine("Exited");


        void GameLoop(GameController controller)
        {
            var gameOver = false;
            Console.Clear();

            controller.DrawInitialBoard();
            do
            {
                if (controller.GetGameBrain().GetPlayer1Type() == EPlayerType.Ai ||
                    controller.GetGameBrain().GetPlayer2Type() == EPlayerType.Ai)
                {
                    if ((controller.GetGameBrain().GetPlayer1Type() == EPlayerType.Ai && controller.GetGameBrain().IsNextPlayerX()) || (controller.GetGameBrain().GetPlayer2Type() == EPlayerType.Ai && !controller.GetGameBrain().IsNextPlayerX()))
                    {
                        // AI makes a move
                        var ai = new AI();
                        var bestMove = ai.FindBestMove(controller.GetGameBrain());
                        
                        Thread.Sleep(1000);
                        
                        try
                        {
                            var returned = controller.GameLoop((bestMove + 1).ToString()); // Convert to 1-based for input
                            if (returned == "W")
                            {
                                break;
                            }
                            else if (returned == "D")
                            {
                                break;
                            }
                            continue;
                        }
                        catch (ArgumentException e)
                        {
                            // Check if the board is full (draw condition)
                            if (controller.GetGameBrain().IsDraw())
                            {
                                Console.Clear();
                                controller.DrawInitialBoard();
                                Console.WriteLine("Game ended in a draw!");
                                break;
                            }
                            else
                            {
                                Console.Clear();
                                controller.DrawInitialBoard();
                                Console.WriteLine("Invalid input. Please try again.");
                                continue;
                            }
                        }
                    }
                    
                }
                
                Console.Write("Choice (x):");
                var input = Console.ReadLine();
                if (input?.ToLower() == "x")
                {
                    gameOver = true;
                    Console.Write("Save game? (y/n): ");
                    var save = Console.ReadLine();
                    if (save?.ToLower() == "y")
                    {
                        var nameChoice = "";
                        do
                        {
                            if (controller.GetGameBrain().GetName() == null)
                            {
                                Console.Write("Enter name for saved game (do not leave empty!): ");
                                nameChoice = Console.ReadLine();
                            }
                            else
                            {
                                Console.Write("Enter name for saved game or leave empty to use current name: ");
                                nameChoice = Console.ReadLine();
                
                                if (nameChoice == "")
                                {
                                    nameChoice = controller.GetGameBrain().GetName();
                                }
                            }
            
                        } while (nameChoice == "");
        
                        controller.GetGameBrain().SetName(nameChoice);
                        
                        gameRepo.Save(controller.GetGameBrain());
                        Console.Clear();
                        Console.WriteLine("Game saved.");
                        break;
                    }
                    Console.Clear();
                    break;
                    
                }
                if (input == null) continue;
                try
                {
                    var returned = controller.GameLoop(input);
                    if (returned == "W")
                    {
                        break;
                    }
                    else if (returned == "D")
                    {
                        break;
                    }
                }
                catch (ArgumentException e)
                {
                    // Check if the board is full (draw condition)
                    if (controller.GetGameBrain().IsDraw())
                    {
                        Console.Clear();
                        controller.DrawInitialBoard();
                        Console.WriteLine("Game ended in a draw!");
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        controller.DrawInitialBoard();
                        Console.WriteLine("Invalid input. Please try again.");
                    }
                }
                
            } while (!gameOver);
        }
        
        void GameConfigEdit(GameConfiguration configuration)
        {
            bool configEdited = false;
            do
            {
                var nameChoice = "";
                do
                {
                    Console.Write("Enter configuration name (do not leave empty!): ");
                    nameChoice = Console.ReadLine();
                } while (nameChoice == "");
                
                Console.Write("Enter board width (int, max 20), leave empty for default '7': ");
                var widthChoice = Console.ReadLine();
                
                Console.Write("Enter board height (int, max 20), leave empty for default '6': ");
                var heightChoice = Console.ReadLine();
                
                Console.Write("Enter win condition (int), leave empty for default '4': ");
                var winChoice = Console.ReadLine();
                
                Console.Write("Is the board cylindrical? (y/n), leave empty for default 'no': ");
                var cylindricalChoice = Console.ReadLine();
                
                Console.Write("Player 1 type (1 for human, 2 for AI), leave empty for default 'human'): ");
                var player1Choice = Console.ReadLine();
                
                Console.Write("Player 2 type (1 for human, 2 for AI), leave empty for default 'human'): ");
                var player2Choice = Console.ReadLine();
                
                if (configuration.GameConfigEdit(nameChoice, widthChoice, heightChoice, winChoice, cylindricalChoice, player1Choice, player2Choice))
                {
                    configEdited = true;
                } else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input/inputs. Please try again.");
                }
            } while (!configEdited);
        }

        void GameConfigEditWithoutName(GameConfiguration configuration)
        {
            bool configEdited = false;
            do
            {
                Console.Write("Enter board width (int), leave empty for default '7': ");
                var widthChoice = Console.ReadLine();
                    
                Console.Write("Enter board height (int), leave empty for default '6': ");
                var heightChoice = Console.ReadLine();
                    
                Console.Write("Enter win condition (int), leave empty for default '4': ");
                var winChoice = Console.ReadLine();
                    
                Console.Write("Is the board cylindrical? (y/n), leave empty for default 'no': ");
                var cylindricalChoice = Console.ReadLine();
                
                Console.Write("Player 1 type (1 for human, 2 for AI), leave empty for default 'human'): ");
                var player1Choice = Console.ReadLine();
                
                Console.Write("Player 2 type (1 for human, 2 for AI), leave empty for default 'human'): ");
                var player2Choice = Console.ReadLine();
                    
                if (configuration.GameConfigEditWithoutName(widthChoice, heightChoice, winChoice, cylindricalChoice, player1Choice, player2Choice))
                {
                    configEdited = true;
                } else
                {
                    Console.Clear();
                    Console.WriteLine("Invalid input/inputs. Please try again.");
                }
            } while (!configEdited);
        }

    }
}