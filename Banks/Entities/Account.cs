using System;

namespace Banks.Entities
{
    public class Account
    {
        private Bank _bank;
        private Person _person;

        public Account(Guid id, Bank bank, Person person)
        {
            Id = id;
            Bank = bank;
            Person = person;
        }

        private Account()
        {
        }

        public Guid Id { get; set; }

        public Bank Bank
        {
            get => _bank;
            set => _bank = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Person Person
        {
            get => _person;
            set => _person = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}