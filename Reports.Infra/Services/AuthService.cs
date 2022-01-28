using System;
using System.Linq;
using Reports.Core.Entities;
using Reports.Core.Services;
using Reports.Infra.Data;
using Reports.Infra.Tools;

namespace Reports.Infra.Services
{
    public class AuthService : IAuthService
    {
        private readonly ReportsContext _context;

        public AuthService(ReportsContext context)
        {
            _context = context;
        }

        public Guid Login(string username, string password)
        {
            Person person = _context.Persons.FirstOrDefault(person => username.Equals(person.Username))
                            ?? throw new AuthException("There is no account with such username");

            if (!password.Equals(person.Password))
                throw new AuthException("Incorrect password");

            return person.Id;
        }

        public Guid Register(string username, string password, string name, string surname)
        {
            return Register(username, password, name, surname, false, Guid.Empty);
        }

        public Guid Register(string username, string password, string name, string surname, Guid directorId)
        {
            return Register(username, password, name, surname, true, directorId);
        }

        private Guid Register(string username, string password, string name, string surname, bool withDirector, Guid directorId)
        {
            if (_context.Persons.Any(person => username.Equals(person.Username)))
                throw new AuthException("There is another account with such username");

            Person? director = null;

            if (withDirector)
            {
                director = _context.Persons.FirstOrDefault(person => directorId.Equals(person.Id))
                           ?? throw new AuthException("Wrong director id: there is no person with such id");
            }

            Person newPerson = new ()
            {
                Name = name,
                Surname = surname,
                Username = username,
                Password = password,
                Director = director
            };

            _context.Persons.Add(newPerson);
            _context.SaveChanges();

            return newPerson.Id;
        }
    }
}