using System;
using System.ComponentModel.DataAnnotations;

namespace Kers.Models.Entities.KERScore
{
    public partial class TrainingSession
    {
        [Key]
        public int Id { get; set; }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public string Note {get;set;}

        public int Index { get; set; }
        public bool IsCancelled { get; set; }
    }
}