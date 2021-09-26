using System;
using Shops.Entities.List;
using Shops.Tools;

namespace Shops.Entities
{
    public class Shop
    {
        private PricedProductList _products = new PricedProductList();

        public Shop(string id, string name, string address)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public string Id { get; }
        public string Name { get; }
        public string Address { get; }

        public void Deliver(PricedProductList list)
        {
            _products.Add(list);
        }

        public void TryChangePriceForProduct(string productId, decimal newPricePerOne)
        {
            if (productId is null)
                throw new ArgumentNullException(nameof(productId));
            if (newPricePerOne < 0)
                throw new ArgumentException("Price cannot be negative", nameof(newPricePerOne));
            if (_products.TryGetValue(productId, out PricedProductListRow productInfo))
            {
                _products.Put(new PricedProductListRow(productId, productInfo.Amount, newPricePerOne));
            }
        }

        public decimal HowMuchDoesItCost(ProductList productList)
        {
            if (productList == null)
                throw new ArgumentNullException(nameof(productList));
            decimal sum = 0;

            productList.GetRows().ForEach(row =>
            {
                PricedProductListRow existingRow;
                if (!_products.TryGetValue(row.ProductId, out existingRow))
                {
                    throw new ShopException($"There is no such product {row.ProductId} in the shop '{Id}'");
                }

                if (existingRow.Amount < row.Amount)
                {
                    throw new ShopException(
                        $"There is no such amount of product {row.ProductId} in the shop '{Id}'");
                }

                sum += existingRow.PricePerOne * row.Amount;
            });

            return sum;
        }

        public void Buy(ProductList productList)
        {
            if (productList == null)
                throw new ArgumentNullException(nameof(productList));
            foreach (var productInfo in productList.GetRows())
            {
                if (_products.TryGetValue(productInfo.ProductId, out PricedProductListRow existingProduct))
                {
                    if (existingProduct.Amount < productInfo.Amount)
                        throw new ShopException("There is no such amount of product in shop");
                }
                else
                {
                    throw new ShopException("There is no such product in shop");
                }
            }

            foreach (var productInfo in productList.GetRows())
            {
                _products.TryGetValue(productInfo.ProductId, out PricedProductListRow existingProduct);
                if (existingProduct.Amount == productInfo.Amount)
                {
                    _products.TryRemove(existingProduct.ProductId);
                }
                else
                {
                    _products.Put(new PricedProductListRow(
                        existingProduct.ProductId,
                        existingProduct.Amount - productInfo.Amount,
                        existingProduct.PricePerOne));
                }
            }
        }

        public decimal GetProductAmount(string productId)
        {
            if (productId == null)
                throw new ArgumentNullException(nameof(productId));
            if (_products.TryGetValue(productId, out PricedProductListRow productInfo))
            {
                return productInfo.Amount;
            }

            return 0;
        }

        public decimal GetProductPrice(string productId)
        {
            if (productId == null)
                throw new ArgumentNullException(nameof(productId));
            if (_products.TryGetValue(productId, out PricedProductListRow productInfo))
            {
                return productInfo.PricePerOne;
            }

            throw new ShopException("There is no product with such id int this shop");
        }
    }
}