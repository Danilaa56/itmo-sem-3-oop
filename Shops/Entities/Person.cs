using System;

namespace Shops.Entities
{
    public class Person
    {
        private decimal _money;

        public Person(string name, decimal money = 0)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (money < 0) throw new ArgumentException("Money cannot be below zero", nameof(money));
            Money = money;
        }

        public string Name { get; }

        public decimal Money
        {
            get => _money;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Money cannot be below zero", nameof(value));
                _money = value;
            }
        }
    }
}