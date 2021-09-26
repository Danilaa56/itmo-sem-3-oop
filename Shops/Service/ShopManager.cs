using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Shops.Entities;
using Shops.Entities.List;
using Shops.Tools;

namespace Shops.Service
{
    public class ShopManager
    {
        private readonly Dictionary<string, Shop> _shops = new Dictionary<string, Shop>();

        public void RegisterShop(Shop shop)
        {
            if (_shops.ContainsKey(shop.Id))
                throw new ShopException("Shop with such id already exists");

            _shops[shop.Id] = shop;
        }

        public void UnregisterShop(string shopId)
        {
            if (!_shops.Remove(shopId))
                throw new ShopException("There is no shop with such id");
        }

        public ImmutableList<Shop> GetShops()
        {
            return _shops.Values.ToImmutableList();
        }

        public Shop GetShopById(string shopId)
        {
            if (shopId is null)
                throw new ArgumentNullException(nameof(shopId));
            if (_shops.TryGetValue(shopId, out Shop shop))
            {
                return shop;
            }

            throw new ShopException("Shop with such id does not exist");
        }

        public decimal HowMuchDoesItCostIn(ProductList productList, string shopId)
        {
            if (_shops.TryGetValue(shopId, out Shop shop))
            {
                return shop.HowMuchDoesItCost(productList);
            }

            throw new ShopException("No shop with such id");
        }

        public bool CanBuyIn(ProductList productList, string shopId, decimal moneyAvailable)
        {
            try
            {
                return HowMuchDoesItCostIn(productList, shopId) <= moneyAvailable;
            }
            catch (ShopException)
            {
                return false;
            }
        }

        public decimal FindShopWithTheLowestPrice(ProductList productList, out Shop bestShop)
        {
            bestShop = null;
            decimal lowestPrice = -1;
            foreach (Shop shop in _shops.Values)
            {
                try
                {
                    decimal price = shop.HowMuchDoesItCost(productList);
                    if (price < lowestPrice || bestShop == null)
                    {
                        lowestPrice = price;
                        bestShop = shop;
                    }
                }
                catch (ShopException)
                {
                }
            }

            return lowestPrice;
        }

        public void PersonBuyIn(Person person, Shop shop, ProductList productList)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));
            if (shop == null)
                throw new ArgumentNullException(nameof(shop));
            if (productList == null)
                throw new ArgumentNullException(nameof(productList));

            decimal price = shop.HowMuchDoesItCost(productList);
            if (price > person.Money)
                throw new ShopException("Person do not have enough money");

            person.Money -= price;
            shop.Buy(productList);
        }
    }
}