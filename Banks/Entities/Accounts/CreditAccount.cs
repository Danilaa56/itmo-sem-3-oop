namespace Banks.Entities.Accounts
{
    public class CreditAccount : BankAccount
    {
        public override decimal CommissionTopUp(decimal amountNow, decimal amountTransferring)
        {
            return amountNow < 0 ? Account.Bank.CreditCommission : 0;
        }

        public override decimal CommissionWithdraw(decimal amountNow, decimal amountTransferring)
        {
            return amountNow < 0 ? Account.Bank.CreditCommission : 0;
        }

        public override decimal AmountAvailable(decimal amountNow)
        {
            return amountNow + Account.Bank.CreditLimit - CommissionWithdraw(amountNow, 0);
        }
    }
}