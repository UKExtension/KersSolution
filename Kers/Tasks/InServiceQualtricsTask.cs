using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kers.Models.Contexts;
using Kers.Models.Entities.KERScore;
using Kers.Models.Repositories;
using Kers.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace Kers.Tasks
{
    public class InServiceQualtricsTask : TaskBase, IScheduledTask
    {
        IServiceProvider serviceProvider;
        public InServiceQualtricsTask(
            IServiceProvider serviceProvider
        ){
            this.serviceProvider = serviceProvider;
        }
        public string Schedule => "52 14 * * *";
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {

            var serviceScopeFactory = this.serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<KERScoreContext>();
                try{
                    var startTime = DateTime.Now;
                    
                    var trainings = new List<vInServiceQualtricsSurveysToCreate>();
                    var data = "";
                    var _configuration = scope.ServiceProvider.GetService<IConfiguration>();



                    var trnngs = context.Training
                                .Where( t => t.qualtricsSurveyID == null
                                                &&
                                                t.Start > new DateTime(2017,01,01,0, 0, 0, 0)
                                                &&
                                                t.Start < DateTimeOffset.Now
                                                &&
                                                t.tStatus == "A"
                                                )
                                .Include( t => t.submittedBy).ThenInclude( s => s.RprtngProfile);


/* 

                    var optionsBuilder = new DbContextOptionsBuilder<KersReportingContext>();
                    optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:connKersReporting"]);

                    using (var contexReporting = new KersReportingContext(optionsBuilder.Options))
                    {
                        trainings = contexReporting.vInServiceQualtricsSurveysToCreate.ToList();
 */
                        var qualtricsApiHost = _configuration["QualtricsApi:sApiHost"];
                        var qualtricsUser = _configuration["QualtricsApi:sUser"];
                        var qualtricsToken = _configuration["QualtricsApi:sToken"];
                        var qualtricsFormat = _configuration["QualtricsApi:sFormat"];
                        var qualtricsVersion = _configuration["QualtricsApi:sVersion"];
                        var qualtricsImportFormat = _configuration["QualtricsApi:sImportFormat"];
                        var qualtricsActivate = _configuration["QualtricsApi:sActivate"];
                        var client = new HttpClient();
                        
                        foreach( var training in trnngs ){

                            string sSurveyURL = "https://kers.ca.uky.edu/core/reports/Data/qltrx/" +
                                                    HttpUtility.UrlEncode(training.Subject) +
                                                    "/" + training.Id +
                                                    "/" + HttpUtility.UrlEncode(training.Start.ToString("MM/dd/yyyy") +
                                                    (training.End != null ? " - " +training.End?.ToString("MM/dd/yyyy") : "")); //title/id/dates
                            //"https://kers.ca.uky.edu/CES/rpt/zQualtricsInServiceEvaluationSurveyText.aspx?t=" + training.Id;
 
                            String url = qualtricsApiHost
                            + "Request=importSurvey"
                            + "&User=" + HttpUtility.UrlEncode(qualtricsUser)
                            + "&Token=" + qualtricsToken
                            + "&Format=" + qualtricsFormat
                            + "&Version=" + qualtricsVersion
                            + "&ImportFormat=" + qualtricsImportFormat
                            + "&Activate=" + qualtricsActivate
                            + "&Name=" + HttpUtility.UrlEncode(training.Subject)
                            + "&URL=" + HttpUtility.UrlEncode(sSurveyURL);
                            
                            try
                            {
                                client.DefaultRequestHeaders.Accept.Clear();
                                var result = client.GetAsync(url).Result;
                                data = result.Content.ReadAsStringAsync().Result;
                                XDocument xmlDoc = new XDocument();
                                    try
                                    {
                                        xmlDoc = XDocument.Parse(data);
                                        String surveyID = xmlDoc.Root.Element("Result").Value;
                                        training.qualtricsSurveyID = surveyID;
/* 

                                        var commandText = "UPDATE [UKCA_Reporting]..[zInServiceTrainingCatalog] SET qualtricsSurveyID = @p1 WHERE rID = @p2";;
                                        var surveyParameter = new SqlParameter("@p1", surveyID);
                                        var trainingParameter = new SqlParameter("@p2", training.tID);
                                        contexReporting.Database.ExecuteSqlCommand(commandText, parameters: new[] {
                                                                                                        surveyParameter, trainingParameter
                                                                                    });
 */

                                    }
                                    catch (Exception e)
                                    {
                                        await LogError(context, 
                                                "InServiceQualtricsTask", e, 
                                                "InService Qualtrics Task failed"
                                            ); 
                                    }
                            }
                            catch (WebException e)
                            {
                                await LogError(context, 
                                    "InServiceQualtricsTask", e, 
                                    "InService Qualtrics Task failed"
                                );    
                            } 
                        }
                   // }
                    context.SaveChanges();
                    var endTime = DateTime.Now;
                    await LogComplete(context, 
                                    "InServiceQualtricsTask", data, 
                                    "InService Qualtrics Task executed for " + (endTime - startTime).TotalSeconds + " seconds"
                                );
                }catch( Exception e){
                    await LogError(context, 
                                    "InServiceQualtricsTask", e, 
                                    "InService Qualtrics Task failed"
                            );
                }   
            }
        }
    }

    class KersReportingContext:DbContext{
        public KersReportingContext(DbContextOptions<KersReportingContext> options)
        : base(options)
        { }

        public virtual DbSet<vInServiceQualtricsSurveysToCreate> vInServiceQualtricsSurveysToCreate { get; set; }
    }

    public partial class vInServiceQualtricsSurveysToCreate
    {
        [Key]
        public int rID {get;set;}
        public string tID {get; set;}
        public string trainDateBegin {get;set;}
        public string  tTitle {get;set;}
        public string qualtricsTitle {get;set;}
    }

}

