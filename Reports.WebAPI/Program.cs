using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reports.Core.Entities;
using Reports.Core.Services;
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
                IPersonService personService = services.GetRequiredService<IPersonService>();
                IProblemService problemService = services.GetRequiredService<IProblemService>();
                ISprintService sprintService = services.GetRequiredService<ISprintService>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                Person person = new Person()
                {
                    Id = new Guid(56, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
                    Name = "John",
                    Surname = "Snow",
                    Password = "qwe",
                    Username = "qwe",
                };
                context.Persons.Add(person);
                context.SaveChanges();
                Guid sprintId = sprintService.CreateSprint("Test sprint", DateTime.Now, DateTime.Now.AddDays(1));

                Guid problem1 = problemService.CreateProblem("Title 1", "Description for the first problem", person.Id, sprintId);
                Guid problem2 = problemService.CreateProblem("Title 2 but it is very long (or not)",
                "Description for the second problem",
                person.Id,
                sprintId);
                Guid problem3 = problemService.CreateProblem("Title 3", "Description for the third problem", person.Id, sprintId);
                problemService.SetExecutor(problem3, person.Id);
                problemService.WriteComment(problem2, person.Id, "Test comment");
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