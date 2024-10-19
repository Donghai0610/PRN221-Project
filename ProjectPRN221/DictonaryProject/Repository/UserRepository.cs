using DictonaryProject.DataAccess;
using DictonaryProject.Models;
using DictonaryProject.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictonaryProject.Repository
{
    public class UserRepository : IUserRepository
    {
        public bool checkUserNameExist(string username)
        {
            return UserDAO.Instance.checkUserNameExist(username);
        }

        public User Login(string username, string password)
        {
            return UserDAO.Instance.CheckLogin(username, password);
        }

        public bool RegisterUser(string username, string password)
        {
            return UserDAO.Instance.RegisterUser(username, password);
        }
    }
}
