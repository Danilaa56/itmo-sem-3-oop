using System.Collections.Generic;
using System.Collections.Immutable;
using Banks.Entities;

namespace Banks
{
    public class PeopleRegistry
    {
        private List<Person> _persons = new List<Person>();

        public void Register(Person person)
        {
            _persons.Add(person);
        }

        public void Unregister(int index)
        {
            _persons.RemoveAt(index);
        }

        public ImmutableList<Person> Persons()
        {
            return _persons.ToImmutableList();
        }

        // private Dictionary<string, Person> people = new Dictionary<string, Person>();
        //
        // public Register(Person person)
        // {
        //     if (people.ContainsKey(person.Id))
        //     {
        //         throw new ShopException("Person with such id already exists");
        //     }
        //
        //     people[person.Id] = person;
        // }
        //
        // public void Unregister(string id)
        // {
        //     if (!people.Remove(id))
        //     {
        //         throw new ShopException("There is no person with such id");
        //     }
        // }
        //
        // public Person GetById(string id)
        // {
        //     if (people.TryGetValue(id, out Person person))
        //     {
        //         return person;
        //     }
        //
        //     throw new ShopException("There is no person with such id");
        // }
        //
        // public ImmutableList<Person> GetPeople()
        // {
        //     return people.Values.ToImmutableList();
        // }
    }
}