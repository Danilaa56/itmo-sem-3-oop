#nullable enable
using System.Linq;
using Backups.Tools;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Tools;
using Microsoft.EntityFrameworkCore;

namespace Banks.BLL
{
    public static class AccountLogic
    {
        public static int CreateDeposit(int bankId, int personId)
        {
            using var db = new DataContext();
            Account account = Create(db, bankId, personId);

            var bankAccount = new DepositAccount
            {
                Account = account,
                UnlockTimeMs = TimeLogic.CurrentTimeMillis() + account.Bank.DepositTimeMs,
            };

            db.DepositAccounts.Add(bankAccount);
            db.SaveChanges();
            return bankAccount.Account.Id;
        }

        public static int CreateDebit(int bankId, int personId)
        {
            using var db = new DataContext();
            Account account = Create(db, bankId, personId);

            var bankAccount = new DebitAccount()
            {
                Account = account,
            };

            db.DebitAccounts.Add(bankAccount);
            db.SaveChanges();
            return bankAccount.Account.Id;
        }

        public static int CreateCredit(int bankId, int personId)
        {
            using var db = new DataContext();
            Account account = Create(db, bankId, personId);

            var bankAccount = new CreditAccount()
            {
                Account = account,
            };

            db.CreditAccounts.Add(bankAccount);
            db.SaveChanges();
            return bankAccount.Account.Id;
        }

        public static int Correction(DataContext db, int accountId, decimal correction)
        {
            BankAccount bankAccount = db.BankAccountById(accountId);

            var transaction = new TransactionCorrection
            {
                Id = db.NextTransactionId(),
                Account = bankAccount.Account,
                Correction = correction,
                DateUtcMs = TimeLogic.CurrentTimeMillis(),
            };
            db.CorrectionTransactions.Add(transaction);

            db.SaveChanges();
            return transaction.Id;
        }

        public static int Destroy(int accountId)
        {
            using var db = new DataContext();
            Account account = db.AccountById(accountId);

            if (!TryDestroyAccount(db.CreditAccounts, accountId)
                || !TryDestroyAccount(db.DebitAccounts, accountId)
                || !TryDestroyAccount(db.DepositAccounts, accountId))
            {
                throw new BankException("There is no account with such id");
            }

            var transaction = new TransactionDestroy()
            {
                Id = db.NextTransactionId(),
                Account = account,
                DateUtcMs = TimeLogic.CurrentTimeMillis(),
            };
            db.DestroyTransactions.Add(transaction);

            db.Accounts.Remove(db.AccountById(accountId));
            db.SaveChanges();
            return transaction.Id;
        }

        public static int TopUp(int accountId, decimal amount)
        {
            using var db = new DataContext();

            BankAccount bankAccount = db.BankAccountById(accountId);

            var transaction = new TransactionTopUp
            {
                Id = db.NextTransactionId(),
                Account = bankAccount.Account,
                Amount = amount,
                Commission = bankAccount.CommissionTopUp(db.AmountAt(accountId), amount),
                DateUtcMs = TimeLogic.CurrentTimeMillis(),
            };
            db.TopUpTransactions.Add(transaction);

            db.SaveChanges();
            return transaction.Id;
        }

        public static int Transfer(int senderId, int receiverId, decimal amount)
        {
            using var db = new DataContext();
            BankAccount senderAccount = db.BankAccountById(senderId);
            BankAccount receiverAccount = db.BankAccountById(receiverId);

            decimal amountAtSender = db.AmountAt(senderId);
            decimal amountAtReceiver = db.AmountAt(receiverId);

            decimal amountAvailable = senderAccount.AmountAvailable(amountAtSender);
            if (amountAvailable < amount)
                throw new BackupException("There is no such amount of money available for this account");

            CheckTrust(senderAccount.Account, amount);

            var transaction = new TransactionTransfer
            {
                DateUtcMs = TimeLogic.CurrentTimeMillis(),
                Account = senderAccount.Account,

                Amount = amount,
                Commission = senderAccount.CommissionWithdraw(amountAtSender, amount),
                ReceiverAmount = amount,
                ReceiverCommission = receiverAccount.CommissionTopUp(amountAtReceiver, amount),
            };
            db.TransferTransactions.Add(transaction);

            db.SaveChanges();
            return transaction.Id;
        }

        public static int Withdraw(int accountId, decimal amount)
        {
            using var db = new DataContext();

            BankAccount account = db.BankAccountById(accountId);
            decimal amountAtAccount = db.AmountAt(account.Account);
            decimal amountAvailable = account.AmountAvailable(amountAtAccount);

            if (amountAvailable < amount)
                throw new BackupException("There is no such amount of money available for this account");

            CheckTrust(account.Account, amount);

            var transaction = new TransactionWithdraw
            {
                DateUtcMs = TimeLogic.CurrentTimeMillis(),
                Account = account.Account,

                Amount = amount,
                Commission = account.CommissionWithdraw(amountAtAccount, amount),
            };
            db.WithdrawTransactions.Add(transaction);

            db.SaveChanges();
            return transaction.Id;
        }

        public static decimal AmountAt(int accountId)
        {
            using var db = new DataContext();
            return db.AmountAt(accountId);
        }

        public static BankAccount BankAccountById(this DataContext db, int id)
        {
            BankAccount? account = db.DebitAccounts
                .Include(account => account.Account)
                .Include(account => account.Account.Bank)
                .Include(account => account.Account.Person)
                .FirstOrDefault(acc => acc.Account.Id == id);
            if (account is null)
            {
                account = db.CreditAccounts
                    .Include(account => account.Account)
                    .Include(account => account.Account.Bank)
                    .Include(account => account.Account.Person)
                    .FirstOrDefault(acc => acc.Account.Id == id);
            }

            if (account is null)
            {
                account = db.DepositAccounts
                    .Include(account => account.Account)
                    .Include(account => account.Account.Bank)
                    .Include(account => account.Account.Person)
                    .FirstOrDefault(acc => acc.Account.Id == id);
            }

            return account ?? throw new BankException("There is no bank account with such id");
        }

        private static bool TryDestroyAccount<TBankAccount>(DbSet<TBankAccount> set, int id)
            where TBankAccount : BankAccount
        {
            TBankAccount? bankAccount = set
                .Include(account => account.Account)
                .FirstOrDefault(account => account.Account.Id == id);
            if (bankAccount is null)
                return false;
            set.Remove(bankAccount);
            return true;
        }

        private static Account Create(DataContext db, int bankId, int personId)
        {
            Bank bank = db.BankById(bankId);
            Person person = db.PersonById(personId);

            var account = new Account
            {
                Bank = bank,
                Person = person,
            };
            db.Accounts.Add(account);

            var transaction = new TransactionCreate
            {
                Id = db.NextTransactionId(),
                Account = account,
                DateUtcMs = TimeLogic.CurrentTimeMillis(),
            };
            db.CreateTransactions.Add(transaction);

            db.SaveChanges();
            return account;
        }

        private static void CheckTrust(Account account, decimal amount)
        {
            if (!BankLogic.CanTrust(account.Person) && amount > account.Bank.AnonLimit)
                throw new BackupException("Transferring this amount is restricted for anonymous accounts");
        }
    }
}