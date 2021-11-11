using System;

namespace Banks.Entities
{
    public class Account
    {
        private Bank _bank;
        private Person _person;

        public int Id { get; set; }

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

        // public decimal Amount()
        // {
        //     return Bank.CentralBank.GetAccountAmount(this);
        // }
    }
}