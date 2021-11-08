using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionCorrection : Transaction
    {
        public TransactionCorrection(Account account, decimal correction)
            : base(account, TransactionType.Correction)
        {
            Correction = correction;
        }

        public decimal Correction { get; }

        public override void Process(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] += Correction;
        }

        public override void Reverse(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] -= Correction;
        }
    }
}