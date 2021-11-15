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
        [SetUp]
        public void Setup()
        {
            ServiceLogic.Reset();
            BankLogic.RegisterBank(
                "Sample bank",
                3.65M,
                1000,
                10,
                10,
                new Dictionary<decimal, decimal>(),
                TimeLogic.Month,
                100);
            PersonLogic.Create("Ivan", "Ivanov", "ul. Pushkina", "multi");
        }

        [Test]
        public void DebitAccountBelowZero()
        {
            int accountId = AccountLogic.CreateDebit(1, 1);
            AccountLogic.TopUp(accountId, 100);

            Assert.Catch(() => AccountLogic.Withdraw(accountId, 150));
        }

        [Test]
        public void CreateDebitAccountAndTopItUp()
        {
            int accountId = AccountLogic.CreateDebit(1, 1);
            AccountLogic.TopUp(accountId, 100);

            Assert.AreEqual(100, AccountLogic.AmountAt(accountId));
            {
                using var db = new DataContext();
                Assert.AreEqual(2, db.Transactions.Count);
            }
        }

        [Test]
        public void TestCreditAccount()
        {
            int accountId = AccountLogic.CreateCredit(1, 1);
            {
                using var db = new DataContext();
                BankAccount account = db.BankAccountById(accountId);
                Assert.AreEqual(db.BankById(1).CreditLimit, account.AmountAvailable(0));
            }

            decimal withdrawAmount = 100;
            AccountLogic.Withdraw(accountId, withdrawAmount);
            {
                using var db = new DataContext();
                BankAccount account = db.BankAccountById(accountId);
                Bank bank = db.BankById(1);

                decimal amountAtAccount = db.AmountAt(account.Account);
                Assert.AreEqual(-withdrawAmount, amountAtAccount);
                Assert.AreEqual(bank.CreditLimit - withdrawAmount - bank.CreditCommission,
                    account.AmountAvailable(amountAtAccount));
            }

            decimal topUpAmount = 50;
            AccountLogic.TopUp(accountId, topUpAmount);
            {
                using var db = new DataContext();
                BankAccount account = db.BankAccountById(accountId);
                Bank bank = db.BankById(1);

                decimal amountAtAccount = db.AmountAt(account.Account);
                Assert.AreEqual(-withdrawAmount - bank.CreditCommission + topUpAmount, amountAtAccount);
                Assert.AreEqual(bank.CreditLimit - withdrawAmount + topUpAmount - bank.CreditCommission * 2,
                    account.AmountAvailable(amountAtAccount));
            }
        }

        [Test]
        public void AnonLimitTest()
        {
            int personWithoutAddressId = PersonLogic.Create("Ya", "Ya", null, "4016");
            int accountId = AccountLogic.CreateDebit(1, personWithoutAddressId);

            decimal anonLimit;
            using (var db = new DataContext())
            {
                anonLimit = db.BankById(1).AnonLimit;
            }

            AccountLogic.TopUp(accountId, anonLimit * 10);

            Assert.DoesNotThrow(() =>
            {
                AccountLogic.Withdraw(accountId, anonLimit);
            });

            Assert.Catch(() =>
            {
                AccountLogic.Withdraw(accountId, anonLimit + 1.1M);
            });
        }

        [Test]
        public void CancelTest()
        {
            int accountId = AccountLogic.CreateDebit(1, 1);

            int transactionId = AccountLogic.TopUp(accountId, 100);
            Assert.AreEqual(100, AccountLogic.AmountAt(accountId));

            TransactionLogic.Cancel(transactionId);
            Assert.AreEqual(0, AccountLogic.AmountAt(accountId));

            Assert.Catch(() => TransactionLogic.Cancel(transactionId));
        }

        [Test]
        public void RotateTimeTest()
        {
            int accountId = AccountLogic.CreateDebit(1, 1);
            AccountLogic.TopUp(accountId, 10000);

            TimeLogic.RotateTime(TimeLogic.Month + 1);
            Assert.AreEqual(10030, AccountLogic.AmountAt(accountId));

            TimeLogic.RotateTime(TimeLogic.Month + 1);
            Assert.AreEqual(10060.09M, AccountLogic.AmountAt(accountId));
        }
    }
}