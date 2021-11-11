using System.Collections.Generic;
using System.Linq;
using Banks.DTO;
using Banks.Entities;
using Banks.Tools;

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
            long depositTime,
            decimal anonLimit)
        {
            var bank = new Bank(
                name,
                debitPercentForRemains,
                creditLimit,
                creditCommission,
                minDepositPercentForRemains,
                depositTime,
                anonLimit);
            bank.SetDepositLevels(depositPercentForRemainsLevels);
            using var db = new BanksDbContext();
            db.Banks.Add(bank);
            db.SaveChanges();
            return bank.Id;
        }

        public static void Subscribe(int bankId, int personId)
        {
            using var db = new BanksDbContext();
            if (db.Subscribers.Any(subscriber => subscriber.Bank.Id == bankId && subscriber.Person.Id == personId))
                throw new BankException("Person is already subscribed for this bank updates");
            Bank bank = db.BankById(bankId);
            Person person = db.PersonById(personId);
            db.ThrowIfNotPresentBank(personId);
            db.Subscribers.Add(new PersonBankSubscriber() { Bank = bank, Person = person });
            db.SaveChanges();
        }

        public static void Unsubscribe(int bankId, int personId)
        {
            using var db = new BanksDbContext();
            if (db.Subscribers.Find(subscriber => subscriber.Bank.Id == bankId && subscriber.Person.Id == personId))
                throw new BankException("Person is already subscribed for this bank updates");
            return db.Banks.FirstOrDefault(bank => bank.Id == id) ??
                   throw new BankException("There is no bank with such id");
            Bank bank = db.BankById(bankId);
            Person person = db.PersonById(personId);
            db.ThrowIfNotPresentBank(personId);
            db.Subscribers.Add(new PersonBankSubscriber() { Bank = bank, Person = person });
            db.SaveChanges();
        }

        public static List<Person> GetSubscribers(int bankId)
        {
            using var db = new BanksDbContext();
            return db.Subscribers.Where(subscriber => subscriber.Bank.Id == bankId).Select(sub => sub.Person).ToList();
        }

        public static Bank BankById(this BanksDbContext db, int id)
        {
            return db.Banks.FirstOrDefault(bank => bank.Id == id) ??
                   throw new BankException("There is no bank with such id");
        }

        public static void ThrowIfNotPresentBank(this BanksDbContext db, int bankId)
        {
            if (!db.Banks.Any(bank => bank.Id == bankId))
                throw new BankException("There is no person with such id");
        }
    }
}