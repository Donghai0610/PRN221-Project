using System;
using System.Collections.Generic;

namespace DictonaryProject.Models
{
    public partial class Meaning
    {
        public int MeaningId { get; set; }
        public int? WordId { get; set; }
        public string EnglishMeaning { get; set; } = null!;
        public string VietnameseMeaning { get; set; } = null!;
        public string? ExampleSentence { get; set; }

        public virtual Dictionary? Word { get; set; }
    }
}
