using Shops.Tools;

namespace Shops.Entities
{
    public class ProductStack
    {
        public ProductStack(string id, decimal count, decimal pricePerOne)
        {
            if (pricePerOne < 0)
                throw new ShopException("Price cannot be negative");
            Id = id;
            Count = count;
            PricePerOne = pricePerOne;
        }

        public string Id { get; }

        public decimal Count { get; }

        public decimal PricePerOne { get; }
    }
}