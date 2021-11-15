using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;

namespace Banks.BLL
{
    public static class TimeLogic
    {
        public static long Day => 24 * 3600 * 1000;
        public static long Month => Day * 30;

        public static long CurrentTimeMillis()
        {
            using var db = new DataContext();
            return db.Environment().TimeMs;
        }

        public static void RotateTime(long timeMs)
        {
            List<Bank> banks;
            using (var db = new DataContext())
            {
                var amountsByAccounts = db.AmountsByAccounts();

                long currentTime = db.Environment().TimeMs += timeMs;
                banks = db.Banks.ToList();

                foreach (Bank bank in banks)
                {
                    // GetAccountToTransactions()
                    Update(
                        amountsByAccounts,
                        db,
                        bank,
                        currentTime);
                }

                db.SaveChanges();
            }
        }

        private static void Update(
            Dictionary<Account, decimal> currentAmounts,
            DataContext db,
            Bank bank,
            long currentTimeMs)
        {
            Dictionary<BankAccount, List<Transaction>> accountToTransactions = GetAccountToTransactions(db, bank);

            foreach (KeyValuePair<BankAccount, List<Transaction>> accountToTransaction in accountToTransactions)
            {
                BankAccount account = accountToTransaction.Key;
                List<Transaction> transactions = accountToTransaction.Value;

                if (account is CreditAccount)
                {
                    continue;
                }

                var fakeTransaction = new TransactionCorrection
                {
                    Id = int.MaxValue,
                    Account = account.Account,
                    Correction = 0,
                    DateUtcMs = currentTimeMs,
                    IsCancelled = false,
                };
                transactions.Add(fakeTransaction);

                long dayCircleTime = transactions.First().DateUtcMs;
                int daysCount = 0;

                decimal correction = 0;
                var accountToAmount = new Dictionary<Account, decimal>();

                foreach (Transaction transaction in transactions)
                {
                    while (transaction.DateUtcMs - dayCircleTime > Day)
                    {
                        if (account is DebitAccount)
                        {
                            correction += accountToAmount[account.Account] * bank.DebitPercentForRemains / 36500M;
                        }
                        else if (account is DepositAccount)
                        {
                            correction += accountToAmount[account.Account] *
                                bank.DepositPercentForRemains(accountToAmount[account.Account]) / 36500M;
                        }

                        dayCircleTime += Day;
                        if (++daysCount >= 30)
                        {
                            daysCount -= 30;
                            accountToAmount[account.Account] += correction;
                            correction = 0;
                        }
                    }

                    if (transaction is not TransactionCorrection)
                    {
                        transaction.Process(accountToAmount);
                    }
                }

                if (accountToAmount.TryGetValue(account.Account, out decimal amount))
                {
                    decimal currentAmount = currentAmounts[account.Account];
                    if (currentAmount != amount)
                    {
                        AccountLogic.Correction(db, account.Account.Id, amount - currentAmount);
                    }
                }
            }
        }

        private static Dictionary<BankAccount, List<Transaction>> GetAccountToTransactions(DataContext db, Bank bank)
        {
            var accountToTransactions = new Dictionary<Account, List<Transaction>>();
            var bankAccounts =
                db.BankAccounts.Where(bankAccount => bank.Equals(bankAccount.Account.Bank)).ToList();
            bankAccounts.ForEach(bankAccount => accountToTransactions[bankAccount.Account] = new List<Transaction>());

            db.Transactions.ForEach(transaction =>
            {
                if (transaction.IsCancelled)
                    return;
                if (bank.Equals(transaction.Account.Bank))
                    accountToTransactions[transaction.Account].Add(transaction);
                if (transaction is TransactionTransfer transfer && bank.Equals(transfer.ReceiverAccount.Bank))
                    accountToTransactions[transfer.Account].Add(transaction);
            });

            var bankAccountToTransactions = new Dictionary<BankAccount, List<Transaction>>();
            bankAccounts.ForEach(bankAccount =>
                bankAccountToTransactions[bankAccount] = accountToTransactions[bankAccount.Account]);

            return bankAccountToTransactions;
        }
    }
}