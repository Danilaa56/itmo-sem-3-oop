namespace Banks.Entities.Accounts
{
    public class DebitAccount : BankAccount
    {
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
            return amountNow;
        }
    }
}