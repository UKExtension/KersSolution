using System;
using System.IO;
using Kers.Models.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

class SoilDataContextDbContextFactory : IDesignTimeDbContextFactory<SoilDataContext>
{
    public SoilDataContext CreateDbContext(string[] args){
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
 
        var builder = new DbContextOptionsBuilder<SoilDataContext>();

        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if(environment == "development" || environment == "Development"){
            builder.UseSqlite(configuration["ConnectionStrings:soilDataLocal"], b => b.MigrationsAssembly("Kers"));
        }else{
            //builder.UseSqlServer(configuration["ConnectionStrings:connKersCore"], b => b.MigrationsAssembly("Kers"));
        }
        return new SoilDataContext(builder.Options);
    }
}