#nullable enable
using System.Linq;
using Backups.Tools;
using Banks.Entities.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Banks.BLL
{
    public static class TransactionLogic
    {
        public static void Cancel(int transactionId)
        {
            using var db = new DataContext();

            if (db.CreateTransactions.FirstOrDefault(transaction => transaction.Id == transactionId) is not null)
            {
                throw new BackupException("'Create Account' transaction cannot be cancelled");
            }

            if (!TryCancel(db.CorrectionTransactions, transactionId)
                && !TryCancel(db.DestroyTransactions, transactionId)
                && !TryCancel(db.TransferTransactions, transactionId)
                && !TryCancel(db.TopUpTransactions, transactionId)
                && !TryCancel(db.WithdrawTransactions, transactionId))
                throw new BackupException("There is no transaction with such id");

            db.SaveChanges();
        }

        private static bool TryCancel<TTransaction>(DbSet<TTransaction> set, int id)
            where TTransaction : Transaction
        {
            TTransaction? transaction = set.FirstOrDefault(transaction => transaction.Id == id);
            if (transaction is null)
                return false;
            if (transaction.IsCancelled)
                throw new BackupException("Transaction cannot be cancelled twice");
            transaction.IsCancelled = true;
            return true;
        }
    }
}