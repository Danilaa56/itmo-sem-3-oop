using System;
using System.Collections.Generic;
using Shops.Entities;
using Shops.Tools;

namespace Shops.Service
{
    public class ShopManager
    {
        private readonly Dictionary<string, Shop> _shops = new ();

        public void RegisterShop(string id, string name, string shopAddress)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (shopAddress == null)
                throw new ArgumentNullException(nameof(shopAddress));
            if (_shops.ContainsKey(id))
                throw new ShopException("Shop with such id already exists");

            _shops[id] = new Shop(id, name, shopAddress);
        }

        public bool UnregisterShop(string id)
        {
            return _shops.Remove(id);
        }

        public void Deliver(string shopId, List<ProductStack> productStacks)
        {
            if (shopId == null)
                throw new ArgumentNullException(nameof(shopId));
            if (!_shops.ContainsKey(shopId))
                throw new ShopException("No shop with such id");

            _shops[shopId].Deliver(productStacks);
        }

        public void SetPriceForProductIn(string productId, string shopId, decimal newPrice)
        {
            if (productId == null)
                throw new ArgumentNullException(nameof(productId));
            if (shopId == null)
                throw new ArgumentNullException(nameof(shopId));
            if (!_shops.ContainsKey(shopId))
                throw new ShopException("No shop with such id");

            _shops[shopId].SetPriceForProduct(productId, newPrice);
        }

        public decimal HowMuchDoesItCostIn(Dictionary<string, decimal> productsList, string shopId)
        {
            if (shopId == null)
                throw new ArgumentNullException(nameof(shopId));
            if (!_shops.ContainsKey(shopId))
                throw new ShopException("No shop with such id");

            return _shops[shopId].HowMuchDoesItCost(productsList);
        }

        public bool CanBuyIn(Dictionary<string, decimal> productsList, string shopId, decimal moneyAvailable)
        {
            decimal price = HowMuchDoesItCostIn(productsList, shopId);
            if (price == -1)
                return false;
            return price <= moneyAvailable;
        }

        public string FindShopWithTheLowestPrice(Dictionary<string, decimal> productsList)
        {
            decimal lowestSum = -1;
            Shop foundShop = null;
            foreach (KeyValuePair<string, Shop> shop in _shops)
            {
                decimal sum = shop.Value.HowMuchDoesItCost(productsList);
                if (sum != -1 && (sum < lowestSum || lowestSum == -1))
                {
                    lowestSum = sum;
                    foundShop = shop.Value;
                }
            }

            return foundShop?.Id;
        }

        public bool PersonBuyIn(Person person, Dictionary<string, decimal> productsList, string shopId)
        {
            if (person == null)
                throw new ArgumentNullException(nameof(person));
            decimal sum = HowMuchDoesItCostIn(productsList, shopId);
            if (sum == -1 || sum > person.Money)
                return false;

            if (!_shops[shopId].Buy(productsList))
                return false;
            person.Money -= sum;
            return true;
        }

        public decimal HowManyIn(string productId, string shopId)
        {
            if (shopId == null)
                throw new ArgumentNullException(nameof(shopId));
            if (!_shops.ContainsKey(shopId))
                throw new ShopException("No shop with such id");
            return _shops[shopId].HowMany(productId);
        }
    }
}