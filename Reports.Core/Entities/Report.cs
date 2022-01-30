using System;
using System.Collections.Generic;

namespace Reports.Core.Entities
{
    public class Report
    {
        public Guid Id { get; set; }
        public Person Author { get; set; }
        public Sprint Sprint { get; set; }
        public string Content { get; set; }
        public List<Problem> LinkedProblems { get; set; }
        public List<Report> LinkedReports { get; set; }
        public bool IsEditable { get; set; }
    }
}