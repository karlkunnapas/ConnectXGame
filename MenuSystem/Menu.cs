namespace MenuSystem;

public class Menu
{
    private Dictionary<string, MenuItem> MenuItems { get; set; } = new();
    private Dictionary<string, MenuItem> StaticMenuItems { get; set; } = new();
    public EMenuLevel Level { get; set; }
    private string Title { get; set; } = default!;

    public Menu(string title, EMenuLevel level)
    {
        Title = title;
        Level = level;

        switch (level)
        {
            case EMenuLevel.Root:
                StaticMenuItems["x"] = new MenuItem() { Key = "x", Value = "Exit" };
                break;
            case EMenuLevel.Second:
                StaticMenuItems["m"] = new MenuItem() { Key = "m", Value = "Return to Main Menu" };
                StaticMenuItems["x"] = new MenuItem() { Key = "x", Value = "Exit" };
                break;
            case EMenuLevel.Deep:
                StaticMenuItems["b"] = new MenuItem() { Key = "b", Value = "Back to previous Menu" };
                StaticMenuItems["m"] = new MenuItem() { Key = "m", Value = "Return to Main Menu" };
                StaticMenuItems["x"] = new MenuItem() { Key = "x", Value = "Exit" };
                break;
                
        }
    }
    
    
    public void AddMenuItem(string key, string value, Func<string> methodToRun)
    {
        if (MenuItems.ContainsKey(key))
        {
            throw new ArgumentException("This key already exists!");
        }

        MenuItems[key] = new MenuItem() {Key = key, Value = value, MethodToRun = methodToRun};
    }

        
    public string Run()
    {
        Console.Clear();
        var menuRunning = true;
        var userChoice = "";
        do
        {
            DisplayMenu();
            Console.Write("Select an option: ");
            var input = Console.ReadLine();
            // validate and act on input

            if (input == null)
            {
                Console.WriteLine("Invalid input. Please try again.");
                continue;
            }
            userChoice = input!.ToLower();
            if (userChoice == "x" || userChoice == "m" || userChoice == "b")
            {
                menuRunning = false;
            }
            else
            {
                if (MenuItems.ContainsKey(userChoice))
                {
                    var returnValueFromMethod = MenuItems[userChoice].MethodToRun?.Invoke();
                    if (returnValueFromMethod == "x")
                    {
                        menuRunning = false;
                        userChoice = "x";
                    }
                    else if (returnValueFromMethod == "m" && Level != EMenuLevel.Root)
                    {
                        menuRunning = false;
                        userChoice = "m";
                    }

                    // exit, return to main menu, or back from submenu return
                }
                else
                {
                    Console.WriteLine("Invalid option. Please try again.");
                }
            }
        } while (menuRunning);
        Console.Clear();

        return userChoice; // return choice to caller
    }
    
    
    private void DisplayMenu()
    {
        Console.WriteLine(Title);
        Console.WriteLine("--------------------");
        foreach (var menuItem in MenuItems.Values)
        {
            Console.WriteLine(menuItem);
        }
        foreach (var staticMenuItem in StaticMenuItems.Values)
        {
            Console.WriteLine(staticMenuItem);
        }
    }
}