using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class TaskSchedule : IEntityBase
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TaskRecurringScheduleId {get;set;}
        public TaskRecurringSchedule TaskRecurringSchedule {get;set;}
        public int TaskOperationId {get;set;}
        public TaskOperation TaskOperation {get;set;}
        public DateTime EndDatetime {get;set;}
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
    }
}