using System.Collections.Generic;
using System.Linq;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.Infra.Data;

namespace Reports.Infra.Services
{
    public class PersonService : IPersonService
    {
        private readonly ReportsContext _context;
        
        public PersonService(ReportsContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Person> GetPersonsList()
        {
            return _context.Persons.ToList();
        }
    }
}