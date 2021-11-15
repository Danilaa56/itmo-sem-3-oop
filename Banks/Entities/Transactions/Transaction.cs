using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public abstract class Transaction
    {
        public int Id { get; set; }
        public long DateUtcMs { get; set; }
        public Account Account { get; set; }
        public bool IsCancelled { get; set; }

        public abstract void Process(Dictionary<Account, decimal> accountToMoney);
        public virtual void Reverse(Dictionary<Account, decimal> accountToMoney) { }
    }
}