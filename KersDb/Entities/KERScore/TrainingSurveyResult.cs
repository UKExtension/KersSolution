using System;
using System.ComponentModel.DataAnnotations;

namespace Kers.Models.Entities.KERScore
{
    public partial class TrainingSurveyResult
    {
        [Key]
        public int Id { get; set; }

        public string Result { get; set; }

        public KersUser User { get; set; }
        public int? UserId {get;set;}
        public Training Training {get;set;}
        public int TrainingId {get;set;}
        public DateTime Created {get;set;}
    }
}