using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Banks.Entities;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Tools;
using Environment = Banks.Entities.Environment;

namespace Banks.BLL
{
    public class ApplicationContext
    {
        private readonly string _fileName;

        private readonly Dictionary<Guid, decimal> _amountByAccountId = new Dictionary<Guid, decimal>();

        public ApplicationContext(string fileName)
        {
            _fileName = fileName;

            Person = new PersonLogic(this);
            Bank = new BankLogic(this);
            Account = new AccountLogic(this);
            Time = new TimeLogic(this);
            Transaction = new TransactionLogic(this);

            Load();
        }

        public PersonLogic Person { get; }
        public BankLogic Bank { get; }
        public AccountLogic Account { get; }
        public TimeLogic Time { get; }
        public TransactionLogic Transaction { get; }

        public Environment Environment { get; set; }

        internal List<Person> Persons { get; set; }
        internal List<Bank> Banks { get; set; }
        internal List<Account> Accounts { get; set; }
        internal List<DebitAccount> DebitAccounts { get; set; }
        internal List<CreditAccount> CreditAccounts { get; set; }
        internal List<DepositAccount> DepositAccounts { get; set; }

        internal List<BankAccount> BankAccounts
        {
            get
            {
                var list = new List<BankAccount>();
                list.AddRange(DebitAccounts);
                list.AddRange(CreditAccounts);
                list.AddRange(DepositAccounts);
                return list;
            }
        }

        internal List<Transaction> Transactions
        {
            get
            {
                var list = new List<Transaction>();
                list.AddRange(CorrectionTransactions);
                list.AddRange(CreateTransactions);
                list.AddRange(DestroyTransactions);
                list.AddRange(TopUpTransactions);
                list.AddRange(TransferTransactions);
                list.AddRange(WithdrawTransactions);
                list.Sort((ta1, ta2) => (ta1.DateUtcMs - ta2.DateUtcMs) > 1 ? 1 : 0);
                return list;
            }
        }

        internal List<TransactionCorrection> CorrectionTransactions { get; set; }
        internal List<TransactionCreate> CreateTransactions { get; set; }
        internal List<TransactionDestroy> DestroyTransactions { get; set; }
        internal List<TransactionTopUp> TopUpTransactions { get; set; }
        internal List<TransactionTransfer> TransferTransactions { get; set; }
        internal List<TransactionWithdraw> WithdrawTransactions { get; set; }

        public void Save()
        {
            using (var db = new DataContext(_fileName))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                db.Persons.AddRange(Persons);
                db.Banks.AddRange(Banks);
                db.Accounts.AddRange(Accounts);
                db.DebitAccounts.AddRange(DebitAccounts);
                db.CreditAccounts.AddRange(CreditAccounts);
                db.DepositAccounts.AddRange(DepositAccounts);
                db.CorrectionTransactions.AddRange(CorrectionTransactions);
                db.CreateTransactions.AddRange(CreateTransactions);
                db.DestroyTransactions.AddRange(DestroyTransactions);
                db.TopUpTransactions.AddRange(TopUpTransactions);
                db.TransferTransactions.AddRange(TransferTransactions);
                db.WithdrawTransactions.AddRange(WithdrawTransactions);
                db.Environments.Add(Environment);

                db.SaveChanges();
            }
        }

        public void Load()
        {
            using (var db = new DataContext(_fileName))
            {
                Persons = db.Persons.Any() ? db.Persons.ToList() : new List<Person>();
                Banks = db.Banks.ToList();
                Accounts = db.Accounts.ToList();
                DebitAccounts = db.DebitAccounts.ToList();
                CreditAccounts = db.CreditAccounts.ToList();
                DepositAccounts = db.DepositAccounts.ToList();

                CorrectionTransactions = db.CorrectionTransactions.ToList();
                CreateTransactions = db.CreateTransactions.ToList();
                DestroyTransactions = db.DestroyTransactions.ToList();
                TopUpTransactions = db.TopUpTransactions.ToList();
                TransferTransactions = db.TransferTransactions.ToList();
                WithdrawTransactions = db.WithdrawTransactions.ToList();

                Environment = db.Environments.FirstOrDefault() ?? new Environment();

                _amountByAccountId.Clear();
                Transactions.ForEach(transaction =>
                {
                    ProcessTransaction(transaction);
                });
            }
        }

        public void Reset()
        {
            using (var db = new DataContext(_fileName))
            {
                db.Database.EnsureDeleted();
            }

            Load();
        }

        public void Default()
        {
            Reset();

            Bank.RegisterBank(
                "GoodBank",
                2,
                1000,
                10,
                3,
                new Dictionary<decimal, decimal>(),
                24 * 3600 * 1000,
                100);
            Person.Create("Ivan", "Ivanov", null, null);
        }

        public ImmutableDictionary<Guid, decimal> AmountByAccountId()
        {
            return _amountByAccountId.ToImmutableDictionary();
        }

        public decimal AmountAtAccount(Guid accountId)
        {
            if (!_amountByAccountId.TryGetValue(accountId, out decimal amount))
            {
                throw new BankException("There is no account with such id");
            }

            return amount;
        }

        internal void PushTransaction(TransactionCorrection transaction)
        {
            CorrectionTransactions.Add(transaction);
            ProcessTransaction(transaction);
        }

        internal void PushTransaction(TransactionCreate transaction)
        {
            CreateTransactions.Add(transaction);
            ProcessTransaction(transaction);
        }

        internal void PushTransaction(TransactionDestroy transaction)
        {
            DestroyTransactions.Add(transaction);
            ProcessTransaction(transaction);
        }

        internal void PushTransaction(TransactionTransfer transaction)
        {
            TransferTransactions.Add(transaction);
            ProcessTransaction(transaction);
        }

        internal void PushTransaction(TransactionTopUp transaction)
        {
            TopUpTransactions.Add(transaction);
            ProcessTransaction(transaction);
        }

        internal void PushTransaction(TransactionWithdraw transaction)
        {
            WithdrawTransactions.Add(transaction);
            ProcessTransaction(transaction);
        }

        internal void ReverseTransaction(Transaction transaction)
        {
            transaction.Reverse(_amountByAccountId);
        }

        private void ProcessTransaction(Transaction transaction)
        {
            Environment.TimeMs += 1;
            if (!transaction.IsCancelled)
                transaction.Process(_amountByAccountId);
        }
    }
}