using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public class TransactionDestroy : Transaction
    {
        public TransactionDestroy(Account account)
            : base(account, TransactionType.Destroy)
        {
        }

        public override void Process(Dictionary<Account, decimal> accountToMoney)
        {
            accountToMoney.Remove(Account);
        }
    }
}