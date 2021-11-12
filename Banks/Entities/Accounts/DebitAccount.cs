using Banks.Tools;

namespace Banks.Entities.Accounts
{
    public class DebitAccount : BankAccount
    {
        public DebitAccount(Account account)
            : base(account)
        {
        }

        public decimal PercentForRemains => Bank.DebitPercentForRemains;

        public override decimal AmountAvailable()
        {
            return Amount;
        }

        public override decimal CommissionTopUp(decimal amount)
        {
            return 0;
        }

        public override decimal CommissionWithdraw(decimal amount)
        {
            if (amount > AmountAvailable())
                throw new BankException($"Such amount of money={amount} is not available for the account");
            return 0;
        }
    }
}