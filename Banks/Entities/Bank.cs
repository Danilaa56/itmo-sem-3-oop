using System;
using System.Collections.Generic;
using System.Text.Json;
using Banks.Tools;

namespace Banks.Entities
{
    public class Bank
    {
        private string _name;

        private decimal _debitPercentForRemains;
        private decimal _creditLimit;
        private decimal _creditCommission;
        private decimal _minDepositPercentForRemains;
        private Dictionary<decimal, decimal> _depositPercentForRemainsLevels;
        private long _depositTimeMsMs;
        private decimal _anonLimit;

        public Bank(
            string name,
            decimal debitPercentForRemains,
            decimal creditLimit,
            decimal creditCommission,
            decimal minDepositPercentForRemains,
            long depositTimeMs,
            decimal anonLimit)
        {
            Name = name;
            DebitPercentForRemains = debitPercentForRemains;
            CreditLimit = creditLimit;
            CreditCommission = creditCommission;
            MinDepositPercentForRemains = minDepositPercentForRemains;
            DepositTimeMs = depositTimeMs;
            AnonLimit = anonLimit;
        }

        public int Id { get; set; }

        public string Name
        {
            get => _name;
            set => _name = value ?? throw new ArgumentNullException(nameof(value));
        }

        public decimal DebitPercentForRemains
        {
            get => _debitPercentForRemains;
            set => _debitPercentForRemains = value;
        }

        public decimal CreditLimit
        {
            get => _creditLimit;
            set
            {
                if (value <= 0)
                    throw new ArithmeticException("Credit limit must be positive");
                _creditLimit = value;
            }
        }

        public decimal CreditCommission
        {
            get => _creditCommission;
            set
            {
                if (value <= 0)
                    throw new ArithmeticException("Credit commission cannot be negative");
                _creditCommission = value;
            }
        }

        public decimal MinDepositPercentForRemains
        {
            get => _minDepositPercentForRemains;
            set
            {
                _minDepositPercentForRemains = value;
            }
        }

        public string DepositPercentForRemainsLevels
        {
            get => JsonSerializer.Serialize(_depositPercentForRemainsLevels);
            set => _depositPercentForRemainsLevels = JsonSerializer.Deserialize<Dictionary<decimal, decimal>>(value);
        }

        public long DepositTimeMs
        {
            get => _depositTimeMsMs;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Deposit time cannot be negative");
                _depositTimeMsMs = value;
            }
        }

        public decimal AnonLimit
        {
            get => _anonLimit;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Anon limit cannot be negative");
                _anonLimit = value;
            }
        }

        public List<Person> Subscribers { get; set; } = new List<Person>();

        public decimal DepositPercentForRemains(decimal amount)
        {
            decimal percent = _minDepositPercentForRemains;
            foreach (KeyValuePair<decimal, decimal> level in _depositPercentForRemainsLevels)
            {
                if (amount >= level.Key)
                {
                    percent = level.Value;
                }
                else
                {
                    break;
                }
            }

            return percent;
        }

        public void SetDepositLevels(Dictionary<decimal, decimal> depositPercentForRemainsLevels)
        {
            _depositPercentForRemainsLevels = new Dictionary<decimal, decimal>(
                depositPercentForRemainsLevels ??
                throw new ArgumentNullException(nameof(depositPercentForRemainsLevels)));
        }

        public void SubscribeForUpdates(Person person)
        {
            if (Subscribers.Contains(person))
                throw new BankException("This person has been already subscribed");
            Subscribers.Add(person);
        }

        public void UnsubscribeFromUpdates(Person person)
        {
            if (!Subscribers.Remove(person))
                throw new BankException("This person has not been subscribed yet");
        }
    }
}