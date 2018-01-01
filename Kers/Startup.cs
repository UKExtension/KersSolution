using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Kers.Models.Abstract;
using Kers.Models.Contexts;
using Kers.Models.Repositories;
using Kers.Services;
using Kers.Services.Abstract;
using Kers.Services.Midleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

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

            services.AddMvc()
                    .AddJsonOptions(jsonOptions=>
                {
                    jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    jsonOptions.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            if(CurrentEnvironment.IsDevelopment()){
                // Development
                services.AddDbContext<KERSmainContext>(options => 
                    options.UseSqlite(Configuration["ConnectionStrings:connKersMainLocal"]));
                services.AddDbContext<KERScoreContext>(options => 
                    options.UseSqlite(Configuration["ConnectionStrings:connKersCoreLocal"]));
                services.AddDbContext<KERS_SNAPED2017Context>(options => 
                   options.UseSqlite(Configuration["ConnectionStrings:connKersSnapLocal"]));

                services.AddDistributedMemoryCache();
            }else if(CurrentEnvironment.IsStaging()){
                // Staging
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
            }else if( CurrentEnvironment.IsProduction()){
                //Production
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
            app.UseAuthentication();


/*
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            app.UseSimpleTokenProvider(new TokenProviderOptions
            {
                Path = "/api/token",
                Audience = "KersUsers",
                Issuer = "KERSSystem",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
                IdentityResolver = GetIdentity
            });


            var tokenValidationParameters = new TokenValidationParameters
            {
                // The signing key must match!
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                // Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = "KERSSystem",

                // Validate the JWT Audience (aud) claim
                ValidateAudience = true,
                ValidAudience = "KersUsers",

                // Validate the token expiry
                ValidateLifetime = true,
                
                // If you want to allow a certain amount of clock drift, set that here:
                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
 */
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

        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            // Don't do this in production, obviously!
            if(this.CurrentEnvironment.IsEnvironment("Development") ){
                if (username == "random"){
                    return await Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { }));
                }
            }
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            String uri = "https://kers.ca.uky.edu/kers_mobile/Handler.ashx";

            Dictionary<string, string> pairs = new Dictionary<string,string>();
            pairs.Add("username", username);
            pairs.Add("password", password);
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(pairs);

            var result = client.PostAsync(uri, formContent).Result;
            var data = result.Content.ReadAsStringAsync().Result;

            if( data == "{\"valid\":true}"){
                return await Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token"), new Claim[] { }));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
