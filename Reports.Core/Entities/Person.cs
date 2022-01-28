using System;

namespace Reports.Core.Entities
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Person? Director { get; set; }
    }
}