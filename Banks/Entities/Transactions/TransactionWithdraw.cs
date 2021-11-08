using System;
using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionWithdraw : Transaction
    {
        public TransactionWithdraw(Account account, decimal amount, decimal commission)
            : base(account, TransactionType.Withdraw)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));
            Amount = amount;
            Commission = commission;
        }

        public decimal Amount { get; }
        public decimal Commission { get; }

        public override void Process(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] -= Amount + Commission;
        }

        public override void Reverse(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] += Amount + Commission;
        }
    }
}