namespace Banks.Entities.Accounts
{
    public abstract class BankAccount : Account
    {
        protected BankAccount(int id, Bank bank, Person person)
        {
            Id = id;
            Bank = bank;
            Person = person;
        }

        // // public decimal Amount => Account.Amount();
        // // public Bank Bank => Account.Bank;
        // // public CentralBank CentralBank => Account.Bank.CentralBank;
        //
        // public abstract decimal AmountAvailable();
        // public abstract decimal CommissionTopUp(decimal amount);
        // public abstract decimal CommissionWithdraw(decimal amount);
        //
        // public virtual decimal CommissionSend(decimal amount)
        // {
        //     return CommissionWithdraw(amount);
        // }
        //
        public virtual decimal CommissionReceive(decimal amount)
        {
            return CommissionTopUp(amount);
        }
        
        public void TopUp(decimal amount)
        {
            CentralBank.TopUp(this, amount);
        }
        
        public void Withdraw(decimal amount)
        {
            CentralBank.Withdraw(this, amount);
        }
        
        public void Transfer(BankAccount destination, decimal amount)
        {
            CentralBank.Transfer(this, amount, destination, amount);
        }
    }
}