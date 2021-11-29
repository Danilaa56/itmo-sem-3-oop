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

            Transaction transaction = _context.Transactions.Find(transaction => transaction.Id == transactionId);
            if (transaction is null)
                throw new BackupException("There is no transaction with such id");
            if (transaction.IsCancelled)
                throw new BackupException("Transaction cannot be cancelled twice");
            transaction.IsCancelled = true;
            _context.ReverseTransaction(transaction);
        }

        public List<Transaction> List()
        {
            return _context.Transactions.ToList();
        }
    }
}