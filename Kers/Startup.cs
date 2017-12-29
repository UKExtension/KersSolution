using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kers
{
    public class Startup
    {
        private string secretKey;
        private IHostingEnvironment CurrentEnvironment;
        public Startup(IHostingEnvironment env)
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
            services.AddMemoryCache();
            services.AddMvc();

            services.AddMvc()
                    .AddJsonOptions(jsonOptions=>
                {
                    jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    jsonOptions.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });


              /*  
            if(CurrentEnvironment.IsEnvironment("Development")){
                services.AddDbContext<KERSmainContext>(options => 
                    options.UseSqlite(Configuration["ConnectionStrings:connKersMainLocal"]));
                services.AddDbContext<KERScoreContext>(options => 
                    options.UseSqlite(Configuration["ConnectionStrings:connKersCoreLocal"]));
                services.AddDbContext<KERS_SNAPED2017Context>(options => 
                   options.UseSqlite(Configuration["ConnectionStrings:connKersSnapLocal"]));

                services.AddDistributedMemoryCache();
            }else{
                 
                services.AddDbContext<KERSmainContext>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKersMain"]));
                services.AddDbContext<KERScoreContext>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKersCore"]));
                services.AddDbContext<KERS2017Context>(options => 
                    options.UseSqlServer(Configuration["ConnectionStrings:connKERS2017"]));
                
                
                services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = Configuration["ConnectionStrings:connKersCore"];
                        options.SchemaName = "dbo";
                        options.TableName = "Cache";
                    });
            }
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<IzEmpRptProfileRepository, zEmpRptProfileRepository>();
            services.AddScoped<IzEmpRoleTypeRepository, zEmpRoleTypeRepository>();
            services.AddScoped<IKersUserRepository, KersUserRepository>();
            services.AddScoped<INavSectionRepository, NavSectionRepository>();
            services.AddScoped<IInitiativeRepository, InitiativeRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IFiscalYearRepository, FiscalYearRepository>();
            services.AddScoped<IHelpContentRepository, HelpContentRepository>();
            services.AddScoped<IAffirmativeActionPlanRevisionRepository, AffirmativeActionPlanRevisionRepository>();

            services.AddScoped<IMembershipService, MembershipService>();
*/
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();





        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
