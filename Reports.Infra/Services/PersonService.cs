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

        public Guid CreatePerson(string name, string surname)
        {
            Person person = new ()
            {
                Name = name,
                Surname = surname
            };
            _context.Persons.Add(person);
            _context.SaveChanges();
            return person.Id;
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

        public void SetDirector(Guid personId, Guid directorId)
        {
            Person person = GetPersonById(personId);
            person.Director = directorId == Guid.Empty ? null : GetPersonById(directorId);
            _context.Persons.Update(person);
            _context.SaveChanges();
        }

        public IEnumerable<Person> GetWorkers(Guid directorId)
        {
            return _context.Persons
                .Where(person => person.Director != null && person.Director.Id.Equals(directorId))
                .ToList();
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