using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionCreate : Transaction
    {
        public TransactionCreate(Account account)
            : base(account, TransactionType.Create)
        {
        }

        public override void Process(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney[Account] = 0;
        }
    }
}