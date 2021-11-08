using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;

namespace Banks.Entities
{
    public class CentralBank
    {
        private readonly HashSet<Bank> _banks = new HashSet<Bank>();

        public void RegisterBank(Bank bank)
        {
            if (bank is null)
                throw new ArgumentNullException(nameof(bank));
            if (_banks.Contains(bank))
                throw new ArgumentException("This bank is already registered");

            _banks.Add(bank);
        }

        public void UnregisterBank(Bank bank)
        {
            if (bank is null)
                throw new ArgumentNullException(nameof(bank));
            if (!_banks.Remove(bank))
                throw new ArgumentException("This bank was not registered");
        }

        public ImmutableList<Bank> Banks()
        {
            return _banks.ToImmutableList();
        }

        private readonly List<Transaction> _transactions = new List<Transaction>();
        private readonly List<Account> _accounts = new List<Account>();
        private readonly Dictionary<Account, decimal> _accountToMoney = new Dictionary<Account, decimal>();

        private int _nextTransactionId = 1;
        private int _nextAccountId = 1;

        private long _time = 0;

        public long CurrentTimeMillis()
        {
            return _time;
        }

        public void TimePassed(long time)
        {
            _time += time;
            foreach (Bank bank in _banks)
            {
                bank.Update();
            }
        }

        public ImmutableList<Transaction> Transactions() => _transactions.ToImmutableList();

        public decimal GetAccountAmount(Account account)
        {
            if (!_accountToMoney.TryGetValue(account, out decimal amount))
                throw new ArgumentException("There is no such account");
            return amount;
        }

        public Account CreateAccount(Bank bank, Person person)
        {
            if (!Equals(bank.CentralBank))
                throw new ArgumentException("Bank is not registered in this central bank", nameof(bank));

            var account = new Account(_nextAccountId, bank, person);

            var transaction = new TransactionCreate(account);
            CommitTransaction(transaction);

            _accounts.Add(account);
            _nextAccountId++;

            return account;
        }

        public void TopUp(BankAccount account, decimal amount)
        {
            if (!Equals(account.CentralBank))
                throw new ArgumentException("Account is not registered in this central bank", nameof(account));

            decimal commission = account.CommissionTopUp(amount);
            var transaction = new TransactionTopUp(account.Account, amount, commission);
            CommitTransaction(transaction);
        }

        public void Withdraw(BankAccount account, decimal amount)
        {
            if (!Equals(account.CentralBank))
                throw new ArgumentException("Account is not registered in this central bank", nameof(account));

            decimal commission = account.CommissionWithdraw(amount);
            var transaction = new TransactionWithdraw(account.Account, amount, commission);
            CommitTransaction(transaction);
        }

        public void Transfer(
            BankAccount account,
            decimal amount,
            BankAccount receiverAccount,
            decimal receiverAmount)
        {
            if (!Equals(account.CentralBank))
                throw new ArgumentException("Account is not registered in this central bank", nameof(account));
            if (!Equals(receiverAccount.CentralBank))
            {
                throw new ArgumentException(
                    "Receiver account is not registered in this central bank",
                    nameof(receiverAmount));
            }

            decimal commission = account.CommissionSend(amount);
            decimal receiverCommission = receiverAccount.CommissionReceive(amount);
            var transaction = new TransactionTransfer(
                account.Account,
                amount,
                commission,
                receiverAccount.Account,
                receiverAmount,
                receiverCommission);
            CommitTransaction(transaction);
        }

        public void Destroy(Account account)
        {
            if (!Equals(account))
                throw new ArgumentException("Account is not registered in this central bank", nameof(account));

            var transaction = new TransactionCreate(account);
            CommitTransaction(transaction);

            _accounts.Remove(account);
        }

        public void Correction(Account account, decimal amount)
        {
            if (!Equals(account.Bank.CentralBank))
                throw new ArgumentException("Account is not registered in this central bank", nameof(account));

            var transaction = new TransactionCorrection(account, amount);
            CommitTransaction(transaction);
        }

        public void CancelTransaction(Transaction transaction)
        {
            transaction.Cancel();
            transaction.Reverse(_accountToMoney);
        }

        private void CommitTransaction(Transaction transaction)
        {
            transaction.Account.Bank.BeforeTransaction(transaction);
            transaction.Commit(_nextTransactionId++, CurrentTimeMillis());
            _transactions.Add(transaction);
            transaction.Process(_accountToMoney);
        }
    }
}