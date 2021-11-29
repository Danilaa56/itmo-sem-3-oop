using System;
using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionCreate : Transaction
    {
        public override void Process(Dictionary<Guid, decimal> accountToMoney)
        {
            accountToMoney[Account.Id] = 0;
        }
    }
}