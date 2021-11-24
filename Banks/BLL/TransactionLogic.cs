using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Tools;
using Banks.Entities.Transactions;

namespace Banks.BLL
{
    public class TransactionLogic
    {
        private readonly ApplicationContext _context;

        internal TransactionLogic(ApplicationContext context)
        {
            _context = context;
        }

        public void Cancel(Guid transactionId)
        {
            if (_context.CreateTransactions.FirstOrDefault(transaction => transaction.Id == transactionId) is not null)
            {
                throw new BackupException("'Create Account' transaction cannot be cancelled");
            }

            if (!TryCancel(_context.CorrectionTransactions, transactionId)
                && !TryCancel(_context.DestroyTransactions, transactionId)
                && !TryCancel(_context.TransferTransactions, transactionId)
                && !TryCancel(_context.TopUpTransactions, transactionId)
                && !TryCancel(_context.WithdrawTransactions, transactionId))
                throw new BackupException("There is no transaction with such id");
        }

        public List<Transaction> List()
        {
            return _context.Transactions.ToList();
        }

        private bool TryCancel<TTransaction>(List<TTransaction> list, Guid id)
            where TTransaction : Transaction
        {
            TTransaction transaction = list.FirstOrDefault(transaction => transaction.Id == id);
            if (transaction is null)
                return false;
            if (transaction.IsCancelled)
                throw new BackupException("Transaction cannot be cancelled twice");
            transaction.IsCancelled = true;
            _context.ReverseTransaction(transaction);
            return true;
        }
    }
}