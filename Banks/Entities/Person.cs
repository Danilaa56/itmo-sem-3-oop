using System;
using System.ComponentModel.DataAnnotations;

namespace Banks.Entities
{
    public class Person
    {
        private readonly string _name;
        private string _surname;

        public Person(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        [Key]
        public Guid Id { get; set; }

        public string Name
        {
            get => _name;
            init => _name = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Surname
        {
            get => _surname;
            set => _surname = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Address { get; set; }
        public string PassportId { get; set; }

        public void SendMessage(string message)
        {
            Console.WriteLine($"[Message for {Name} {Surname}]: {message}");
        }
    }
}