using DictonaryProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictonaryProject
{
    public static  class CurrentUser
    {
        public static User LoggedInUser { get; private set; }

        public static void SetUser(User user)
        {
            LoggedInUser = user;
        }

        public static void ClearUser()
        {
            LoggedInUser = null;
        }
    }
}
