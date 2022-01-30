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
                IAuthService authService = services.GetRequiredService<IAuthService>();
                IReportService reportService = services.GetRequiredService<IReportService>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Guid teamLeadId = personService.CreatePersonTeamLeader("John", "Snow");
                Guid worker1Id = personService.CreatePerson("Worker", "Of John", teamLeadId);
                Guid worker2Id = personService.CreatePerson("Worker 2", "Of John", teamLeadId);
                Guid worker11Id = personService.CreatePerson("Worker 1", "true 1", worker1Id);

                Guid token = authService.CreateToken(teamLeadId, "MyQWERTYPassword");
                Person johnSnow = authService.PersonByToken(token);
                Guid sprintId = sprintService.CreateSprint("Test sprint", DateTime.Now, DateTime.Now.AddDays(1));

                Guid problem1 = problemService.CreateProblem("Title 1", "Description for the first problem", sprintId, johnSnow);
                Guid problem2 = problemService.CreateProblem("Title 2 but it is very long (or not)",
                "Description for the second problem",
                sprintId,
                johnSnow);
                Guid problem3 = problemService.CreateProblem("Title 3", "Description for the third problem", sprintId, johnSnow);
                problemService.SetExecutor(problem3, teamLeadId, johnSnow);
                problemService.AddComment(problem2, "Test comment", johnSnow);

                Guid reportId = reportService.InitReport(sprintId, johnSnow);
                reportService.LinkProblem(reportId, problem1, johnSnow);
                reportService.LinkProblem(reportId, problem2, johnSnow);
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