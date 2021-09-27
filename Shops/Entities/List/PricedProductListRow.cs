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
    }
}