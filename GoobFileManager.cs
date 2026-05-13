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
    //Helper class to manage file operations such as creating, reading, writing, and deleting files.
    //Most of this code was originally written for the kernel class. 
    public class GoobFileManager
    {
        public string CurrentDirectory { get; private set; }
        public Kernel kernel;

        public GoobFileManager()
        {
            CurrentDirectory = @"0:\";
        }

        public string CreatePath(string path)
        {
            if (path.StartsWith(@"0:\"))
            {
                return path;
            }

            return kernel.currentDirectory + path;
        }

        public void HandleMoveFile(string[] parts)
        {
            try
            {
                if (parts.Length < 3)
                {
                    kernel.print("Usage: movefile <file_name> <new_path>");
                    return;
                }

                string fileName = parts[1];
                string newPathInput = parts[2];

                string oldPath = CreatePath(fileName);
                string newPath = CreatePath(newPathInput);

                if (!File.Exists(oldPath))
                {
                    kernel.print("File not found: " + oldPath);
                    return;
                }

                File.Copy(oldPath, newPath);
                File.Delete(oldPath);

                kernel.print("Moved file from " + oldPath + " to " + newPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void HandleWriteFile(string[] parts)
        {
            try
            {

                if (parts.Length < 3)
                {
                    kernel.print("Usage: writefile <file_name> <text>");
                    return;
                }

                string fileName = parts[1];
                string text = kernel.JoinParts(parts, 2);
                string filePath = CreatePath(fileName);

                File.WriteAllText(filePath, text);

                File.WriteAllText(fileName, text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void HandleCreateFile(string[] parts)
        {
            try
            {
                if (parts.Length < 2)
                {
                    kernel.print("Usage: read <file_name>");
                    return;
                }

                string fileName = "";

                for (int i = 1; i < parts.Length; i++)
                {
                    fileName += parts[i];

                    if (i < parts.Length - 1)
                    {
                        fileName += " ";
                    }
                }

                string filePath = CreatePath(fileName);
                var file_stream = File.Create(filePath);
                file_stream.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void HandleDeleteFile(string[] parts)
        {
            try
            {
                if (parts.Length < 2)
                {
                    kernel.print("Usage: deletefile <file_name>");
                    return;
                }

                string fileName = "";

                for (int i = 1; i < parts.Length; i++)
                {
                    fileName += parts[i];

                    if (i < parts.Length - 1)
                    {
                        fileName += " ";
                    }
                }

                string filePath = CreatePath(fileName);
                File.Delete(filePath);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void HandleDeleteCurrentDirectory()
        {
            try
            {
                if (kernel.userManager.CurrentUser.UserRole == Role.Admin)
                    Directory.Delete(kernel.currentDirectory);
                else
                    kernel.print("You must be an admin to delete this directory.");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void HandleMkDir(string[] parts)
        {
            if (parts.Length < 2)
            {
                kernel.print("Usage: mkdir <directory_name>");
                return;
            }

            string directoryName = "";

            for (int i = 1; i < parts.Length; i++)
            {
                directoryName += parts[i];

                if (i < parts.Length - 1)
                {
                    directoryName += " ";
                }
            }

            try
            {
                string directoryPath = CreatePath(directoryName);
                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void HandleRead(string[] parts)
        {
            if (parts.Length < 2)
            {
                kernel.print("Usage: read <file_name>");
                return;
            }

            string fileName = "";

            for (int i = 1; i < parts.Length; i++)
            {
                fileName += parts[i];

                if (i < parts.Length - 1)
                {
                    fileName += " ";
                }
            }

            string filePath = CreatePath(fileName);

            try
            {
                if (!File.Exists(filePath))
                {
                    kernel.print("File not found: " + fileName);
                    return;
                }

                string content = File.ReadAllText(filePath);

                kernel.print("File name: " + fileName);
                kernel.print("File size: " + content.Length + " characters");
                kernel.print("Content:");
                Console.WriteLine(content);
            }
            catch (Exception e)
            {
                kernel.print("Could not read file: " + fileName);
                Console.WriteLine(e.ToString());
            }
        }

        public void HandleListFiles()
        {
            var filesList = Directory.GetFiles(@"0:\");
            foreach (var file in filesList)
            {
                Console.WriteLine(file);
            }
        }

        public void HandleReadAll()
        {
            var files = Directory.GetFiles(kernel.currentDirectory);

            foreach (var file in files)
            {
                string filePath = CreatePath(file);
                if (!file.StartsWith(@"0:\"))
                {
                    filePath = CreatePath(file);
                }

                try
                {
                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine("File does not exist: " + filePath);
                        continue;
                    }

                    string content = File.ReadAllText(filePath);

                    Console.WriteLine("File name: " + filePath);
                    Console.WriteLine("File size: " + content.Length);
                    Console.WriteLine("Content: " + content);
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not read file: " + filePath);
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void HandleMoveDir(string[] parts)
        {
            var currentDirectory = kernel.currentDirectory;
            if (parts.Length < 2)
            {
                kernel.print("Usage: movedir <directory_name>");
                return;
            }

            string directoryName = "";

            for (int i = 1; i < parts.Length; i++)
            {
                directoryName += parts[i];

                if (i < parts.Length - 1)
                {
                    directoryName += " ";
                }
            }

            try
            {
                if (directoryName == @"\")
                {
                    currentDirectory = @"0:\";
                    kernel.print("Current directory: " + currentDirectory);
                    return;
                }

                string directoryPath = CreatePath(directoryName);

                if (!directoryPath.EndsWith(@"\"))
                {
                    directoryPath += @"\";
                }

                if (!Directory.Exists(directoryPath))
                {
                    kernel.print("Directory not found: " + directoryPath);
                    return;
                }

                kernel.currentDirectory = directoryPath;
                kernel.print("Current directory: " + kernel.currentDirectory);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void HandleRootDir(string[] parts)
        {
            kernel.currentDirectory = @"0:\";
            kernel.print("Current directory: " + kernel.currentDirectory);
        }

    }
}

