using System.Collections.Generic;
using System.Collections.Immutable;
using Shops.Tools;

namespace Shops.Entities
{
    public class PeopleRegistry
    {
        private Dictionary<string, Person> people = new Dictionary<string, Person>();

        public void Register(Person person)
        {
            if (people.ContainsKey(person.Id))
            {
                throw new ShopException("Person with such id already exists");
            }

            people[person.Id] = person;
        }

        public void Unregister(string id)
        {
            if (!people.Remove(id))
            {
                throw new ShopException("There is no person with such id");
            }
        }

        public Person GetById(string id)
        {
            if (people.TryGetValue(id, out Person person))
            {
                return person;
            }

            throw new ShopException("There is no person with such id");
        }

        public ImmutableList<Person> GetPeople()
        {
            return people.Values.ToImmutableList();
        }
    }
}