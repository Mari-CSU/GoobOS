using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static GoobOS.User;
using Sys = Cosmos.System;

namespace GoobOS
{
    public class Kernel : Sys.Kernel
    {
        //Various helper systems and managers.
        public UserManager userManager;
        public GoobTimerSystem timerSystem;

        //Console input was rewired to be manually handled, so we need to store the current input and prompt.
        //This is used to allow for features like the timer command, which needs to print updates to the console without disrupting the current input.
        //The system was designed to mimic Unity's Time system, allowing tasks to be scheduled in advanced. 
        public string currentInput = "";
        public DateTime lastUpdateTime;
        public int deltaMilliseconds;
        public string currentPrompt = "";
        public int maxInputLength = 80;

        //Used to lock the console during certain operations, such as the login screen, to prevent user input.
        public bool ConsoleIsLocked = false;

        //Cosmo's file system
        public CosmosVFS fs;
        //Own file system built on top of Cosmo's
        public GoobFileManager fileManager;

        //Default directory.
        public string currentDirectory = @"0:\";

        //Command list
        private string[] commands = new string[]
        {
            "help | Displays all commands.",
            "clear | Clears console window.",
            "about | Displays information about the OS.",
            "echo <message> | Echoes the next message entered.",
            "restart | Restarts the OS.",
            "shutdown | Shuts down the OS.",
            "login | Return to login screen",
            "logout | Logout User",
            "whoami | Displays information about the current user.",
            "createuser <username> <password> <user/admin> | Creates a new user. Only admins can create new users.",
            "timer <seconds> | Sets a timer for X seconds.",
            "listusers | Lists all users of the OS.",
            "listfiles | Lists all files within the current directory.",
            "readall | Reads all files within the current directory.",
            "read <file_name> | Reads a specific file.",
            "mkdir <directory_name> | Makes a new directory with the inputted name.",
           "listdir | Lists the current directory.",
           "createfile <file_name> | Creates a new file with the given name.",
           "deletefile <file_name> | Deletes specified file.",
            "deletedir | Deletes current directory (Warning unstable, admin only)",
            "writefile <file_name> <text> | Writes text to a specified file.",
            "movefile <file_name> <new_path> | Moves a file to a new path.",
            "movedir <directory_name> | Moves into specified directory.",
            "rootdir | Returns to root directory."
        };

        //Before Run is called before the main loop starts, and is used to initialize various systems and display the login screen.
        protected override void BeforeRun()
        {
            //Here we initialize the file system, user manager, and timer system, as well as display the login screen and print the initial prompt.
            fs = new CosmosVFS();
            VFSManager.RegisterVFS(fs);

            fileManager = new GoobFileManager();
            fileManager.kernel = this;

            userManager = new UserManager();
            timerSystem = new GoobTimerSystem();

            lastUpdateTime = DateTime.Now;

            DisplayLogin();

            PrintPrompt();
        }

        //Run is called every frame, and is used to update the timer system and handle console input.
        protected override void Run()
        {
            UpdateDeltaTime();

            timerSystem.Update(deltaMilliseconds);

            UpdateShellInput();
        }

        //Delta time is always being called and updated every frame, allowing it to be used for things like the timer command, which relies on accurate timing to function properly.
        private void UpdateDeltaTime()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan deltaTime = currentTime - lastUpdateTime;
            lastUpdateTime = currentTime;

            deltaMilliseconds = (int)deltaTime.TotalMilliseconds;

            if (deltaMilliseconds < 0)
            {
                deltaMilliseconds = 0;
            }

            
            if (deltaMilliseconds > 1000)
            {
                deltaMilliseconds = 1000;
            }
        }

