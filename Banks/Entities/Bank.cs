using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Entities.Accounts;
using Banks.Entities.Transactions;
using Banks.Tools;

namespace Banks.Entities
{
    public class Bank
    {
        public const long Day = 1000 * 3600 * 24;
        public const long Month = Day * 30;

        private List<BankAccount> _accounts = new List<BankAccount>();
        private HashSet<Person> _subscribers = new HashSet<Person>();

        private decimal _debitPercentForRemains;
        private decimal _creditLimit;
        private decimal _creditCommission;
        private decimal _minDepositPercentForRemains;
        private Dictionary<decimal, decimal> _depositPercentForRemainsByAmountLevels;
        private long _depositTime;
        private decimal _anonLimit;

        public Bank(
            CentralBank centralBank,
            string name,
            decimal debitPercentForRemains,
            decimal creditLimit,
            decimal creditCommission,
            decimal minDepositPercentForRemains,
            Dictionary<decimal, decimal> depositPercentForRemainsByAmountLevels,
            long depositTime,
            decimal anonLimit)
        {
            CentralBank = centralBank ?? throw new ArgumentNullException(nameof(centralBank));
            Name = name ?? throw new ArgumentNullException(nameof(name));

            SetDebitPercentForRemains(debitPercentForRemains);
            SetCreditLimit(creditLimit);
            SetCreditCommission(creditCommission);
            SetDepositPercentForRemains(minDepositPercentForRemains, depositPercentForRemainsByAmountLevels);
            SetDepositTime(depositTime);
            SetAnonLimit(anonLimit);

            centralBank.RegisterBank(this);
        }

        public CentralBank CentralBank { get; }
        public string Name { get; }

        public decimal DebitPercentForRemains => _debitPercentForRemains;
        public decimal CreditLimit => _creditLimit;
        public decimal CreditCommission => _creditCommission;

        public decimal DepositPercentForRemains(decimal amount)
        {
            decimal percent = _minDepositPercentForRemains;
            foreach (KeyValuePair<decimal, decimal> level in _depositPercentForRemainsByAmountLevels)
            {
                if (amount >= level.Key)
                {
                    percent = level.Value;
                }
                else
                {
                    break;
                }
            }

            return DebitPercentForRemains;
        }

        public long DepositTime => _depositTime;
        public decimal AnonLimit => _anonLimit;

        public DebitAccount CreateDebitAccount(Person person)
        {
            Account account = CentralBank.CreateAccount(this, person);
            var bankAccount = new DebitAccount(account);
            _accounts.Add(bankAccount);
            return bankAccount;
        }

        public CreditAccount CreateCreditAccount(Person person)
        {
            Account account = CentralBank.CreateAccount(this, person);
            var bankAccount = new CreditAccount(account);
            _accounts.Add(bankAccount);
            return bankAccount;
        }

        public DepositAccount CreateDepositAccount(Person person)
        {
            Account account = CentralBank.CreateAccount(this, person);
            long time = CentralBank.CurrentTimeMillis();
            var bankAccount = new DepositAccount(account, time + _depositTime);
            _accounts.Add(bankAccount);
            return bankAccount;
        }

        public void CloseAccount(BankAccount account)
        {
            CentralBank.Destroy(account.Account);
            _accounts.Remove(account);
        }

        public void BeforeTransaction(Transaction transaction)
        {
            Person person = null;
            decimal amount = 0;
            if (transaction is TransactionWithdraw withdraw)
            {
                person = withdraw.Account.Person;
                amount = withdraw.Amount;
            }
            else if (transaction is TransactionTransfer transfer)
            {
                person = transfer.Account.Person;
                amount = transfer.Amount;
            }

            if (person != null && amount > _anonLimit && !CanTrust(person))
            {
                throw new BankException(
                    "Person without address or passport id are not trusted to withdraw such amount of money");
            }
        }

        public void Update()
        {
            long time = CentralBank.CurrentTimeMillis();
            Dictionary<BankAccount, List<Transaction>> accountToTransactions = GetAccountToTransactions();

            foreach (KeyValuePair<BankAccount, List<Transaction>> accountToTransaction in accountToTransactions)
            {
                BankAccount account = accountToTransaction.Key;
                if (account is CreditAccount)
                {
                    continue;
                }

                List<Transaction> transactions = accountToTransaction.Value;

                var fakeTransaction = new TransactionCorrection(account.Account, 0);
                fakeTransaction.Commit(int.MaxValue, time);
                transactions.Add(fakeTransaction);

                long dayCircleTime = transactions.First().DateUtc;
                int daysCount = 0;

                decimal correction = 0;
                var accountToAmount = new Dictionary<Account, decimal>();

                foreach (Transaction transaction in transactions)
                {
                    while (transaction.DateUtc - dayCircleTime > Day)
                    {
                        if (account is DebitAccount debitAccount)
                        {
                            correction += accountToAmount[account.Account] * debitAccount.PercentForRemains / 36500M;
                        }
                        else if (account is DepositAccount depositAccount)
                        {
                            correction += accountToAmount[account.Account] * depositAccount.PercentForRemains / 36500M;
                        }

                        dayCircleTime += Day;
                        if (++daysCount >= 30)
                        {
                            daysCount -= 30;
                            accountToAmount[account.Account] += correction;
                            correction = 0;
                        }
                    }

                    if (transaction is not TransactionCorrection transactionCorrection)
                    {
                        transaction.Process(accountToAmount);
                    }
                }

                if (accountToAmount.TryGetValue(account.Account, out decimal amount))
                {
                    if (account.Amount != amount)
                    {
                        CentralBank.Correction(account.Account, amount - account.Amount);
                    }
                }
            }
        }

