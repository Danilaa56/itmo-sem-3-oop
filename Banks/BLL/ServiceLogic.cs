using System.Collections.Generic;

namespace Banks.BLL
{
    public static class ServiceLogic
    {
        public static void Reset()
        {
            using var db = new DataContext();
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        public static void Default()
        {
            Reset();

            BankLogic.RegisterBank(
                "GoodBank",
                2,
                1000,
                10,
                3,
                new Dictionary<decimal, decimal>(),
                24 * 3600 * 1000,
                100);
            PersonLogic.Create("Ivan", "Ivanov", null, null);
        }
    }
}