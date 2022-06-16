using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.Infra.Data;
using Reports.Infra.Tools;

namespace Reports.Infra.Services
{
    public class ReportService : IReportService
    {
        private readonly ReportsContext _context;
        private readonly IPersonService _personService;
        private readonly IProblemService _problemService;
        private readonly ISprintService _sprintService;

        public ReportService(ReportsContext context, ISprintService sprintService, IPersonService personService, IProblemService problemService)
        {
            _context = context;
            _sprintService = sprintService;
            _personService = personService;
            _problemService = problemService;
        }

        public Guid InitReport(Guid sprintId, Person actor)
        {
            Sprint sprint = _sprintService.GetSprintById(sprintId);
            Report report = new ()
            {
                Author = actor,
                Content = "",
                Sprint = sprint,
                LinkedProblems = new List<Problem>(),
                LinkedReports = new List<Report>(),
                IsEditable = true,
            };
            _context.Reports.Add(report);
            _context.SaveChanges();
            return report.Id;
        }

        public Report GetReportById(Guid id)
        {
            return _context.Reports
                       .Include(report => report.LinkedProblems)
                       .Include(report => report.LinkedReports)
                       .Include(report => report.Author)
                       .Include(report => report.Sprint)
                       .FirstOrDefault(report => report.Id.Equals(id))
                   ?? throw new ArgumentException($"There is no report with id {id}");
        }

        public void BakeReport(Guid id, Person actor)
        {
            Report report = GetReportById(id);
            ValidateCanEdit(report, actor);
            report.IsEditable = false;
            _context.Reports.Update(report);
            _context.SaveChanges();
        }

        public IEnumerable<Report> FindReportsByDirector(Guid directorId, Guid sprintId)
        {
            return _context.Reports
                .Include(report => report.LinkedProblems)
                .Include(report => report.LinkedReports)
                .Include(report => report.Author)
                .Include(report => report.Sprint)
                .Where(report => report.Sprint.Id.Equals(sprintId))
                .Where(report => report.Author.Director != null && report.Author.Director.Id == directorId)
                .Where(report => !report.IsEditable)
                .ToList();
        }

        public IEnumerable<Person> FindWorkersWithoutReport(Guid directorId, Guid sprintId)
        {
            IEnumerable<Person> withReports = FindReportsByDirector(directorId, sprintId).Select(report => report.Author);
            return _personService.GetWorkers(directorId).Except(withReports).ToList();
        }

        public void LinkProblem(Guid id, Guid problemId, Person actor)
        {
            Report report = GetReportById(id);
            ValidateCanEdit(report, actor);

            Problem problem = _problemService.GetProblemById(problemId);
            if (report.LinkedProblems.Contains(problem))
            {
                throw new ReportException("Problem is already linked to this report");
            }

            report.LinkedProblems.Add(problem);
            _context.Reports.Update(report);
            _context.SaveChanges();
        }

        public void UnlinkProblem(Guid id, Guid problemId, Person actor)
        {
            Report report = GetReportById(id);
            ValidateCanEdit(report, actor);

            Problem problem = _problemService.GetProblemById(problemId);
            if (!report.LinkedProblems.Remove(problem))
            {
                throw new ReportException("Problem is not linked to this report");
            }

            _context.Reports.Update(report);
            _context.SaveChanges();
        }

        public void LinkReport(Guid id, Guid reportId, Person actor)
        {
            Report report = GetReportById(id);
            ValidateCanEdit(report, actor);

            Report other = GetReportById(reportId);
            if (report.LinkedReports.Contains(other))
            {
                throw new ReportException("Report is already linked to this report");
            }

            report.LinkedReports.Add(other);
            _context.Reports.Update(report);
            _context.SaveChanges();
        }

        public void UnlinkReport(Guid id, Guid reportId, Person actor)
        {
            Report report = GetReportById(id);
            ValidateCanEdit(report, actor);

            Report other = GetReportById(reportId);
            if (!report.LinkedReports.Remove(other))
            {
                throw new ReportException("Report is not linked to this report");
            }

            _context.Reports.Update(report);
            _context.SaveChanges();
        }

        public void Edit(Guid id, string content, Person actor)
        {
            Report report = GetReportById(id);
            ValidateCanEdit(report, actor);

            report.Content = content;
            _context.Reports.Update(report);
            _context.SaveChanges();
        }

        private void ValidateCanEdit(Report report, Person actor)
        {
            if (!report.IsEditable)
            {
                throw new InvalidOperationException("Report is not editable");
            }
            if (report.Author != actor)
            {
                throw new ArgumentException("Only author can edit report");
            }
        }
    }
}