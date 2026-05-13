using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sys = Cosmos.System;

namespace GoobOS
{
    //Manager to oversee user accounts, login/logout, and user creation
    public class UserManager
    {
        public List<User> users;
        public User CurrentUser { get; private set; }

        public bool isLoggedIn
        {
          get { return CurrentUser != null; }
        }
        
        //Constructor for the UserManager class, initializes the list of users and adds default users.
        public UserManager()
        {

            users = new List<User>();
            // Adds a default admin
            users.Add(new User("admin", "admin", User.Role.Admin));
            //Adds a default user
            users.Add(new User("user", "user", User.Role.User));
        }   

        public bool Login(string username, string password)
        {
            foreach (var user in users)
            {
                if (user.Username == username && user.CheckPassword(password))
                {
                    CurrentUser = user;
                    CurrentUser.UserRole = user.UserRole;
                    return true;
                }
            }
            return false;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public void CreateUser(string username, string password, User.Role role)
        {
            if (CurrentUser != null)
            {
                users.Add(new User(username, password, role));
                Console.WriteLine("User created successfully.");
            }
        }
    }
}
