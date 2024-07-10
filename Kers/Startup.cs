using System.Text;
using Kers.Models;
using Kers.Models.Abstract;
using Kers.Models.Contexts;
using Kers.Models.Repositories;
using Kers.Services;
using Kers.Services.Abstract;
using Kers.Tasks;
using Kers.Tasks.Scheduling;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Kers
{
    public class Startup
    {

        private string secretKey;
        private IWebHostEnvironment CurrentEnvironment;
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            this.secretKey = Configuration["Secret:JWTKey"].ToString();
            CurrentEnvironment = env;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

           /*  var connString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite( connString   ));
 */
            services.AddDatabaseDeveloperPageExceptionFilter();
/* 
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
 */
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(jwtBearerOptions =>
                {
                    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateActor = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "KERSSystem",
                        ValidAudience = "KersUsers",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Secret:JWTKey"]))
                    };
                });
            services.AddHttpContextAccessor();
            services.AddMvc(
                options =>
                    {
                        //options.OutputFormatters.Add(new CsvOutputFormatter());
                    }
            ).AddNewtonsoftJson( options => {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            
            services.AddControllersWithViews();
            //services.AddRazorPages();






            if(CurrentEnvironment.IsDevelopment()){
                // Development
                services.AddDbContext<KERSmainContext>(options => 
                    options.UseSqlite(Configuration["ConnectionStrings:connKersMainLocal"]));
                services.AddDbContext<KERScoreContext>(options => 
                    options.UseSqlite(Configuration["ConnectionStrings:connKersCoreLocal"], b => {
                        b.MigrationsAssembly("Kers");
                    }));
                services.AddDbContext<KERS_SNAPED2017Context>(options => 
                   options.UseSqlite(Configuration["ConnectionStrings:connKersSnapLocal"]));

                services.AddDbContext<KERSreportingContext>(options => 
                    options.UseSqlite(Configuration["ConnectionStrings:connKersReportingLocal"]));

                services.AddDbContext<SoilDataContext>(options => 
                    options.UseSqlite(Configuration["ConnectionStrings:soilDataLocal"]));
                   

                services.AddDistributedMemoryCache();
            }else if(CurrentEnvironment.IsStaging()){


                // Staging
                services.AddDbContext<KERSmainContext>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKersMain"]));
                services.AddDbContext<KERScoreContext>(options => 
                    options.UseSqlServer(
                        Configuration["ConnectionStrings:connKersCore"], 
                            sqlServerOptions => {
                                    sqlServerOptions.MigrationsAssembly("Kers");
                                    sqlServerOptions.CommandTimeout(60);
                                }
                        )
                    );
                services.AddDbContext<KERS2017Context>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKERS2017"]));
                services.AddDbContext<KERS_SNAPED2017Context>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKERS_SNAPED2017"]));
                services.AddDbContext<KERSreportingContext>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKersReporting"]));
                services.AddDbContext<SoilDataContext>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connSoilData"]));
                

                services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = Configuration["ConnectionStrings:connKersCore"];
                        options.SchemaName = "dbo";
                        options.TableName = "Cache";
                    });


                    
            }else if( CurrentEnvironment.IsProduction()){


               
                //Production
                services.AddDbContext<KERSmainContext>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKersMain"]));
                    
                services.AddDbContext<KERScoreContext>(options => 
                    options.UseSqlServer(
                        Configuration["ConnectionStrings:connKersCore"], 
                            sqlServerOptions => {
                                    sqlServerOptions.MigrationsAssembly("Kers");
                                    sqlServerOptions.CommandTimeout(60);
                                }
                        )
                    );
                
                services.AddDbContext<KERS2017Context>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKERS2017"]));
                services.AddDbContext<KERS_SNAPED2017Context>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKERS_SNAPED2017"]));
                services.AddDbContext<KERSreportingContext>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKersReporting"]));
                services.AddDbContext<SoilDataContext>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connSoilData"]));
                
                services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = Configuration["ConnectionStrings:connKersCore"];
                        options.SchemaName = "dbo";
                        options.TableName = "Cache";
                    });

                // Add scheduled tasks & scheduler
                services.AddSingleton<IScheduledTask, SnapSummaryByMonthTask>();
                services.AddSingleton<IScheduledTask, SnapByAimedTowardImprovementTask>();
                services.AddSingleton<IScheduledTask, SnapPersonalHourDetailsTask>();
                services.AddSingleton<IScheduledTask, SnapSitesPerPersonPerMonthTask>();
                services.AddSingleton<IScheduledTask, SnapSummaryByCountyTask>();
                services.AddSingleton<IScheduledTask, SnapSummaryByEmployeeTask>();
                services.AddSingleton<IScheduledTask, SnapByPartnerCategoryTask>();
                services.AddSingleton<IScheduledTask, SnapCopiesDetailAgentsTask>();
                services.AddSingleton<IScheduledTask, SnapCopiesDetailNotAgentsTask>();
                services.AddSingleton<IScheduledTask, SnapCopiesSummarybyCountyAgentsTask>();
                services.AddSingleton<IScheduledTask, SnapCopiesSummarybyCountyNotAgentsTask>();
                services.AddSingleton<IScheduledTask, SnapReimbursementCountyTask>();
                services.AddSingleton<IScheduledTask, SnapReimbursementNepAssistantsTask>();
                services.AddSingleton<IScheduledTask, SpecificSiteNamesByMonthTask>();
                services.AddSingleton<IScheduledTask, SnapAgentCommunityEventDetailTask>();
                services.AddSingleton<IScheduledTask, SnapNumberofDeliverySitesbyTypeofSettingTask>();
                services.AddSingleton<IScheduledTask, SnapMethodsUsedRecordCountTask>();
                services.AddSingleton<IScheduledTask, SnapIndividualContactTotalsTask>();
                services.AddSingleton<IScheduledTask, SnapEstimatedSizeofAudiencesReachedTask>();
                services.AddSingleton<IScheduledTask, SnapSessionTypebyMonthTask>();
                services.AddSingleton<IScheduledTask, SnapIndirectByEmployeeTask>();
                services.AddSingleton<IScheduledTask, ActivityPerMajorProgramTask>();
                services.AddSingleton<IScheduledTask, ActivityContactsByCountyByMajorProgramTask>();
                services.AddSingleton<IScheduledTask, ActivityPerEmployeeReportsTask>();
                services.AddSingleton<IScheduledTask, SnapSpecificSiteNameDetailsTask>();
                services.AddSingleton<IScheduledTask, GetActivitiesAndContactsTask>();
                services.AddSingleton<IScheduledTask, SnapPartnersOfACountyTask>();
                services.AddSingleton<IScheduledTask, MessageProcessingTask>();
                services.AddSingleton<IScheduledTask,TrainingsRemindersTask>();

            } 
            services.TryAddSingleton<IConfiguration>(Configuration);
            services.AddScoped<IzEmpRptProfileRepository, zEmpRptProfileRepository>();
            services.AddScoped<IzEmpRoleTypeRepository, zEmpRoleTypeRepository>();
            services.AddScoped<IKersUserRepository, KersUserRepository>(); 
            services.AddScoped<ITrainingRepository, TrainingRepository>();
            services.AddScoped<INavSectionRepository, NavSectionRepository>();
            services.AddScoped<IInitiativeRepository, InitiativeRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IFiscalYearRepository, FiscalYearRepository>();
            services.AddScoped<IHelpContentRepository, HelpContentRepository>();
            services.AddScoped<IAffirmativeActionPlanRevisionRepository, AffirmativeActionPlanRevisionRepository>();
            services.AddScoped<ISnapDirectRepository, SnapDirectRepository>();
            services.AddScoped<ISnapPolicyRepository, SnapPolicyRepository>();
            services.AddScoped<ISnapFinancesRepository, SnapFinancesRepository>();
            services.AddScoped<ISnapCommitmentRepository, SnapCommitmentRepository>();
            services.AddScoped<IStoryRepository, StoryRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            
            services.AddScheduler((sender, args) =>
            {
                args.SetObserved();
            });
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment() || env.IsStaging())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
        }
    }
}
