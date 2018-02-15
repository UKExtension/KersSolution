using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class TaskPerformed : IEntityBase
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TaskScheduleId {get;set;}
        public TaskSchedule TaskSchedule {get;set;}
        public DateTime PerformedAt {get;set;}

    }
}