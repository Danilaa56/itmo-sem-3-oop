using System.Collections.Generic;
using NUnit.Framework;
using Shops.Entities;
using Shops.Service;

namespace Shops.Tests
{
    [TestFixture]
    public class Tests
    {
        private ShopManager _manager;

        private const string UsualShopId = "shop1";

        [SetUp]
        public void Setup()
        {
            _manager = new ShopManager();
        }

        [TestCase("item1", 10, 100, true)]
        [TestCase("item1", 10, 5, false)]
        [TestCase("item2", 3, 100, false)]
        [TestCase("item3", 1, 100, false)]
        public void TestDelivery(string id, decimal count, decimal moneyAvailable, bool canBuy)
        {
            _manager.RegisterShop(UsualShopId, "Shestoro4ka", "ul Pushkina");

            var productsToDeliver = new List<ProductStack>
            {
                new("item1", 10, 1),
                new("item2", 2, 2)
            };

            _manager.Deliver("shop1", productsToDeliver);

            var productsToBuy = new Dictionary<string, decimal>
            {
                {id, count}
            };

            Assert.AreEqual(canBuy, _manager.CanBuyIn(productsToBuy, UsualShopId, moneyAvailable));

            _manager.UnregisterShop(UsualShopId);
        }

        [Test]
        public void ChangePriceTest()
        {
            _manager.RegisterShop(UsualShopId, "Sem ya", "ul Esenina");

            var productsToDeliver = new List<ProductStack>
            {
                new ProductStack("grecha", 1, 1)
            };

            _manager.Deliver(UsualShopId, productsToDeliver);

            var productsToBuy = new Dictionary<string, decimal>
            {
                {"grecha", 1}
            };

            Assert.AreEqual(true, _manager.CanBuyIn(productsToBuy, UsualShopId, 1));
            _manager.SetPriceForProductIn("grecha", UsualShopId, 1.1M);
            Assert.AreEqual(false, _manager.CanBuyIn(productsToBuy, UsualShopId, 1));

            _manager.UnregisterShop(UsualShopId);
        }

        [TestCase(1, 1, "shop2")]
        [TestCase(1, 3, "shop1")]
        [TestCase(4, 0, "shop1")]
        [TestCase(1, 5, null)]
        public void FindLowestPriceTest(decimal riceCount, decimal oatCount, string lowestShop)
        {
            _manager.RegisterShop("shop1", "Poloska", "ul Chechova");
            _manager.RegisterShop("shop2", "Nolick", "ul Pestelya");

            _manager.Deliver("shop1", new List<ProductStack>
            {
                new("rice", 5, 2),
                new("oat", 3, 1.5M),
            });

            _manager.Deliver("shop2", new List<ProductStack>
            {
                new("rice", 3, 1.5M),
                new("oat", 3, 1.7M),
            });

            var productsToBuy = new Dictionary<string, decimal>
            {
                {"rice", riceCount},
                {"oat", oatCount}
            };

            Assert.AreEqual(lowestShop, _manager.FindShopWithTheLowestPrice(productsToBuy));

            _manager.UnregisterShop("shop1");
            _manager.UnregisterShop("shop2");
        }

        [TestCase(10, 0.5, 1, 1, true)]
        [TestCase(10, 10, 0, 1.5, false)]
        [TestCase(10, 10, 5, 0, false)]
        public void BuyProductsTest(decimal moneyBefore, decimal moneyAfter, decimal potatoCount, decimal shrimpCount,
            bool isSuccess)
        {
            _manager.RegisterShop(UsualShopId, "Rouble", "ul HeKpacoBa");

            _manager.Deliver(UsualShopId, new List<ProductStack>
            {
                new("potato", 4, 2),
                new("shrimp", 2, 7.5M),
            });

            var person = new Person("Vanya");
            person.Money = moneyBefore;

            var productsToBuy = new Dictionary<string, decimal>
            {
                {"potato", potatoCount},
                {"shrimp", shrimpCount}
            };

            Assert.AreEqual(isSuccess, _manager.PersonBuyIn(person, productsToBuy, UsualShopId));
            Assert.AreEqual(moneyAfter, person.Money);
            if (isSuccess)
            {
                Assert.AreEqual(4 - potatoCount, _manager.HowManyIn("potato", UsualShopId));
                Assert.AreEqual(2 - shrimpCount, _manager.HowManyIn("shrimp", UsualShopId));
            }
            else
            {
                Assert.AreEqual(4, _manager.HowManyIn("potato", UsualShopId));
                Assert.AreEqual(2, _manager.HowManyIn("shrimp", UsualShopId));
            }

            _manager.UnregisterShop(UsualShopId);
        }
    }
}