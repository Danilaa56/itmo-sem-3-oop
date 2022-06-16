using System;
using Reports.Core.Entities;

namespace Reports.WebAPI.Models
{
    public class PersonModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid DirectorId { get; set; }

        public static PersonModel ToModel(Person person)
        {
            return new PersonModel
            {
                Id = person.Id,
                Name = person.Name,
                Surname = person.Surname,
                DirectorId = person.Director?.Id ?? Guid.Empty
            };
        }
    }
}