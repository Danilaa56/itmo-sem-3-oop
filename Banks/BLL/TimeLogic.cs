using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;

namespace Banks.BLL
{
    public class TimeLogic
    {
        private readonly ApplicationContext _context;

        internal TimeLogic(ApplicationContext context)
        {
            _context = context;
        }

        public static long Day => 24 * 3600 * 1000;
        public static long Month => Day * 30;

        public long CurrentTimeMillis()
        {
            return _context.Environment.TimeMs;
        }

        public void Rotate(long timeMs)
        {
            var amountByAccountId = _context.AmountByAccountId();

            long currentTime = _context.Environment.TimeMs += timeMs;
            List<Bank> banks = _context.Banks;

            foreach (Bank bank in banks)
            {
                // GetAccountToTransactions()
                Update(
                    amountByAccountId,
                    bank,
                    currentTime);
            }
        }

        private void Update(
            ImmutableDictionary<Guid, decimal> currentAmounts,
            Bank bank,
            long currentTimeMs)
        {
            Dictionary<BankAccount, List<Transaction>> accountToTransactions = GetAccountToTransactions(bank);

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
                    Id = Guid.NewGuid(),
                    Account = account.Account,
                    Correction = 0,
                    DateUtcMs = currentTimeMs + 1,
                    IsCancelled = false,
                };
                transactions.Add(fakeTransaction);

                long dayCircleTime = transactions.First().DateUtcMs;
                int daysCount = 0;

                decimal correction = 0;
                var accountToAmount = new Dictionary<Guid, decimal>();

                foreach (Transaction transaction in transactions)
                {
                    while (transaction.DateUtcMs - dayCircleTime > Day)
                    {
                        if (account is DebitAccount)
                        {
                            correction += accountToAmount[account.Account.Id] * bank.DebitPercentForRemains / 36500M;
                        }
                        else if (account is DepositAccount)
                        {
                            correction += accountToAmount[account.Account.Id] *
                                bank.DepositPercentForRemains(accountToAmount[account.Account.Id]) / 36500M;
                        }

                        dayCircleTime += Day;
                        if (++daysCount >= 30)
                        {
                            daysCount -= 30;
                            accountToAmount[account.Account.Id] += correction;
                            correction = 0;
                        }
                    }

                    if (transaction is not TransactionCorrection)
                    {
                        transaction.Process(accountToAmount);
                    }
                }

                if (accountToAmount.TryGetValue(account.Account.Id, out decimal amount))
                {
                    decimal currentAmount = currentAmounts[account.Account.Id];
                    if (currentAmount != amount)
                    {
                        _context.Account.Correction(account.Account.Id, amount - currentAmount);
                    }
                }
            }
        }

        private Dictionary<BankAccount, List<Transaction>> GetAccountToTransactions(Bank bank)
        {
            var accountToTransactions = new Dictionary<Account, List<Transaction>>();
            var bankAccounts =
                _context.BankAccounts.FindAll(bankAccount => bank.Equals(bankAccount.Account.Bank));
            bankAccounts.ForEach(bankAccount => accountToTransactions[bankAccount.Account] = new List<Transaction>());

            _context.Transactions.ForEach(transaction =>
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