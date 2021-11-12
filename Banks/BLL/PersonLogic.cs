using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
using Banks.Tools;

namespace Banks.BLL
{
    public static class PersonLogic
    {
        public static int Create(string name, string surname, string address, string passportId)
        {
            var person = new Person(name, surname)
            {
                Address = address,
                PassportId = passportId,
            };
            using var db = new DataContext();
            db.Persons.Add(person);
            db.SaveChanges();
            return person.Id;
        }

        public static void ChangeAddress(int id, string newAddress)
        {
            using var db = new DataContext();
            db.PersonById(id).Address = newAddress;
            db.SaveChanges();
        }

        public static void ChangePassportId(int id, string newPassportId)
        {
            using var db = new DataContext();
            db.PersonById(id).PassportId = newPassportId;
            db.SaveChanges();
        }

        public static void Destroy(int id)
        {
            using var db = new DataContext();
            db.Persons.Remove(db.PersonById(id));
            db.SaveChanges();
        }

        public static List<Person> List()
        {
            using var db = new DataContext();
            return db.Persons.ToList();
        }

        public static Person PersonById(this DataContext db, int id)
        {
            return db.Persons.FirstOrDefault(person => person.Id == id) ??
                   throw new BankException("There is no person with such id");
        }

        public static void ThrowIfNotPresentPerson(this DataContext db, int personId)
        {
            if (!db.Persons.Any(person => person.Id == personId))
                throw new BankException("There is no person with such id");
        }
    }
}