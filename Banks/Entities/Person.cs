using System;

namespace Banks.Entities
{
    public class Person
    {
        private Person(string name, string surname, string address, string passportId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Surname = surname ?? throw new ArgumentNullException(nameof(surname));
            Address = address;
            PassportId = passportId;
        }

        public string Name { get; }
        public string Surname { get; }
        public string Address { get; set; }
        public string PassportId { get; set; }

        public static PersonBuilder Builder(string name, string surname)
        {
            return new PersonBuilder(name, surname);
        }

        public void SendMessage(string message)
        {
            Console.WriteLine($"[Message for {Name} {Surname}]: {message}");
        }

        public class PersonBuilder
        {
            private string _name;
            private string _surname;
            private string _address;
            private string _passportId;

            public PersonBuilder(string name, string surname)
            {
                _name = name ?? throw new ArgumentNullException(nameof(name));
                _surname = surname ?? throw new ArgumentNullException(nameof(surname));
            }

            public PersonBuilder Address(string address)
            {
                _address = address;
                return this;
            }

            public PersonBuilder PassportId(string passportId)
            {
                _passportId = passportId;
                return this;
            }

            public Person Build()
            {
                return new Person(_name, _surname, _address, _passportId);
            }
        }
    }
}