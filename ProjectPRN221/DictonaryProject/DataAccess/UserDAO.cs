using DictonaryProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DictonaryProject.DataAccess
{
    class UserDAO
    {
        private static UserDAO _instance = null;
        private static readonly object instancelock = new object();

        public UserDAO() { }
        public static UserDAO Instance
        {
            get
            {
                lock (instancelock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserDAO();
                    }
                    return _instance;
                }
            }
        }

        public bool checkUserNameExist(string username)
        {
            try
            {
                using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Username == username);
                    if (user != null)
                    {
                        MessageBox.Show("Username already exists.");
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }
        }
        public bool RegisterUser(string username, string password)
        {
            try
            {
                using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
                {

                    var defaultRole = context.Roles.FirstOrDefault(r => r.RoleId == 1);
                    if (defaultRole == null)
                    {
                        MessageBox.Show("Default role not found.");
                        return false;
                    }


                    var newUser = new User()
                    {
                        Username = username,
                        PasswordHash = HashPassword(password),
                        CreatedDate = DateTime.Now,
                        Roles = new List<Role>() { defaultRole }
                    };
                    context.Users.Add(newUser);
                    context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return false;
            }

        }
        public User CheckLogin(string username, string password)
        {
            try
            {
                using (PersonalDictionaryDBContext context = new PersonalDictionaryDBContext())
                {
                    string hashedPassword = HashPassword(password);

                    var user = context.Users
                                      .Include(u => u.Roles) 
                                      .FirstOrDefault(u => u.Username == username && u.PasswordHash == hashedPassword);

                    if (user != null)
                    {
                        var role = user.Roles.FirstOrDefault();
                        if (role != null)
                        {
                            if (role.RoleName == "Admin")
                            {
                                DictionaryManagementScreen adminScreen = new DictionaryManagementScreen();
                                adminScreen.Show();
                            }
                            else if (role.RoleName == "User")
                            {
                                DictionaryMainScreen userScreen = new DictionaryMainScreen();
                                userScreen.Show();
                            }
                            Application.Current.MainWindow.Close(); 
                        }
                        return user;
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }




        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
