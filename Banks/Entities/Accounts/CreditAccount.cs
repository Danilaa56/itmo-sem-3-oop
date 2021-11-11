// using System;
// using Banks.Tools;
//
// namespace Banks.Entities.Accounts
// {
//     public class CreditAccount : BankAccount
//     {
//         public CreditAccount(Account account)
//             : base(account)
//         {
//         }
//
//         public decimal CreditLimit => Bank.CreditLimit;
//         public decimal Commission => Bank.CreditCommission;
//
//         public override decimal AmountAvailable()
//         {
//             decimal amountAvailable = Amount;
//             decimal currentCommission = amountAvailable < 0 ? Commission : 0;
//             return amountAvailable + Bank.CreditLimit - currentCommission;
//         }
//
//         public override decimal CommissionTopUp(decimal amount)
//         {
//             return Amount < 0 ? Commission : 0;
//         }
//
//         public override decimal CommissionWithdraw(decimal amount)
//         {
//             decimal amountAvailable = Amount;
//             decimal currentCommission = amountAvailable < 0 ? Commission : 0;
//             amountAvailable += CreditLimit - currentCommission;
//             if (amount > amountAvailable)
//                 throw new BankException($"Such amount of money={amount} is not available for the account");
//             return currentCommission;
//         }
//     }
// }