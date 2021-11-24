using System;
using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionDestroy : Transaction
    {
        public override void Process(Dictionary<Guid, decimal> accountToMoney)
        {
            accountToMoney.Remove(Account.Id);
        }
    }
}