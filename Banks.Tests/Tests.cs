using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
using Banks.Entities.Accounts;
using NUnit.Framework;

namespace Banks.Tests
{
    [TestFixture]
    public class Tests
    {
        private CentralBank _centralBank;
        private Bank _bank;
        private Person _person;

        [SetUp]
        public void Setup()
        {
            _centralBank = new CentralBank();
            var depositLevels = new Dictionary<decimal, decimal> {{50000, 3.5M}, {10000, 4M}};
            _bank = new Bank(
                _centralBank,
                "Sample bank",
                3.65M,
                1000,
                10,
                10,
                depositLevels,
                Bank.Month,
                100);
            _person = Person.Builder("Ivan", "Ivanov").Address("ul. Pushkina").PassportId("multi").Build();
        }

        [Test]
        public void DebitAccountBelowZero()
        {
            DebitAccount bankAccount = _bank.CreateDebitAccount(_person);
            bankAccount.TopUp(100);

            Assert.Catch(() => bankAccount.Withdraw(150));
        }

        [Test]
        public void CreateDebitAccountAndTopItUp()
        {
            DebitAccount bankAccount = _bank.CreateDebitAccount(_person);
            bankAccount.TopUp(100);

            Assert.AreEqual(100, bankAccount.Amount);
            Assert.AreEqual(2, _centralBank.Transactions().Count);
        }

        [Test]
        public void TestCreditAccount()
        {
            CreditAccount bankAccount = _bank.CreateCreditAccount(_person);
            Assert.AreEqual(1000, bankAccount.AmountAvailable());

            bankAccount.Withdraw(100);
            Assert.AreEqual(-100, bankAccount.Amount);
            Assert.AreEqual(890, bankAccount.AmountAvailable());

            bankAccount.TopUp(50);
            Assert.AreEqual(-60, bankAccount.Amount);
            Assert.AreEqual(930, bankAccount.AmountAvailable());

            bankAccount.TopUp(100);
            Assert.AreEqual(30, bankAccount.Amount);
            Assert.AreEqual(1030, bankAccount.AmountAvailable());

            bankAccount.TopUp(100);
            Assert.AreEqual(130, bankAccount.Amount);
            Assert.AreEqual(1130, bankAccount.AmountAvailable());
        }
        
        [Test]
        public void AnonLimitTest()
        {
            Person personWithoutAddress = Person.Builder("Ya", "Ya").PassportId("4016").Build();
            
            DebitAccount bankAccount = _bank.CreateDebitAccount(personWithoutAddress);
            bankAccount.TopUp(1000);

            Assert.DoesNotThrow(() =>
            {
                bankAccount.Withdraw(100);
            });
            
            Assert.Catch(() =>
            {
                bankAccount.Withdraw(101);
            });
        }
        
        [Test]
        public void CancelTest()
        {
            DebitAccount bankAccount = _bank.CreateDebitAccount(_person);
            bankAccount.TopUp(100);
            
            Assert.AreEqual(100, bankAccount.Amount);
            
            _centralBank.CancelTransaction(_centralBank.Transactions().Last());

            Assert.AreEqual(0, bankAccount.Amount);
        }
        
        [Test]
        public void RotateTimeTest()
        {
            DebitAccount bankAccount = _bank.CreateDebitAccount(_person);
            bankAccount.TopUp(10000);
            
            _centralBank.TimePassed(Bank.Month + 1);
            Assert.AreEqual(10030, bankAccount.Amount);
            
            _centralBank.TimePassed(Bank.Month + 1);
            Assert.AreEqual(10060.09M, bankAccount.Amount);
        }
    }
}