        public void SetDebitPercentForRemains(decimal value)
        {
            SubscribersWithDebit().ForEach(subscriber =>
            {
                subscriber.SendMessage($"Debit percent for remains has changed: {_debitPercentForRemains} -> {value}");
            });
            _debitPercentForRemains = value;
        }

        public void SetCreditLimit(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("Credit limit must be positive");
            SubscribersWithCredit().ForEach(subscriber =>
            {
                subscriber.SendMessage($"Credit limit has changed: {_creditLimit} -> {value}");
            });
            _creditLimit = value;
        }

        public void SetCreditCommission(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Credit commission cannot be negative");
            SubscribersWithCredit().ForEach(subscriber =>
            {
                subscriber.SendMessage($"Credit commission has changed: {_creditCommission} -> {value}");
            });
            _creditCommission = value;
        }

        public void SetDepositPercentForRemains(decimal min, Dictionary<decimal, decimal> levels)
        {
            if (levels is null)
                throw new ArgumentNullException(nameof(levels));
            SubscribersWithDeposit().ForEach(subscriber =>
            {
                string line = $"Debit percent for remains has changed: min {_minDepositPercentForRemains} -> {min}%";
                foreach (KeyValuePair<decimal, decimal> keyValuePair in levels)
                {
                    line += $"\n    from {keyValuePair.Key}: {keyValuePair.Value}%";
                }

                subscriber.SendMessage(line);
            });
            _minDepositPercentForRemains = min;
            _depositPercentForRemainsByAmountLevels = levels;
        }

        public void SetDepositTime(long value)
        {
            if (value < 0)
                throw new ArgumentException("Deposit time cannot be negative");
            SubscribersWithDeposit().ForEach(subscriber =>
            {
                subscriber.SendMessage($"Deposit time has changed: {_depositTime} -> {value}");
            });
            _depositTime = value;
        }

        public void SetAnonLimit(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Credit commission cannot be negative");
            SubscribersFromList(_accounts.FindAll(bankAccount => !CanTrust(bankAccount.Account.Person)))
                .ForEach(subscriber =>
                {
                    subscriber.SendMessage($"Anon limit has changed: {_anonLimit} -> {value}");
                });
            _anonLimit = value;
        }

        public void SubscribeForUpdates(Person person)
        {
            if (_subscribers.Contains(person))
                throw new BankException("This news receiver has already subscribed");
            _subscribers.Add(person);
        }

        public void UnsubscribeFromUpdates(Person person)
        {
            if (!_subscribers.Remove(person))
                throw new BankException("This news receiver has not subscribed");
        }

        private bool CanTrust(Person person)
        {
            if (person is null)
                throw new ArgumentNullException(nameof(person));
            return person.Address is not null && person.PassportId is not null;
        }

        private Dictionary<BankAccount, List<Transaction>> GetAccountToTransactions()
        {
            var accountToTransactions = new Dictionary<Account, List<Transaction>>();
            _accounts.ForEach(bankAccount => accountToTransactions[bankAccount.Account] = new List<Transaction>());

            CentralBank.Transactions().ForEach(transaction =>
            {
                if (transaction.IsCancelled)
                    return;
                if (Equals(transaction.Account.Bank))
                    accountToTransactions[transaction.Account].Add(transaction);
                if (transaction is TransactionTransfer transfer && Equals(transfer.ReceiverAccount.Bank))
                    accountToTransactions[transfer.Account].Add(transaction);
            });

            var bankAccountToTransactions = new Dictionary<BankAccount, List<Transaction>>();
            _accounts.ForEach(bankAccount =>
                bankAccountToTransactions[bankAccount] = accountToTransactions[bankAccount.Account]);

            return bankAccountToTransactions;
        }

        private List<Person> SubscribersWithDebit()
        {
            return SubscribersFromList(_accounts.FindAll(bankAccount => bankAccount is DebitAccount));
        }

        private List<Person> SubscribersWithCredit()
        {
            return SubscribersFromList(_accounts.FindAll(bankAccount => bankAccount is CreditAccount));
        }

        private List<Person> SubscribersWithDeposit()
        {
            return SubscribersFromList(_accounts.FindAll(bankAccount => bankAccount is DepositAccount));
        }

        private List<Person> SubscribersFromList(List<BankAccount> list)
        {
            return list
                .Select(bankAccount => bankAccount.Account.Person).ToList()
                .FindAll(person => _subscribers.Contains(person))
                .ToHashSet().ToList();
        }
    }
}