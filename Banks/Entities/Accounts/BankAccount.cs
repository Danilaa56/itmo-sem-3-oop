namespace Banks.Entities.Accounts
{
    public abstract class BankAccount
    {
        public int Id { get; set; }
        public Account Account { get; set; }

        public abstract decimal CommissionTopUp(decimal amountNow, decimal amountTransferring);
        public abstract decimal CommissionWithdraw(decimal amountNow, decimal amountTransferring);
        public abstract decimal AmountAvailable(decimal amountNow);
    }
}