using DictonaryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictonaryProject.Repository.IRepository
{
    public interface IUserRepository
    {
        bool RegisterUser(string username, string password);
        User Login(string username, string password);

        bool checkUserNameExist(string username);
    }
}
