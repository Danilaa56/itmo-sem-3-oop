using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
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
    }
}