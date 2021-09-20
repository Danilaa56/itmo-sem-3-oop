using System;
using System.Collections.Generic;

namespace Shops.Entities
{
    public class Shop
    {
        private readonly Dictionary<string, ProductStack> _productStacks = new ();

        public Shop(string id, string name, string address)
        {
            Id = id;
            Name = name;
            Address = address;
        }

        public string Id { get; }
        public string Name { get; }
        public string Address { get; }

        public void Deliver(List<ProductStack> productStacks)
        {
            productStacks?.ForEach(productStack =>
            {
                if (_productStacks.ContainsKey(productStack.Id))
                {
                    _productStacks[productStack.Id] = new ProductStack(
                        productStack.Id,
                        _productStacks[productStack.Id].Count + productStack.Count,
                        _productStacks[productStack.Id].PricePerOne);
                }
                else
                {
                    _productStacks[productStack.Id] = productStack;
                }
            });
        }

        public void SetPriceForProduct(string productId, decimal newPricePerOne)
        {
            if (_productStacks.ContainsKey(productId))
            {
                var productStack = new ProductStack(productId, _productStacks[productId].Count, newPricePerOne);
                _productStacks[productId] = productStack;
            }
        }

        public decimal HowMuchDoesItCost(Dictionary<string, decimal> productsList)
        {
            if (productsList == null)
                throw new ArgumentNullException(nameof(productsList));
            decimal sum = 0;
            foreach (KeyValuePair<string, decimal> product in productsList)
            {
                if (!_productStacks.ContainsKey(product.Key))
                    return -1;
                if (_productStacks[product.Key].Count < product.Value)
                    return -1;
                sum += product.Value * _productStacks[product.Key].PricePerOne;
            }

            return sum;
        }

        public bool Buy(Dictionary<string, decimal> productsList)
        {
            if (productsList == null)
                throw new ArgumentNullException(nameof(productsList));
            foreach (KeyValuePair<string, decimal> product in productsList)
            {
                if (!_productStacks.ContainsKey(product.Key))
                    return false;
                if (_productStacks[product.Key].Count < product.Value)
                    return false;
            }

            foreach (KeyValuePair<string, decimal> product in productsList)
            {
                if (_productStacks[product.Key].Count == product.Value)
                {
                    _productStacks.Remove(product.Key);
                }
                else
                {
                    _productStacks[product.Key] = new ProductStack(
                        product.Key,
                        _productStacks[product.Key].Count - product.Value,
                        _productStacks[product.Key].PricePerOne);
                }
            }

            return true;
        }

        public decimal HowMany(string productId)
        {
            if (productId == null)
                throw new ArgumentNullException(nameof(productId));
            if (_productStacks.ContainsKey(productId))
                return _productStacks[productId].Count;
            return 0;
        }
    }
}