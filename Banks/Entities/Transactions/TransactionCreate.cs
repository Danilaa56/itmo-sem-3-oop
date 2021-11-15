using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionCreate : Transaction
    {
        public override void Process(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] = 0;
        }
    }
}