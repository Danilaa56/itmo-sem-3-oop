using System;
using System.Collections.Generic;
using Reports.Core.Entities;

namespace Reports.Core.Services
{
    public interface ISprintService
    {
        Guid CreateSprint(string name, DateTime start, DateTime finish);
        Sprint GetSprintById(Guid id);
        IEnumerable<Sprint> GetSprintsList();
    }
}