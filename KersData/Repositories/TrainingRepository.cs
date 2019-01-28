using System;
using System.Collections.Generic;
using System.Linq;
using Kers.Models.Repositories;
using System.Threading.Tasks;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Entities.KERScore;
using Kers.Models.Contexts;
using System.IO;
using CsvHelper;
using Kers.Models.Entities.UKCAReporting;

namespace Kers.Models.Repositories
{
    public class TrainingRepository : ITrainingRepository
    {

        KERScoreContext context;
        public TrainingRepository(KERScoreContext context)
            
        { 
            this.context = context;
        }

        public List<zInServiceTrainingCatalog>  csv2list(string fileUrl = "database/trainingsData.csv"){
            //ViewData["trainings"] = this.reportingContext.zInServiceTrainingCatalog.Take(10).ToList();
            var records = new List<zInServiceTrainingCatalog>();
            using (var reader = new StreamReader(fileUrl))
            using (var csv = new CsvReader(reader))
            {    


                
                

                var newRecords = new List<Training>();
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    DateTime date;
                    DateTime.TryParse(csv.GetField("rDT"), out date);
                    
                    DateTime submittedDate;
                    DateTime.TryParse(csv.GetField("submittedDate"), out submittedDate);

                    DateTime approvedDate;
                    DateTime.TryParse(csv.GetField("approvedDate"), out approvedDate);

                    DateTime sessionCancelledDate;
                    DateTime.TryParse(csv.GetField("approvedDate"), out sessionCancelledDate);
                    

                    var record = new zInServiceTrainingCatalog
                    {
                        rID = csv.GetField<int>("rID"),
                        rDT = date,
                        submittedDate = submittedDate,
                        submittedByPersonID = csv.GetField("submittedByPersonID"),
                        submittedByPersonName = csv.GetField("submittedByPersonName"),
                        approvedDate = approvedDate,
                        approvedByPersonID = csv.GetField("approvedByPersonID"),
                        approvedByPersonName = csv.GetField("approvedByPersonName"),
                        tID = csv.GetField("tID"),
                        tStatus = csv.GetField("tStatus"),
                        sessionCancelledDate = sessionCancelledDate,
                        TrainDateBegin = csv.GetField("TrainDateBegin"),
                        TrainDateEnd = csv.GetField("TrainDateEnd"),
                        RegisterCutoffDays = csv.GetField<int>("RegisterCutoffDays"),
                        CancelCutoffDays = csv.GetField<int>("CancelCutoffDays"),
                        iHours = csv.GetField("iHours") == "NULL" ? 0 : csv.GetField<int>("iHours"),
                        seatLimit = csv.GetField("seatLimit") == "NULL" ? 0 : csv.GetField<int>("seatLimit"),
                        tTitle = csv.GetField("tTitle"),
                        tLocation = csv.GetField("tLocation"),
                        tTime = csv.GetField("tTime"),
                        day1 = csv.GetField("day1"),
                        day2 = csv.GetField("day2"),
                        day3 = csv.GetField("day3"),
                        day4 = csv.GetField("day4"),
                        tContact = csv.GetField("tContact"),
                        tDescription = csv.GetField("tDescription"),
                        tAudience = csv.GetField("tAudience"),
                        qualtricsSurveyID = csv.GetField("qualtricsSurveyID"),
                        evaluationLink = csv.GetField("evaluationLink"),
                    };
                    records.Add(record);
                }
            }
            return records;
        }
    }
}