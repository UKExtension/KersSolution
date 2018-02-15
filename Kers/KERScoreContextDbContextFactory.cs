using System;
using System.IO;
using Kers.Models.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

class KERScoreContextDbContextFactory : IDesignTimeDbContextFactory<KERScoreContext>
{
    public KERScoreContext CreateDbContext(string[] args){
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
 
        var builder = new DbContextOptionsBuilder<KERScoreContext>();

        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if(environment == "development"){
            builder.UseSqlite(configuration["ConnectionStrings:connKersCoreLocal"], b => b.MigrationsAssembly("Kers"));
        }else{
            builder.UseSqlServer(configuration["ConnectionStrings:connKersCore"], b => b.MigrationsAssembly("Kers"));
        }
        return new KERScoreContext(builder.Options);
    }
}