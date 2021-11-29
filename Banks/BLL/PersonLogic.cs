using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
using Banks.Tools;

namespace Banks.BLL
{
    public class PersonLogic
    {
        private readonly ApplicationContext _context;

        internal PersonLogic(ApplicationContext context)
        {
            _context = context;
        }

        public Guid Create(string name, string surname, string address, string passportId)
        {
            var person = new Person(name, surname)
            {
                Id = Guid.NewGuid(),
                Address = address,
                PassportId = passportId,
            };
            _context.Persons.Add(person);
            return person.Id;
        }

        public void ChangeAddress(Guid id, string newAddress)
        {
            ById(id).Address = newAddress;
        }

        public void ChangePassportId(Guid id, string newPassportId)
        {
            ById(id).PassportId = newPassportId;
        }

        public void Destroy(Guid id)
        {
            _context.Persons.Remove(ById(id));
        }

        public List<Person> List()
        {
            return _context.Persons.ToList();
        }

        internal Person ById(Guid id)
        {
            return _context.Persons.FirstOrDefault(person => person.Id == id) ??
                   throw new BankException("There is no person with such id");
        }
    }
}