        //Shell input is handled manually and must be designed to handle special cases and keys.
        private void UpdateShellInput()
        {
            if (ConsoleIsLocked)
            {
                return;
            }

            if (!Console.KeyAvailable)
            {
                return;
            }

            ConsoleKeyInfo key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();

                string command = currentInput.Trim();
                currentInput = "";

                if (command.Length > 0)
                {
                    HandleCommand(command);
                }

                PrintPrompt();
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                HandleBackspace();
            }
            else
            {
                HandleTypedCharacter(key.KeyChar);
            }
        }

        //Here a backspace is handled by removing the last character from the current input and updating the console display accordingly.
        private void HandleBackspace()
        {
            if (currentInput.Length <= 0)
            {
                return;
            }

            currentInput = currentInput.Substring(0, currentInput.Length - 1);

            int left = Console.CursorLeft;
            int top = Console.CursorTop;

            if (left > currentPrompt.Length)
            {
                Console.SetCursorPosition(left - 1, top);
                Console.Write(" ");
                Console.SetCursorPosition(left - 1, top);
            }
        }

        //Here normal text is handled by adding the character to the current input and displaying it on the console, while also ensuring that non-printing characters are ignored and that the input does not exceed the maximum length.
        private void HandleTypedCharacter(char character)
        {
            // Ignore non-printing characters.
            if (character == '\0')
            {
                return;
            }

            // Prevent long commands from wrapping to the next line.
            if (currentInput.Length >= maxInputLength)
            {
                return;
            }

            currentInput += character;
            Console.Write(character);
        }

        //Here all the commands are handled and various helper method are called to keep the code clean. 
        private void HandleCommand(string input)
        {
            //Inputs are split into parts to allows for commands with arguments.
            string[] parts = input.Split(' ');

            if (parts.Length == 0)
            {
                return;
            }

            string command = parts[0].ToLower();

            switch (command)
            {
                case "help":
                    PrintCommands();
                    break;

                case "clear":
                    Console.Clear();
                    break;

                case "about":
                    print("I don't really have anymore info to give so hey!");
                    break;

                case "echo":
                    HandleEcho(parts);
                    break;

                case "restart":
                    print("Restarting...");
                    Sys.Power.Reboot();
                    break;

                case "shutdown":
                    print("Shutting down...");
                    Sys.Power.Shutdown();
                    break;

                case "login":
                    DisplayLogin();
                    break;

                case "logout":
                    userManager.Logout();
                    print("Logged out.");
                    break;

                case "whoami":
                    HandleWhoAmI();
                    break;

                case "createuser":
                    HandleCreateUserCommand(parts);
                    break;

                case "timer":
                    HandleTimerCommand(parts);
                    break;

                case "listusers":
                    HandleListUsers();
                    break;

                    //Various file functions are handled outside the Kernel class. This was just to organization on my end. 
                case "listfiles":
                    fileManager.HandleListFiles();
                    break;

                case "readall":
                    fileManager.HandleReadAll();
                    break;
               

                case "read":
                    fileManager.HandleRead(parts);
                    break;

                case "mkdir":
                    fileManager.HandleMkDir(parts);
                    break;

                case "listdir":
                    print(currentDirectory);
                    break;

                case "createfile":
                    fileManager.HandleCreateFile(parts);
                    break;

                case "deletefile":
                    fileManager.HandleDeleteFile(parts);
                    break;


                case "deletedir":
                    fileManager.HandleDeleteCurrentDirectory();
                    break;

                case "writefile":
                    fileManager.HandleWriteFile(parts);
                    break;

                case "movefile":
                    fileManager.HandleMoveFile(parts);
                    break;

                case "movedir":
                    fileManager.HandleMoveDir(parts);
                    break;

                case "rootdir":
                    fileManager.HandleRootDir(parts);
                    break;

                default:
                    print("Unknown command. Type 'help' for a list of commands.");
                    break;
            }
        }

        //Here various parts of a string are joined together from a specified index, allowing for commands with multiple arguments to be handled more easily.

        public string JoinParts(string[] parts, int startIndex)
        {
            string result = "";

            for (int i = startIndex; i < parts.Length; i++)
            {
                result += parts[i];

                if (i < parts.Length - 1)
                {
                    result += " ";
                }
            }

            return result;
        }

        private void HandleListUsers()
        {
            foreach (var user in userManager.users)
            {
                print($"Name:{user.Username}\nRole:{GetRoleName(user)}\n", false);
            }
        }

        //Echo statement is handle slightly differently than other commands, as it needs to join all the parts of the input together after the command itself to allow for multi-word messages to be echoed properly.
        private void HandleEcho(string[] parts)
        {
            if (parts.Length < 2)
            {
                print("Usage: echo <message>");
                return;
            }

            string message = "";

            for (int i = 1; i < parts.Length; i++)
            {
                message += parts[i];

                if (i < parts.Length - 1)
                {
                    message += " ";
                }
            }

            print(message);
        }

        private void HandleTimerCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                print("Usage: timer <seconds>");
                return;
            }

            int seconds;

            if (!int.TryParse(parts[1], out seconds))
            {
                print("Invalid number. Usage: timer <seconds>");
                return;
            }

            if (seconds <= 0)
            {
                print("Timer must be greater than 0.");
                return;
            }

            //Creates a new "CountdownTimer" task in the timer system. The console input is continually refreshed to avoid disrupting the user's current input.
            timerSystem.StartCountdown(
                "CountdownTimer",
                seconds,
                secondsLeft =>
                {
                    Console.WriteLine();
                    print("Time left: " + secondsLeft + " seconds");
                    RedrawInputLine();
                },
                () =>
                {
                    Console.WriteLine();
                    print("Timer finished!");
                    RedrawInputLine();
                }
            );

            print("Countdown started for " + seconds + " seconds.");
        }

        private void HandleCreateUserCommand(string[] parts)
        {
            if (!userManager.isLoggedIn || userManager.CurrentUser.UserRole != User.Role.Admin)
            {
                print("Only admins can create new users.");
                return;
            }

            if (parts.Length < 4)
            {
                print("Usage: createuser <username> <password> <user/admin>");
                return;
            }

            string username = parts[1];
            string password = parts[2];
            string roleInput = parts[3].ToLower();

            User.Role role = User.Role.User;

            if (roleInput == "admin")
            {
                role = User.Role.Admin;
            }

            userManager.CreateUser(username, password, role);

            print("Created user: " + username);
        }

        private void HandleWhoAmI()
        {
            if (userManager.isLoggedIn)
            {
                print("You are logged in as: " + userManager.CurrentUser.Username);
                print("Your role is: " + GetRoleName(userManager.CurrentUser));
            }
            else
            {
                print("You are not logged in.");
            }
        }

        private string GetRoleName(User user)
        {
            if (user.UserRole == User.Role.Admin)
            {
                return "Admin";
            }

            if (user.UserRole == User.Role.User)
            {
                return "User";
            }

            return "Unknown";
        }

        private void PrintCommands()
        {
            Console.WriteLine("Available commands:");

            foreach (string command in commands)
            {
                Console.WriteLine("- " + command);
            }
        }
        //Here a login screen is displayed, prompting users for thier username and password.
        private void DisplayLogin()
        {
            if (userManager.isLoggedIn)
            {
                userManager.Logout();
            }

            Console.Clear();

            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine("|              GoobOS Login            |");
            Console.WriteLine("+--------------------------------------+");
            Console.WriteLine();

            Console.Write("Username: ");
            string username = Console.ReadLine();

            Console.Write("Password: ");
            string password = Console.ReadLine();

            if (userManager.Login(username, password))
            {
                Console.Clear();
                Console.WriteLine("Login successful.");
                Console.WriteLine("Welcome, " + userManager.CurrentUser.Username + ".");
                print("Type 'help' for commands.");
            }
            else
            {
                //If a login fails, than the console is locked for 5 seconds to prevent additional input. Upon the timer ending, the login screen is redisplayed. 
                ConsoleIsLocked = true;
                Console.Clear();
                Console.WriteLine("Invalid username or password. Resetting Login...");

                timerSystem.WaitSeconds("Failed Login", 5, () =>
                {
                    ConsoleIsLocked = false;
                    DisplayLogin();

                });
            }
        }

        //The prompt is printed before every input, and is updated to reflect the current user's username if they are logged in.
        private void PrintPrompt()
        {
            if (userManager != null && userManager.isLoggedIn)
            {
                currentPrompt = userManager.CurrentUser.Username + "@goob> ";
            }
            else
            {
                currentPrompt = "guest@goob> ";
            }

            Console.Write(currentPrompt);
        }

        //This is a helper method for redrawing the current input line. It is used in situations like the timer command, which needs to print updates to the console without disrupting the user's current input.
        private void RedrawInputLine()
        {
            PrintPrompt();
            Console.Write(currentInput);
        }

        //This is a helper method for printing text to the console. If a user is logged in and the "IncludeUserTag" parameter is set to true, the user's username will be included in the printed message as a tag.
        public void print(string text, bool IncludeUserTag = true)
        {
            if (userManager != null && userManager.isLoggedIn && IncludeUserTag)
            {
                text = "[" + userManager.CurrentUser.Username + "] " + text;
            }

            Console.WriteLine(text);
        }
    }
}