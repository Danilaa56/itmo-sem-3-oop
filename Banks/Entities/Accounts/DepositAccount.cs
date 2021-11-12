using Banks.Tools;

namespace Banks.Entities.Accounts
{
    public class DepositAccount : BankAccount
    {
        public DepositAccount(Account account, long unlockTimeUtc)
            : base(account)
        {
            UnlockTimeUtc = unlockTimeUtc;
        }

        public decimal PercentForRemains => Bank.DepositPercentForRemains(Amount);
        public long UnlockTimeUtc { get; }

        public override decimal AmountAvailable()
        {
            if (CentralBank.CurrentTimeMillis() < UnlockTimeUtc)
                return 0;
            return Amount;
        }

        public override decimal CommissionTopUp(decimal amount)
        {
            return 0;
        }

        public override decimal CommissionWithdraw(decimal amount)
        {
            if (amount > AmountAvailable())
                throw new BankException($"Such amount of money={amount} is not available for the account now");
            return 0;
        }
    }
}