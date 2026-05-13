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
Help: Displays all commands

About: Displays information about the OS

Echo: Echoes the next message entered.

Logout: Logs user out of OS. *Note does not return them to the login screen.

WhoAmi: Displays information about the current user

Createuser: Creates a new user. Only admins can create new users.

Timer: Sets a timer for X seconds

Listusers: List all users within the OS

Listfiles: List all files within the current directory

Readall: Reads all files within the current directory

Read:  Reads a specific file

Mkdir: Make a new directory with inputted name

Movedir: Moves into specified directory *Slightly bugged

Createfile: Creates a new file with the given name

Deletefile: Deletes specified file

Writefile: Writes text to a specified file. If a file doesn’t exist a new one is created. *Warning, while the console will display a file not found exception, this is really just to prompt the creation of a new file. 


moveFile: Moves files to a new path.
moveDir: Moves into specified directory.
*Readall was used here to display all files present within newDir.


Returns user to root directory.
