using System;

namespace Banks.Entities
{
    public class Account
    {
        public Account(int id, Bank bank, Person person)
        {
            if (id < 1)
                throw new ArgumentException("Id must be positive");
            Id = id;
            Bank = bank ?? throw new ArgumentNullException(nameof(bank));
            Person = person ?? throw new ArgumentNullException(nameof(person));
        }

        public int Id { get; }
        public Bank Bank { get; }
        public Person Person { get; }

        public decimal Amount()
        {
            return Bank.CentralBank.GetAccountAmount(this);
        }
    }
}