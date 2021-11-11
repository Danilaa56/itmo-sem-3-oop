// using System;
// using Banks.Entities;
//
// namespace Banks
// {
//     public class Context : IDisposable
//     {
//         public CentralBank CentralBank { get; } = new CentralBank();
//         public PeopleRegistry PeopleRegistry { get; } = new PeopleRegistry();
//         public BanksDbContext Database { get; } = new BanksDbContext();
//
//         public void Dispose()
//         {
//             Database.Dispose();
//         }
//     }
// }