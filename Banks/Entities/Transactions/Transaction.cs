using System;
using System.Collections.Generic;
using Banks.Tools;

namespace Banks.Entities.Transactions
{
    public abstract class Transaction
    {
        private int _transactionId = -1; // -1 means not committed
        private long _dateUtc;
        private bool _isCancelled;

        public Transaction(Account account, TransactionType type, bool isCancelled = false)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            TransactionType = type;
            _isCancelled = isCancelled;
        }

        public abstract void Process(Dictionary<Account, decimal> accountToMoney);

        public virtual void Reverse(Dictionary<Account, decimal> accountToMoney)
        {
        }

        public int TransactionId => _transactionId;
        public long DateUtc => _dateUtc;
        public Account Account { get; }
        public TransactionType TransactionType { get; }
        public bool IsCancelled => _isCancelled;

        public void Commit(int transactionId, long dateUtc)
        {
            if (_transactionId != -1)
                throw new BankException("Transaction has already been committed");
            if (_transactionId != -1)
                throw new BankException("Transaction has already been committed");
            if (transactionId < 1)
                throw new ArgumentException("Transaction id cannot be non-positive", nameof(transactionId));
            _transactionId = transactionId;
            _dateUtc = dateUtc;
        }

        public void Cancel()
        {
            if (_isCancelled)
                throw new BankException("Transaction cannot be cancelled twice");
            if (!TransactionType.IsCancellable())
                throw new BankException("This transaction cannot be cancelled");
            _isCancelled = true;
        }
    }
}