using System;

namespace Shops.Entities.List
{
    public class PricedProductListRow
    {
        public PricedProductListRow(string productId, decimal amount, decimal pricePerOne)
        {
            ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
            if (amount < 0) throw new ArgumentException("Amount cannot be negative", nameof(amount));
            Amount = amount;
            if (pricePerOne < 0) throw new ArgumentException("Price per one cannot be negative", nameof(pricePerOne));
            PricePerOne = pricePerOne;
        }

        public string ProductId { get; }
        public decimal Amount { get; }
        public decimal PricePerOne { get; }

        public PricedProductListRow Merge(PricedProductListRow other)
        {
            if (other is null)
                throw new ArgumentNullException(nameof(other));
            if (!ProductId.Equals(other.ProductId))
                throw new ArgumentException("Product ids must be equal", nameof(other));
            return new PricedProductListRow(ProductId, Amount + other.Amount, PricePerOne);
        }
    }
}