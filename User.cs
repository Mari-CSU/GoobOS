using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace GoobOS
{
    // User class to represent a user in the system
    // This class includes properties for username, password, and role
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Role UserRole { get; set; }
        public enum Role
        {
            User,
            Admin
        }

        public User(string username, string password, Role role)
        {
            Username = username;
            Password = password;
            UserRole = role;
        }

        public bool CheckPassword(string password)
        {
            return Password == password;
        }

        public void DisplayProfile()
        {
            Console.WriteLine("+----------------------------+");
            Console.WriteLine("|        USER PROFILE        |");
            Console.WriteLine("+----------------------------+");
            Console.WriteLine("Username: " + Username);
            Console.WriteLine("Role: " + UserRole);
            Console.WriteLine("Status: Active");
            Console.WriteLine("+----------------------------+");
        }
    }
}
