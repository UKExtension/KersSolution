using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Kers.Models.Contexts;
using Kers.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KersTasks
{
    class Program
    {
        private static IConfigurationRoot Configuration;
        static void Main(string[] args)
        {



            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            KERScoreContext context;
            IDistributedCache cache;
            if(environment == "development"){
                var optionsBuilder = new DbContextOptionsBuilder<KERScoreContext>();
                optionsBuilder.UseSqlite(Configuration["ConnectionStrings:connKersCoreLocal"]);
                context = new KERScoreContext(optionsBuilder.Options);
                var provider = new ServiceCollection()
                       .AddDistributedMemoryCache()
                       .BuildServiceProvider();
                cache = provider.GetService<IDistributedCache>();
            }else{
                var optionsBuilder = new DbContextOptionsBuilder<KERScoreContext>();
                optionsBuilder.UseSqlServer(Configuration["ConnectionStrings:connKERScore"]);
                context = new KERScoreContext(optionsBuilder.Options);
                var provider = new ServiceCollection()
                       .AddDistributedSqlServerCache(options =>
                                {
                                    options.ConnectionString = Configuration["ConnectionStrings:connKersCore"];
                                    options.SchemaName = "dbo";
                                    options.TableName = "Cache";
                                })
                       .BuildServiceProvider();
                cache = provider.GetService<IDistributedCache>();
            }
            var fiscalYearRepo = new FiscalYearRepository( context );

            var repo = new SnapDirectRepository(context, cache);
            

            var str = repo.TotalByMonth(fiscalYearRepo.currentFiscalYear("snapEd"), true);

            

            Type type = Assembly.GetEntryAssembly().GetType("KersTasks.Task");
            if(type == null)
                Console.WriteLine("Object type is NULL.");
            else
                Console.WriteLine("Object type has value.");

            ITask entity = (ITask) Activator.CreateInstance(type);
            entity.SetName( "Sample");

            Console.WriteLine("Hello World!" + str);
        }
    }

    class Task:ITask{
        public string Name;
        public string GetName(){
            return Name;
        }
        public void SetName(string Name){
            this.Name = Name;
        }
    }
    interface ITask{
        string GetName();
        void SetName(string Name);
    }
}
