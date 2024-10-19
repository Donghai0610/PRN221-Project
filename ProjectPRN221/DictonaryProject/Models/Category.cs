using System;
using System.Collections.Generic;

namespace DictonaryProject.Models
{
    public partial class Category
    {
        public Category()
        {
            Words = new HashSet<Dictionary>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public virtual ICollection<Dictionary> Words { get; set; }
    }
}
