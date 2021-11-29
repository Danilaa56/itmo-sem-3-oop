using System;
using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionCorrection : Transaction
    {
        public decimal Correction { get; set; }

        public override void Process(Dictionary<Guid, decimal> accountToMoney)
        {
            accountToMoney[Account.Id] += Correction;
        }

        public override void Reverse(Dictionary<Guid, decimal> accountToMoney)
        {
            accountToMoney[Account.Id] -= Correction;
        }
    }
}