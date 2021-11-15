using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionCorrection : Transaction
    {
        public decimal Correction { get; set; }

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