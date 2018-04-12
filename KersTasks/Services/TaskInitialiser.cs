using System;
using System.Linq;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;

namespace Kers.Tasks.Services{
    public class TaskInitialiser{
        KERScoreContext context;
        public TaskInitialiser(KERScoreContext context){
            this.context = context; 
        }
/* 
        public void addSampleTask(){
            if( context.TaskSchedule.Any()){
                return;
            }
            addSummaryByMonthTask();

        }

 */        public void addSummaryByMonthTask(){
            var recurringSchedule = new TaskRecurringSchedule();
            recurringSchedule.Frequency = 2;
            recurringSchedule.DayNo = 2;
            recurringSchedule.EndDatetime = DateTime.Now.AddDays(12345);
            recurringSchedule.Created = DateTime.Now;
            recurringSchedule.Updated = DateTime.Now;

            var operation = new TaskOperation();
            operation.ClassName = "SnapSummaryByMonthTask";
            operation.Description = "Snap-Ed Summary By Month Csv Generation";
            operation.Arguments = "";
            operation.Created = DateTime.Now;
            operation.Updated = DateTime.Now;


            var schedule = new TaskSchedule();
            schedule.TaskOperation = operation;
            schedule.TaskRecurringSchedule = recurringSchedule;
            schedule.Created = DateTime.Now;
            schedule.Updated = DateTime.Now;

            context.Add(schedule);
            context.SaveChanges();
        }

        public void addSummaryByCountyTask(){
            var recurringSchedule = new TaskRecurringSchedule();
            recurringSchedule.Frequency = 2;
            recurringSchedule.DayNo = 3;
            recurringSchedule.EndDatetime = DateTime.Now.AddDays(12345);
            recurringSchedule.Created = DateTime.Now;
            recurringSchedule.Updated = DateTime.Now;

            var operation = new TaskOperation();
            operation.ClassName = "SnapSummaryByCountyTask";
            operation.Description = "Snap-Ed Summary By County Csv Generation";
            operation.Arguments = "";
            operation.Created = DateTime.Now;
            operation.Updated = DateTime.Now;


            var schedule = new TaskSchedule();
            schedule.TaskOperation = operation;
            schedule.TaskRecurringSchedule = recurringSchedule;
            schedule.Created = DateTime.Now;
            schedule.Updated = DateTime.Now;

            context.Add(schedule);
            context.SaveChanges();
        }

        public void addSummaryByEmployeeTask(){
            var recurringSchedule = new TaskRecurringSchedule();
            recurringSchedule.Frequency = 2;
            recurringSchedule.DayNo = 4;
            recurringSchedule.EndDatetime = DateTime.Now.AddDays(12345);
            recurringSchedule.Created = DateTime.Now;
            recurringSchedule.Updated = DateTime.Now;

            var operation = new TaskOperation();
            operation.ClassName = "SnapSummaryByEmployeeTask";
            operation.Description = "Snap-Ed Summary By Employee Csv Generation";
            operation.Arguments = "";
            operation.Created = DateTime.Now;
            operation.Updated = DateTime.Now;


            var schedule = new TaskSchedule();
            schedule.TaskOperation = operation;
            schedule.TaskRecurringSchedule = recurringSchedule;
            schedule.Created = DateTime.Now;
            schedule.Updated = DateTime.Now;

            context.Add(schedule);
            context.SaveChanges();
        }
    }
}