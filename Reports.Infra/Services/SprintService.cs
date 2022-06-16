using System;
using System.Collections.Generic;
using System.Linq;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.Infra.Data;

namespace Reports.Infra.Services
{
    public class SprintService : ISprintService
    {
        private readonly ReportsContext _context;

        public SprintService(ReportsContext context)
        {
            _context = context;
        }
        
        public Guid CreateSprint(string name, DateTime start, DateTime finish)
        {
            Sprint sprint = new ()
            {
                Name = name,
                Start = start,
                Finish = finish
            };
            _context.Sprints.Add(sprint);
            _context.SaveChanges();

            return sprint.Id;
        }
        
        public Sprint GetSprintById(Guid id)
        {
            return _context.Sprints.FirstOrDefault(sprint => sprint.Id.Equals(id))
                   ?? throw new ArgumentException($"There is no sprint with such id {id}");
        }
        
        public IEnumerable<Sprint> GetSprintsList()
        {
            return _context.Sprints.ToList();
        }
    }
}