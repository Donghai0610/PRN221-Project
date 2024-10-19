using System;
using System.Collections.Generic;

namespace DictonaryProject.Models
{
    public partial class User
    {
        public User()
        {
            Dictionaries = new HashSet<Dictionary>();
            Roles = new HashSet<Role>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }

        public virtual ICollection<Dictionary> Dictionaries { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
