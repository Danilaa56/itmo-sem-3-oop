using System;
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

        public void EditPerson(Guid personId, string newName, string newSurname)
        {
            Person person = GetPersonById(personId);
            person.Name = newName;
            person.Surname = newSurname;
            _context.Persons.Update(person);
            _context.SaveChanges();
        }

        public void DeletePerson(Guid id)
        {
            _context.Persons.Remove(GetPersonById(id));
            _context.SaveChanges();
        }

        public Person GetPersonById(Guid id)
        {
            return _context.Persons.FirstOrDefault(person => id.Equals(person.Id))
                   ?? throw new ArgumentException($"There is no person with id {id}");
        }
    }
}