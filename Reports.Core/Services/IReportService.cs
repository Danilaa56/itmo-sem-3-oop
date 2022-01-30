using System;
using System.Collections.Generic;
using Reports.Core.Entities;

namespace Reports.Core.Services
{
    public interface IReportService
    {
        Guid InitReport(Guid sprintId, Person actor);
        Report GetReportById(Guid id);
        void BakeReport(Guid id, Person actor);
        IEnumerable<Report> FindReportsByDirector(Guid directorId, Guid sprintId);
        IEnumerable<Person> FindWorkersWithoutReport(Guid directorId, Guid sprintId);
        void LinkProblem(Guid id, Guid problemId, Person actor);
        void UnlinkProblem(Guid id, Guid problemId, Person actor);
        void LinkReport(Guid id, Guid reportId, Person actor);
        void UnlinkReport(Guid id, Guid reportId, Person actor);
        void Edit(Guid id, string content, Person actor);
    }
}