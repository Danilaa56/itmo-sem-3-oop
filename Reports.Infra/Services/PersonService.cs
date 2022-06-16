using System;
using System.Collections.Generic;
using System.Linq;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.Infra.Data;
using Reports.Infra.Tools;

namespace Reports.Infra.Services
{
    public class PersonService : IPersonService
    {
        private readonly ReportsContext _context;

        public PersonService(ReportsContext context)
        {
            _context = context;
        }

        public Guid CreatePersonTeamLeader(string name, string surname)
        {
            if (_context.Persons.Any())
            {
                throw new ReportException("There can be only one team leader");
            }

            Person person = new ()
            {
                Name = name,
                Surname = surname,
            };
            _context.Persons.Add(person);
            _context.SaveChanges();
            return person.Id;
        }

        public Guid CreatePerson(string name, string surname, Guid directorId)
        {
            Person director = GetPersonById(directorId);
            Person person = new ()
            {
                Name = name,
                Surname = surname,
                Director = director,
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
            if (personId == directorId)
            {
                throw new ReportException("Worker cannot be his/her director");
            }
            Person person = GetPersonById(personId);
            Person director = GetPersonById(directorId);
            if (!OneCanBeDirectorOfOther(director, person))
            {
                throw new ReportException("Cyclic depends");
            }
            _context.Persons.Update(person);
            _context.SaveChanges();
        }

        public IEnumerable<Person> GetWorkers(Guid directorId)
        {
            return _context.Persons
                .Where(person => person.Director != null && person.Director.Id.Equals(directorId))
                .ToList();
        }

        public Person GetPersonById(Guid id)
        {
            return _context.Persons.FirstOrDefault(person => id.Equals(person.Id))
                   ?? throw new ArgumentException($"There is no person with id {id}");
        }

        private bool OneCanBeDirectorOfOther(Person director, Person person)
        {
            Person? tmp = director;
            while (tmp != null)
            {
                tmp = tmp.Director;
                if (tmp == person)
                {
                    return false;
                }
            }
            return true;
        }
    }
}