using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Tools;
using Microsoft.EntityFrameworkCore;

namespace Banks.BLL
{
    public static class BankLogic
    {
        public static int RegisterBank(
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
                anonLimit);
            bank.SetDepositLevels(depositPercentForRemainsLevels);
            using var db = new DataContext();
            db.Banks.Add(bank);
            db.SaveChanges();
            return bank.Id;
        }

        public static void UnregisterBank(int id)
        {
            using var db = new DataContext();
            db.Banks.Remove(db.BankById(id));
        }

        public static void Subscribe(int bankId, int personId)
        {
            using var db = new DataContext();
            db.BankById(bankId).SubscribeForUpdates(db.PersonById(personId));
            db.SaveChanges();
        }

        public static void Unsubscribe(int bankId, int personId)
        {
            using var db = new DataContext();
            db.BankById(bankId).UnsubscribeFromUpdates(db.PersonById(personId));
            db.SaveChanges();
        }

        public static List<Person> GetSubscribers(int bankId)
        {
            using var db = new DataContext();
            return db.BankById(bankId).Subscribers;
        }

        public static List<Bank> List()
        {
            using var db = new DataContext();
            return db.Banks.Include(bank => bank.Subscribers).ToList();
        }

        public static Bank BankById(this DataContext db, int id)
        {
            return db.Banks.Include(bank => bank.Subscribers).FirstOrDefault(bank => bank.Id == id) ??
                   throw new BankException("There is no bank with such id");
        }

        public static void ThrowIfNotPresentBank(this DataContext db, int bankId)
        {
            if (!db.Banks.Any(bank => bank.Id == bankId))
                throw new BankException("There is no person with such id");
        }

        public static bool CanTrust(Person person)
        {
            if (person is null)
                throw new ArgumentNullException(nameof(person));
            return person.Address is not null && person.PassportId is not null;
        }

        public static void ChangeDebitPercent(int bankId, decimal value)
        {
            using var db = new DataContext();
            Bank bank = db.BankById(bankId);
            bank.DebitPercentForRemains = value;

            db.DebitAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Debit percent for remains has changed: {value}"));

            db.SaveChanges();
        }

        public static void ChangeCreditLimit(int bankId, decimal value)
        {
            using var db = new DataContext();
            Bank bank = db.BankById(bankId);
            bank.CreditLimit = value;

            db.CreditAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Credit limit has changed: {value}"));

            db.SaveChanges();
        }

        public static void ChangeCreditCommission(int bankId, decimal value)
        {
            using var db = new DataContext();
            Bank bank = db.BankById(bankId);
            bank.CreditCommission = value;

            db.CreditAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Credit commission has changed: {value}"));

            db.SaveChanges();
        }

        public static void ChangeMinDepositPercent(int bankId, decimal value)
        {
            using var db = new DataContext();
            Bank bank = db.BankById(bankId);
            bank.MinDepositPercentForRemains = value;

            db.DepositAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Minimal deposit percent for remains has changed: {value}"));

            db.SaveChanges();
        }

        public static void ChangeDepositTime(int bankId, long value)
        {
            using var db = new DataContext();
            Bank bank = db.BankById(bankId);
            bank.DepositTimeMs = value;

            db.DepositAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Time to unlock deposit account has changed: {value}"));

            db.SaveChanges();
        }

        public static void ChangeAnonLimit(int bankId, decimal value)
        {
            using var db = new DataContext();
            Bank bank = db.BankById(bankId);
            bank.AnonLimit = value;

            db.UntrustedAccounts(bank).ForEach(account =>
                account.Account.Person.SendMessage($"Anonymous limit for untrusted accounts has changed: {value}"));

            db.SaveChanges();
        }

        private static List<BankAccount> DebitAccounts(this DataContext db, Bank bank)
        {
            return db.BankAccounts
                .FindAll(account => bank.Equals(account.Account.Bank)
                                    && bank.Subscribers.Contains(account.Account.Person)
                                    && account is DebitAccount);
        }

        private static List<BankAccount> CreditAccounts(this DataContext db, Bank bank)
        {
            return db.BankAccounts
                .FindAll(account => bank.Equals(account.Account.Bank)
                                    && bank.Subscribers.Contains(account.Account.Person)
                                    && account is CreditAccount);
        }

        private static List<BankAccount> DepositAccounts(this DataContext db, Bank bank)
        {
            return db.BankAccounts
                .FindAll(account => bank.Equals(account.Account.Bank)
                                    && bank.Subscribers.Contains(account.Account.Person)
                                    && account is DepositAccount);
        }

        private static List<BankAccount> UntrustedAccounts(this DataContext db, Bank bank)
        {
            return db.BankAccounts
                .FindAll(account => bank.Equals(account.Account.Bank)
                                    && bank.Subscribers.Contains(account.Account.Person)
                                    && !CanTrust(account.Account.Person));
        }
    }
}