using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Tools;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Tools;

namespace Banks.BLL
{
    public class AccountLogic
    {
        private readonly ApplicationContext _context;

        internal AccountLogic(ApplicationContext context)
        {
            _context = context;
        }

        public Guid CreateDeposit(Guid bankId, Guid personId)
        {
            Account account = Create(bankId, personId);
            var bankAccount = new DepositAccount
            {
                Id = Guid.NewGuid(),
                Account = account,
                UnlockTimeMs = _context.Time.CurrentTimeMillis() + account.Bank.DepositTimeMs,
            };

            _context.DepositAccounts.Add(bankAccount);
            return bankAccount.Account.Id;
        }

        public Guid CreateDebit(Guid bankId, Guid personId)
        {
            Account account = Create(bankId, personId);
            var bankAccount = new DebitAccount()
            {
                Id = Guid.NewGuid(),
                Account = account,
            };

            _context.DebitAccounts.Add(bankAccount);
            return bankAccount.Account.Id;
        }

        public Guid CreateCredit(Guid bankId, Guid personId)
        {
            Account account = Create(bankId, personId);
            var bankAccount = new CreditAccount
            {
                Id = Guid.NewGuid(),
                Account = account,
            };

            _context.CreditAccounts.Add(bankAccount);
            return bankAccount.Account.Id;
        }

        public Guid Correction(Guid accountId, decimal correction)
        {
            BankAccount bankAccount = BankAccountById(accountId);

            var transaction = new TransactionCorrection
            {
                Id = Guid.NewGuid(),
                Account = bankAccount.Account,
                Correction = correction,
                DateUtcMs = _context.Time.CurrentTimeMillis(),
            };

            _context.PushTransaction(transaction);
            return transaction.Id;
        }

        public Guid Destroy(Guid accountId)
        {
            Account account = ById(accountId);

            if (!TryDestroyAccount(_context.CreditAccounts, accountId)
                && !TryDestroyAccount(_context.DebitAccounts, accountId)
                && !TryDestroyAccount(_context.DepositAccounts, accountId))
            {
                throw new BankException("There is no bank account with such id");
            }

            var transaction = new TransactionDestroy()
            {
                Id = Guid.NewGuid(),
                Account = account,
                DateUtcMs = _context.Time.CurrentTimeMillis(),
            };

            _context.PushTransaction(transaction);
            _context.Accounts.Remove(account);
            return transaction.Id;
        }

        public Guid TopUp(Guid accountId, decimal amount)
        {
            BankAccount bankAccount = BankAccountById(accountId);

            var transaction = new TransactionTopUp
            {
                Id = Guid.NewGuid(),
                Account = bankAccount.Account,
                Amount = amount,
                Commission = bankAccount.CommissionTopUp(_context.AmountAtAccount(accountId), amount),
                DateUtcMs = _context.Time.CurrentTimeMillis(),
            };

            _context.PushTransaction(transaction);
            return transaction.Id;
        }

        public Guid Transfer(Guid senderId, Guid receiverId, decimal amount)
        {
            BankAccount senderAccount = BankAccountById(senderId);
            BankAccount receiverAccount = BankAccountById(receiverId);

            decimal amountAtSender = _context.AmountAtAccount(senderId);
            decimal amountAtReceiver = _context.AmountAtAccount(receiverId);

            decimal amountAvailable = senderAccount.AmountAvailable(amountAtSender, _context.Time.CurrentTimeMillis());
            if (amountAvailable < amount)
                throw new BackupException("There is no such amount of money available for this account");

            CheckTrust(senderAccount.Account, amount);

            var transaction = new TransactionTransfer
            {
                Id = Guid.NewGuid(),
                DateUtcMs = _context.Time.CurrentTimeMillis(),
                Account = senderAccount.Account,

                Amount = amount,
                Commission = senderAccount.CommissionWithdraw(amountAtSender, amount),
                ReceiverAmount = amount,
                ReceiverCommission = receiverAccount.CommissionTopUp(amountAtReceiver, amount),
            };

            _context.PushTransaction(transaction);
            return transaction.Id;
        }

        public Guid Withdraw(Guid accountId, decimal amount)
        {
            BankAccount account = BankAccountById(accountId);
            decimal amountAtAccount = _context.AmountAtAccount(account.Account.Id);
            decimal amountAvailable = account.AmountAvailable(amountAtAccount, _context.Time.CurrentTimeMillis());

            if (amountAvailable < amount)
                throw new BackupException("There is no such amount of money available for this account");

            CheckTrust(account.Account, amount);

            var transaction = new TransactionWithdraw
            {
                Id = Guid.NewGuid(),
                DateUtcMs = _context.Time.CurrentTimeMillis(),
                Account = account.Account,

                Amount = amount,
                Commission = account.CommissionWithdraw(amountAtAccount, amount),
            };

            _context.PushTransaction(transaction);
            return transaction.Id;
        }

        public decimal AmountAt(Guid accountId)
        {
            return _context.AmountAtAccount(accountId);
        }

        public BankAccount BankAccountById(Guid id)
        {
            BankAccount account = _context.BankAccounts.FirstOrDefault(account => account.Account.Id == id);
            return account ?? throw new BankException("There is no bank account with such id");
        }

        private Account ById(Guid id)
        {
            return _context.Accounts.FirstOrDefault(account => account.Id == id) ??
                   throw new BankException("There is no account with such id");
        }

        private bool TryDestroyAccount<TBankAccount>(List<TBankAccount> accountList, Guid id)
            where TBankAccount : BankAccount
        {
            TBankAccount bankAccount = accountList.FirstOrDefault(account => account.Account.Id == id);
            if (bankAccount is null)
                return false;
            accountList.Remove(bankAccount);
            return true;
        }

        private Account Create(Guid bankId, Guid personId)
        {
            Bank bank = _context.Bank.ById(bankId);
            Person person = _context.Person.ById(personId);

            var account = new Account
            {
                Id = Guid.NewGuid(),
                Bank = bank,
                Person = person,
            };
            _context.Accounts.Add(account);

            var transaction = new TransactionCreate
            {
                Id = Guid.NewGuid(),
                Account = account,
                DateUtcMs = _context.Time.CurrentTimeMillis(),
            };
            _context.PushTransaction(transaction);
            return account;
        }

        private void CheckTrust(Account account, decimal amount)
        {
            if (!_context.Bank.CanTrust(account.Person) && amount > account.Bank.AnonLimit)
                throw new BackupException("Transferring this amount is restricted for anonymous accounts");
        }
    }
}