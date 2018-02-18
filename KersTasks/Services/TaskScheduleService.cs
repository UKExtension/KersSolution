using System;
using System.Linq;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;

namespace Kers.Tasks.Services{
    public class TaskScheduleService{
        KERScoreContext context;
        public TaskScheduleService( KERScoreContext context){
            this.context = context;
        }
        public bool ShouldItBeExecuted( TaskSchedule schedule){
            if(schedule.TaskRecurringSchedule.Frequency == 0){
                // Execute each time the script runs
                return true;
            }else if(schedule.TaskRecurringSchedule.Frequency == 2){
                // Daily
                return DailySchedule(schedule);
            }
            return false;
        }

        private bool DailySchedule(TaskSchedule schedule){
            var ent = context.TaskPerformed.Where( p => p.TaskSchedule == schedule && p.PerformedAt.ToShortDateString() == DateTime.Now.ToShortDateString()).FirstOrDefault();
            if(  ent != null ){
                return false;
            }
            if( schedule.TaskRecurringSchedule.DayNo > DateTime.Now.Hour ){
                return false;
            }
            return true;
        }

        /* 
        Frequency : 0 = EachRun, 1 = Hourly, 2 = Daily, 3 = Weekly, 4 = Monthly or 5 = Yearly.
        DayNo : What Day/Hour to run on (0-23 for daily, 1-7 for weekly, 1-31 for monthly, 1-365 for yearly)
        Interval : Every x weeks, months etc.
        WeekOfMonth : first, second, third... etc If populated then DayNo specifies the day of the week.
        MonthOfYear : 1-12.
        EndDatetime : The last date to perform
        Occurences : The number of times to perform. If this and the previous value are null then perform for ever.
        */


    }
}