using System;
using System.Linq;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Tools;

namespace Banks.BLL
{
    public static class AccountLogic
    {
        public static int Create(int bankId, int personId, BankAccountType accountType)
        {
            using var db = new DataContext();
            Bank bank = db.BankById(bankId);
            Person person = db.PersonById(personId);

            var account = new Account
            {
                Bank = bank,
                Person = person,
            };
            db.Accounts.Add(account);

            switch (accountType)
            {
                case BankAccountType.Debit:
                    db.DebitAccounts.Add(new DebitAccount(account));
                    break;
                case BankAccountType.Credit:
                    db.CreditAccounts.Add(new CreditAccount(account));
                    break;
                case BankAccountType.Deposit:
                    db.DepositAccounts.Add(new DepositAccount(account, db.CurrentTimeMillis));
                    break;
                default:
                    throw new BankException("Invalid account type");
            }

            return account.Id;
        }

        public static void Destroy(int accountId)
        {
            using var db = new DataContext();
            Account account = db.AccountById(accountId);
            Console.WriteLine(account.GetType());

            //     // BankAccount account =
            //     //     db.DebitAccounts.FirstOrDefault(account => account.Id == accountId)
            //     //     ?? db.CreditAccounts.FirstOrDefault(account => account.Id == accountId)
            //     //     ?? db.DepositAccounts.FirstOrDefault(account => account.Id == accountId);
            // db.CreditAccounts.Remove(db.AccountById(accountId));
            // Bank bank = db.BankById(bankId);
            // Person person = db.PersonById(personId);
            //
            // var account = new Account
            // {
            //     Bank = bank,
            //     Person = person,
            // };
            // db.Accounts.Add(account);
            //
            // switch (accountType)
            // {
            //     case BankAccountType.Debit:
            //         db.DebitAccounts.Add(new DebitAccount(account));
            //         break;
            //     case BankAccountType.Credit:
            //         db.CreditAccounts.Add(new CreditAccount(account));
            //         break;
            //     case BankAccountType.Deposit:
            //         db.DepositAccounts.Add(new DepositAccount(account, db.CurrentTimeMillis));
            //         break;
            //     default:
            //         throw new BankException("Invalid account type");
            // }

            // return account.Id;
        }

        // public static void ChangeAddress(int id, string newAddress)
        // {
        //     using var db = new DataContext();
        //     db.PersonById(id).Address = newAddress;
        //     db.SaveChanges();
        // }
        //
        // public static void ChangePassportId(int id, string newPassportId)
        // {
        //     using var db = new DataContext();
        //     db.PersonById(id).PassportId = newPassportId;
        //     db.SaveChanges();
        // }
        //
        // public static void Destroy(int id)
        // {
        //     using var db = new DataContext();
        //     db.Persons.Remove(db.PersonById(id));
        //     db.SaveChanges();
        // }
        //
        // public static List<Person> List()
        // {
        //     using var db = new DataContext();
        //     return db.Persons.ToList();
        // }

        public static Account AccountById(this DataContext db, int id)
        {
            return db.Accounts.FirstOrDefault(account => account.Id == id) ??
                   throw new BankException("There is no account with such id");
        }
    }
}