using System;
using System.Collections.Generic;

namespace DictonaryProject.Models
{
    public partial class Dictionary
    {
        public Dictionary()
        {
            Meanings = new HashSet<Meaning>();
            Categories = new HashSet<Category>();
        }

        public int WordId { get; set; }
        public string EnglishWord { get; set; } = null!;
        public string? Pronunciation { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsApproved { get; set; }
        public string? TypeOfWord { get; set; }

        public virtual User? CreatedByNavigation { get; set; }
        public virtual ICollection<Meaning> Meanings { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}