/*





public partial class _Default : System.Web.UI.Page
{
    Int32 rID;
    String sTrainingID = "";
    String sTitle = "";
    String sDates = "";

    protected void Page_Load(object sender, EventArgs e)
    {
        int id;
        if (!int.TryParse(Request.QueryString["t"], out id))
        {
            Response.End();
        }

        rID = id;
        getTrainingDetails();

        if (sTrainingID != "") {
            Response.Clear();
            Response.ClearHeaders();
            Response.AddHeader("Content-Type", "text/plain");
            RenderSurveyText();
            Response.Flush();
            Response.End();
        }
        else
        {
            Response.End();
        }
    }

    protected void getTrainingDetails()
    {
        SqlConnection conn = null;
        try
        {
            string sql = "SELECT * FROM [UKCA_Reporting]..[vInServiceTrainingCatalog] WHERE rID = @p1";
            conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connA"].ConnectionString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            SqlParameter param1 = new SqlParameter();
            param1.ParameterName = "@p1";
            param1.Value = rID;
            cmd.Parameters.Add(param1);
            conn.Open();
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    sTrainingID = reader.GetString(reader.GetOrdinal("tID"));
                    sTitle = reader.GetString(reader.GetOrdinal("tTitle"));
                    sDates = reader.GetString(reader.GetOrdinal("trainDateBeginEndMMDDYYYY"));
                }
            }
            conn.Close();
        }
        catch (Exception ex)
        {
            // Log your error
        }
        finally
        {
            if (conn != null) conn.Close();
        }
    }

    protected void RenderSurveyText()
    {
        Response.Write("[[AdvancedFormat]]\n\n");

        Response.Write("[[Question:Text]]\n");
        Response.Write("Cooperative Extension In-Service Training Evaluation <br /><br />");
        Response.Write(sTitle + " [" + sTrainingID + "] <br /><br />");
        Response.Write(sDates);
        Response.Write("\n\n");
        
        Response.Write("[[Question:Matrix]]\n");
        Response.Write("The Content:\n");
        Response.Write("[[Choices]]\n");
        Response.Write("Was relevant to my needs.\n");
        Response.Write("Was well organized.\n");
        Response.Write("Was adequately related to the topic.\n");
        Response.Write("Was easy to understand.\n");
        Response.Write("[[AdvancedAnswers]]\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Strongly Disagree\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Disagree\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Neutral\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Agree\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Strongly Agree\n");
        Response.Write("\n");

        Response.Write("[[Question:Matrix]]\n");
        Response.Write("The Instructor(s):\n");
        Response.Write("[[Choices]]\n");
        Response.Write("Were well-prepared.\n");
        Response.Write("Used teaching methods appropriate for the content/audience.\n");
        Response.Write("Was knowledgeable of the subject matter.\n");
        Response.Write("Engaged the participants in learning.\n");
        Response.Write("Related program content to practical situations.\n");
        Response.Write("Answered questions clearly and accurately.\n");
        Response.Write("[[AdvancedAnswers]]\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Strongly Disagree\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Disagree\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Neutral\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Agree\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Strongly Agree\n");
        Response.Write("\n");

        Response.Write("[[Question:Matrix]]\n");
        Response.Write("Outcomes:\n");
        Response.Write("[[Choices]]\n");
        Response.Write("I gained knowledge/skills about the topics presented.\n");
        Response.Write("I will use what I learned in my county program.\n");
        Response.Write("This information will help my program move to the next level.\n");
        Response.Write("Based on the in-service, I am now able to teach this topic to others.\n");
        Response.Write("[[AdvancedAnswers]]\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Strongly Disagree\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Disagree\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Neutral\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Agree\n");
        Response.Write("[[Answer]]\n");
        Response.Write("Strongly Agree\n");
        Response.Write("\n");

        Response.Write("[[Question:TE]]\n");
        Response.Write("Based on this in-service, what are two things that you are encouraged to do within the next month?\n");
        Response.Write("\n");

        Response.Write("[[Question:TE]]\n");
        Response.Write("Based on this in-service, what are two things that you are encouraged to do within the next six (6) months?\n");
        Response.Write("\n");

        Response.Write("[[Question:TE]]\n");
        Response.Write("If you have a program related to this topic, what do you think will help take it to the next level (i.e., achieve higher level impact)?\n");
        Response.Write("\n");

        Response.Write("[[Question:TE]]\n");
        Response.Write("Please provide any additional comments about this training.\n");
        Response.Write("\n");

        Response.Write("[[Question:TE]]\n");
        Response.Write("Please provide any comments about the instructor or any additional instructors/presenters.\n");
        Response.Write("\n");
    }

}







 */