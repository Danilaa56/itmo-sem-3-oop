namespace Banks.Entities.Transactions
{
    public enum TransactionType
    {
        Create,
        Destroy,
        TopUp,
        Transfer,
        Withdraw,
        Correction,
    }

    public static class TransactionTypeExtension
    {
        public static bool IsCancellable(this TransactionType type)
        {
            if (type == TransactionType.Create)
                return false;
            if (type == TransactionType.Destroy)
                return false;
            return true;
        }
    }
}