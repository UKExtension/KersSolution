using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class TaskRecurringSchedule : IEntityBase
    {


/* 
Frequency : 0 = EachRun, 1 = Hourly, 2 = Daily, 3 = Weekly, 4 = Monthly or 5 = Yearly.
DayNo : What Day to run on (1-7 for weekly, 1-31 for monthly, 1-365 for yearly)
Interval : Every x weeks, months etc.
WeekOfMonth : first, second, third... etc If populated then DayNo specifies the day of the week.
MonthOfYear : 1-12.
EndDatetime : The last date to perform
Occurences : The number of times to perform. If this and the previous value are null then perform for ever.
 */

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Frequency {get;set;}
        public int DayNo {get;set;}
        public int Interval {get;set;}
        public int WeekOfMonth {get;set;}
        public int MonthOfYear {get;set;}
        public int Occurences {get;set;}
        public DateTime EndDatetime {get;set;}
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
    }
}