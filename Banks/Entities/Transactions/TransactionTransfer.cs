using System;
using System.Collections.Generic;
using Banks.Tools;

namespace Banks.Entities.Transactions
{
    public class TransactionTransfer : Transaction
    {
        public TransactionTransfer(
            Account account,
            decimal amount,
            decimal commission,
            Account receiverAccount,
            decimal receiverAmount,
            decimal receiverCommission)
            : base(account, TransactionType.Transfer)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));
            Amount = amount;
            Commission = commission;
            ReceiverAccount = receiverAccount ?? throw new ArgumentNullException(nameof(receiverAccount));
            if (receiverAccount.Equals(account))
                throw new BankException("Receiver and sender cannot be the same");
            if (receiverAmount <= 0)
                throw new ArgumentException("Receiver amount must be positive", nameof(receiverAmount));
            ReceiverAmount = receiverAmount;
            ReceiverCommission = receiverCommission;
        }

        public decimal Amount { get; }
        public decimal Commission { get; }
        public Account ReceiverAccount { get; }
        public decimal ReceiverAmount { get; }
        public decimal ReceiverCommission { get; }

        public override void Process(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] -= Amount + Commission;
            accountToMoney[ReceiverAccount] += Amount - Commission;
        }

        public override void Reverse(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] += Amount + Commission;
            accountToMoney[ReceiverAccount] -= Amount - Commission;
        }
    }
}