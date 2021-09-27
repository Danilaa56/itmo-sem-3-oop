using System;

namespace Shops.Entities.List
{
    public class ProductListRow
    {
        public ProductListRow(string productId, decimal amount)
        {
            ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
            if (amount < 0) throw new ArgumentException("Amount cannot be negative", nameof(amount));
            Amount = amount;
        }

        public string ProductId { get; }
        public decimal Amount { get; }
    }
}