using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reports.Core.Entities;
using Reports.Infra.Data;

namespace Reports.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            CreateDbIfNotExists(host);
            host.Run();
        }

        private static void CreateDbIfNotExists(IHost host)
        {
            using IServiceScope scope = host.Services.CreateScope();
            IServiceProvider services = scope.ServiceProvider;
            try
            {
                ReportsContext context = services.GetRequiredService<ReportsContext>();
                // context.Database.EnsureCreated();
                // context.SaveChanges();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Persons.Add(new Person()
                {
                    Id = new Guid(56, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                    Name = "PersonName",
                    Surname = "Surname",
                    Password = "qwe",
                    Username = "qwe"
                });
                context.SaveChanges();
                
                
            }
            catch (Exception ex)
            {
                ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}