using System;
using System.Collections.Generic;
using Reports.Core.Entities;

namespace Reports.WebAPI.Models
{
    public class ReportModel
    {
        public Guid Id { get; set; }
        public PersonModel Author { get; set; }
        public Sprint Sprint { get; set; }
        public string Content { get; set; }
        public List<ProblemModel> LinkedProblems { get; set; }
        public List<Guid> LinkedReports { get; set; }
        public bool IsEditable { get; set; }

        public static ReportModel ToModel(Report report)
        {
            return new ReportModel
            {
                Id = report.Id,
                Author = PersonModel.ToModel(report.Author),
                Sprint = report.Sprint,
                Content = report.Content,
                LinkedProblems = report.LinkedProblems.ConvertAll(ProblemModel.ToModel),
                LinkedReports = report.LinkedReports.ConvertAll(other => other.Id),
                IsEditable = report.IsEditable,
            };
        }
    }
}