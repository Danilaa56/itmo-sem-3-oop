using System;
using System.Collections.Generic;

namespace Banks.Entities.Transactions
{
    public abstract class Transaction
    {
        public Guid Id { get; set; }
        public long DateUtcMs { get; set; }
        public Account Account { get; set; }
        public bool IsCancelled { get; set; }

        public abstract void Process(Dictionary<Guid, decimal> accountToMoney);
        public virtual void Reverse(Dictionary<Guid, decimal> accountToMoney) { }
    }
}