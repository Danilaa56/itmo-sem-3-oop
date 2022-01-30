using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reports.Core.Entities;
using Reports.Infra.Data;

namespace Reports.WebAPI
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

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Person person = new Person()
                {
                    Id = new Guid(56, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                    Name = "PersonName",
                    Surname = "Surname",
                    Password = "qwe",
                    Username = "qwe"
                };
                context.Persons.Add(person);
                Sprint sprint = new Sprint()
                {
                    Name = "Test sprint",
                    Start = DateTime.Now,
                    Finish = DateTime.Now.AddDays(1)
                };
                context.Sprints.Add(sprint);
                context.Problems.Add(new Problem()
                {
                    Title = "Title 1",
                    Author = person,
                    Sprint = sprint,
                    Content = "Description for the first problem",
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });
                context.Problems.Add(new Problem()
                {
                    Title = "Title 2 but it is very long (or not)",
                    Author = person,
                    Sprint = sprint,
                    Content = "Description for the second problem",
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });
                context.Problems.Add(new Problem()
                {
                    Title = "Title 3",
                    Author = person,
                    Sprint = sprint,
                    Executor = person,
                    Content = "Description for the third problem",
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                });
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
                logger.LogError(ex.StackTrace);
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}