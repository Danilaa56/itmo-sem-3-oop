using Banks.BLL;

namespace Banks.Entities.Accounts
{
    public class DepositAccount : BankAccount
    {
        public long UnlockTimeMs { get; set; }

        public override decimal CommissionTopUp(decimal amountNow, decimal amountTransferring)
        {
            return 0;
        }

        public override decimal CommissionWithdraw(decimal amountNow, decimal amountTransferring)
        {
            return 0;
        }

        public override decimal AmountAvailable(decimal amountNow)
        {
            if (TimeLogic.CurrentTimeMillis() < UnlockTimeMs)
                return 0;
            return amountNow;
        }
    }
}