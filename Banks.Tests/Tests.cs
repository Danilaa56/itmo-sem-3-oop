using System;
using System.Collections.Generic;
using Banks.BLL;
using Banks.Entities;
using Banks.Entities.Accounts;
using NUnit.Framework;

namespace Banks.Tests
{
    [TestFixture]
    public class Tests
    {
        private ApplicationContext _context;
        private Guid _exampleBankId;
        private Guid _examplePersonId;

        [SetUp]
        public void Setup()
        {
            _context = new ApplicationContext("banks.db");
            _context.Reset();
            _exampleBankId = _context.Bank.RegisterBank(
                "Sample bank",
                3.65M,
                1000,
                10,
                10,
                new Dictionary<decimal, decimal>(),
                TimeLogic.Month,
                100);
            _examplePersonId = _context.Person.Create("Ivan", "Ivanov", "ul. Pushkina", "multi");
        }

        [Test]
        public void DebitAccountBelowZero()
        {
            Guid accountId = _context.Account.CreateDebit(_exampleBankId, _examplePersonId);
            _context.Account.TopUp(accountId, 100);

            Assert.Catch(() => _context.Account.Withdraw(accountId, 150));
        }

        [Test]
        public void CreateDebitAccountAndTopItUp()
        {
            Guid accountId = _context.Account.CreateDebit(_exampleBankId, _examplePersonId);
            _context.Account.TopUp(accountId, 100);

            Assert.AreEqual(100, _context.Account.AmountAt(accountId));
            Assert.AreEqual(2, _context.Transaction.List().Count);
        }

        [Test]
        public void TestCreditAccount()
        {
            Bank bank = _context.Bank.ById(_exampleBankId);
            Guid accountId = _context.Account.CreateCredit(_exampleBankId, _examplePersonId);
            BankAccount account = _context.Account.BankAccountById(accountId);

            Assert.AreEqual(bank.CreditLimit,
                account.AmountAvailable(0, _context.Time.CurrentTimeMillis()));

            decimal withdrawAmount = 100;
            _context.Account.Withdraw(accountId, withdrawAmount);

            decimal amountAtAccount = _context.AmountAtAccount(accountId);
            Assert.AreEqual(-withdrawAmount, amountAtAccount);
            Assert.AreEqual(bank.CreditLimit - withdrawAmount - bank.CreditCommission,
                account.AmountAvailable(amountAtAccount, _context.Time.CurrentTimeMillis()));

            decimal topUpAmount = 50;
            _context.Account.TopUp(accountId, topUpAmount);

            amountAtAccount = _context.AmountAtAccount(accountId);
            Assert.AreEqual(-withdrawAmount - bank.CreditCommission + topUpAmount, amountAtAccount);
            Assert.AreEqual(bank.CreditLimit - withdrawAmount + topUpAmount - bank.CreditCommission * 2,
                account.AmountAvailable(amountAtAccount, _context.Time.CurrentTimeMillis()));
        }

        [Test]
        public void AnonLimitTest()
        {
            Guid personWithoutAddressId = _context.Person.Create("Ya", "Ya", null, "4016");
            Guid accountId = _context.Account.CreateDebit(_exampleBankId, personWithoutAddressId);

            decimal anonLimit = _context.Bank.ById(_exampleBankId).AnonLimit;

            _context.Account.TopUp(accountId, anonLimit * 10);

            Assert.DoesNotThrow(() => _context.Account.Withdraw(accountId, anonLimit));

            Assert.Catch(() => _context.Account.Withdraw(accountId, anonLimit + 1.1M));
        }

        [Test]
        public void CancelTest()
        {
            Guid accountId = _context.Account.CreateDebit(_exampleBankId, _examplePersonId);

            Guid transactionId = _context.Account.TopUp(accountId, 100);
            Assert.AreEqual(100, _context.Account.AmountAt(accountId));

            _context.Transaction.Cancel(transactionId);
            Assert.AreEqual(0, _context.Account.AmountAt(accountId));

            Assert.Catch(() => _context.Transaction.Cancel(transactionId));
        }

        [Test]
        public void RotateTimeTest()
        {
            Guid accountId = _context.Account.CreateDebit(_exampleBankId, _examplePersonId);
            _context.Account.TopUp(accountId, 10000);
        
            _context.Time.Rotate(TimeLogic.Month + 1);
            Assert.AreEqual(10030, _context.Account.AmountAt(accountId));
        
            _context.Time.Rotate(TimeLogic.Month + 1);
            Assert.AreEqual(10060.09M, _context.Account.AmountAt(accountId));
        }
    }
}