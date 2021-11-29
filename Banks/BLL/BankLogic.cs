using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Tools;

namespace Banks.BLL
{
    public class BankLogic
    {
        private readonly ApplicationContext _context;

        internal BankLogic(ApplicationContext context)
        {
            _context = context;
        }

        public Guid RegisterBank(
            string name,
            decimal debitPercentForRemains,
            decimal creditLimit,
            decimal creditCommission,
            decimal minDepositPercentForRemains,
            Dictionary<decimal, decimal> depositPercentForRemainsLevels,
            long depositTimeMs,
            decimal anonLimit)
        {
            var bank = new Bank(
                name,
                debitPercentForRemains,
                creditLimit,
                creditCommission,
                minDepositPercentForRemains,
                depositTimeMs,
                anonLimit)
            {
                Id = Guid.NewGuid(),
            };
            bank.SetDepositLevels(depositPercentForRemainsLevels);

            _context.Banks.Add(bank);

            return bank.Id;
        }

        public void UnregisterBank(Guid id)
        {
            _context.Banks.Remove(ById(id));
        }

        public void Subscribe(Guid bankId, Guid personId)
        {
            ById(bankId).SubscribeForUpdates(_context.Person.ById(personId));
        }

        public void Unsubscribe(Guid bankId, Guid personId)
        {
            ById(bankId).UnsubscribeFromUpdates(_context.Person.ById(personId));
        }

        public List<Person> GetSubscribers(Guid bankId)
        {
            return ById(bankId).Subscribers.ToList();
        }

        public List<Bank> List()
        {
            return _context.Banks.ToList();
        }

        public Bank ById(Guid id)
        {
            return _context.Banks.FirstOrDefault(bank => bank.Id == id) ??
                   throw new BankException("There is no bank with such id");
        }

        public bool CanTrust(Person person)
        {
            if (person is null)
                throw new ArgumentNullException(nameof(person));
            return person.Address is not null && person.PassportId is not null;
        }

        public void ChangeDebitPercent(Guid bankId, decimal value)
        {
            Bank bank = ById(bankId);
            bank.DebitPercentForRemains = value;

            DebitAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Debit percent for remains has changed: {value}"));
        }

        public void ChangeCreditLimit(Guid bankId, decimal value)
        {
            Bank bank = ById(bankId);
            bank.CreditLimit = value;

            CreditAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Credit limit has changed: {value}"));
        }

        public void ChangeCreditCommission(Guid bankId, decimal value)
        {
            Bank bank = ById(bankId);
            bank.CreditCommission = value;

            CreditAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Credit commission has changed: {value}"));
        }

        public void ChangeMinDepositPercent(Guid bankId, decimal value)
        {
            Bank bank = ById(bankId);
            bank.MinDepositPercentForRemains = value;

            DepositAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Minimal deposit percent for remains has changed: {value}"));
        }

        public void ChangeDepositTime(Guid bankId, long value)
        {
            Bank bank = ById(bankId);
            bank.DepositTimeMs = value;

            DepositAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Time to unlock deposit account has changed: {value}"));
        }

        public void ChangeAnonLimit(Guid bankId, decimal value)
        {
            Bank bank = ById(bankId);
            bank.AnonLimit = value;

            UntrustedAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Anonymous limit for untrusted accounts has changed: {value}"));
        }

        private List<DebitAccount> DebitAccounts(Bank bank)
        {
            return _context.DebitAccounts
                .FindAll(account => bank.Equals(account.Account.Bank)
                                    && bank.Subscribers.Contains(account.Account.Person));
        }

        private List<CreditAccount> CreditAccounts(Bank bank)
        {
            return _context.CreditAccounts
                .FindAll(account => bank.Equals(account.Account.Bank)
                                    && bank.Subscribers.Contains(account.Account.Person));
        }

        private List<DepositAccount> DepositAccounts(Bank bank)
        {
            return _context.DepositAccounts
                .FindAll(account => bank.Equals(account.Account.Bank)
                                    && bank.Subscribers.Contains(account.Account.Person));
        }

        private List<BankAccount> UntrustedAccounts(Bank bank)
        {
            return _context.BankAccounts.FindAll(account => bank.Equals(account.Account.Bank)
                                                   && bank.Subscribers.Contains(account.Account.Person)
                                                   && !CanTrust(account.Account.Person));
        }
    }
}