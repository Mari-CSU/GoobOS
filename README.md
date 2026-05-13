# GoobOS
The project I developed is known as GoobOS. For this project I wanted to develop my own OS, but I wanted to do it in a language I was most comfortable with. I’m mainly experienced in C# and Java, which is a problem since OSs are typically made from lower level languages such as C and Assembly. I was adamant about working around this, so I went to the internet in search of solutions. 

Throughout my searching I was able to stumble upon Cosmos. Cosmos is an open source operating system development kit which uses Visual Studio and C# as a development enviroment.

Website Link: https://cosmosos.github.io/

Cosmos primarily handles the low-level boot/runtime layer, allowing the project to be run as an .iso. C# usually required a runtime such as .NET to run on top of the current OS, but Cosmos is able to bypass this restriction, allowing C# to run on a bare-metal/OS-development environment. Additionally Cosmos provides support for file system management through its Virtual File System layer and native I/O support.

For the purposes of this project, I sought to try to recreate the more basic functions of an OS via console window. Currently the project has the following features:
Login Screen
User Manager
User Roles
Command Shell
Command Parser
Timer System
File Commands
Custom I/O Handling
System Utilities
For this report, I will showcase all the commands present within the OS. Additional documentation and comments on individual functions will be a part of the Github page.

Total Code Lines: ~1100

--------------------------------
Default credentials:

Username: Admin
Password: Admin

Username: User
Password: User
--------------------------------
Command List
--------------------------------
  help | Displays all commands.

clear | Clears console window.

about | Displays information about the OS.

echo <message> | Echoes the next message entered.

restart | Restarts the OS.

shutdown | Shuts down the OS.

login | Return to login screen.

logout | Logout User.

whoami | Displays information about the current user.

createuser <username> <password> <user/admin> | Creates a new user. Only admins can create new users.

timer <seconds> | Sets a timer for X seconds.

listusers | Lists all users of the OS.

listfiles | Lists all files within the current directory.

readall | Reads all files within the current directory.

read <file_name> | Reads a specific file.

mkdir <directory_name> | Makes a new directory with the inputted name.

listdir | Lists the current directory.

createfile <file_name> | Creates a new file with the given name.

deletefile <file_name> | Deletes specified file.

deletedir | Deletes current directory (Warning unstable, admin only).

writefile <file_name> <text> | Writes text to a specified file.

movefile <file_name> <new_path> | Moves a file to a new path.

movedir <directory_name> | Moves into specified directory.

rootdir | Returns to root directory.
