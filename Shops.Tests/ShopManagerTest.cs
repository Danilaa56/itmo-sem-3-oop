using System.Collections.Generic;
using NUnit.Framework;
using Shops.Entities;
using Shops.Entities.List;
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
            var shop = new Shop(UsualShopId, "Shestoro4ka", "ul Pushkina");
            _manager.RegisterShop(shop);

            var productsToDeliver = new PricedProductList()
                .Add("item1", 10, 1)
                .Add("item2", 2, 2);

            shop.Deliver(productsToDeliver);

            var productsToBuy = new ProductList().Add(id, count);

            Assert.AreEqual(canBuy, _manager.CanBuyIn(productsToBuy, UsualShopId, moneyAvailable));
        }

        [Test]
        public void ChangePriceTest()
        {
            var shop = new Shop(UsualShopId, "Sem ya", "ul Esenina");

            var productsToDeliver = new PricedProductList()
                .Add("grecha", 1, 1);

            shop.Deliver(productsToDeliver);

            Assert.AreEqual(1, shop.GetProductPrice("grecha"));
            shop.TryChangePriceForProduct("grecha", 1.1M);
            Assert.AreEqual(1.1M, shop.GetProductPrice("grecha"));
        }

        [TestCase(1, 1, "shop2")]
        [TestCase(1, 3, "shop1")]
        [TestCase(4, 0, "shop1")]
        [TestCase(1, 5, null)]
        public void FindLowestPriceTest(decimal riceAmount, decimal oatAmount, string lowestShop)
        {
            var shop1 = new Shop("shop1", "Poloska", "ul Chechova");
            var shop2 = new Shop("shop2", "Nolick", "ul Pestelya");

            _manager.RegisterShop(shop1);
            _manager.RegisterShop(shop2);
            
            shop1.Deliver(new PricedProductList()
                .Add("rice", 5, 2)
                .Add("oat", 3, 1.5M)
            );

            shop2.Deliver(new PricedProductList()
                .Add("rice", 3, 1.5M)
                .Add("oat", 3, 1.7M)
            );

            var productsToBuy = new ProductList()
                .Add("rice", riceAmount)
                .Add("oat", oatAmount);

            if(lowestShop == null)
                Assert.AreEqual(lowestShop, _manager.FindShopWithTheLowestPrice(productsToBuy));
            else
                Assert.AreEqual(lowestShop, _manager.FindShopWithTheLowestPrice(productsToBuy).Id);
        }

        [TestCase(10, 0.5, 1, 1, true)]
        [TestCase(10, 10, 0, 1.5, false)]
        [TestCase(10, 10, 5, 0, false)]
        public void BuyProductsTest(decimal moneyBefore, decimal moneyAfter, decimal potatoAmount, decimal shrimpAmount,
            bool isSuccess)
        {
            var shop = new Shop(UsualShopId, "Rouble", "ul HeKpacoBa");

            shop.Deliver(new PricedProductList()
                .Add("potato", 4, 2)
                .Add("shrimp", 2, 7.5M)
            );

            var person = new Person("Vanya");
            person.Money = moneyBefore;

            var productsToBuy = new ProductList()
                .Add("potato", potatoAmount)
                .Add("shrimp", shrimpAmount);

            if (isSuccess)
                Assert.DoesNotThrow(() => _manager.PersonBuyIn(person, shop, productsToBuy));
            else
                Assert.Catch(() => _manager.PersonBuyIn(person, shop, productsToBuy));
            
            Assert.AreEqual(moneyAfter, person.Money);
            if (isSuccess)
            {
                Assert.AreEqual(4 - potatoAmount, shop.GetProductAmount("potato"));
                Assert.AreEqual(2 - shrimpAmount, shop.GetProductAmount("shrimp"));
            }
            else
            {
                Assert.AreEqual(4, shop.GetProductAmount("potato"));
                Assert.AreEqual(2, shop.GetProductAmount("shrimp"));
            }
        }
    }
}