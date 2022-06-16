using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reports.Core.Services;
using Reports.Infra.Data;
using Reports.Infra.Services;

namespace Reports.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("Sqlite")
                                      ?? throw new Exception("DB Connection string is null");
            services.AddDbContext<ReportsContext>(options => options.UseSqlite(connectionString));

            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISprintService, SprintService>();
            services.AddScoped<IHistoryService, HistoryService>();
            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<IReportService, ReportService>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}