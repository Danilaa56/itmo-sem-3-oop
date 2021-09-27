using System;

namespace Shops.Entities.List
{
    public class PricedProductList : AbstractProductList<PricedProductListRow>
    {
        public PricedProductList Add(string productId, decimal amount, decimal pricePerOne)
        {
            Add(new PricedProductListRow(productId, amount, pricePerOne));
            return this;
        }

        protected override PricedProductListRow MergeRows(PricedProductListRow row1, PricedProductListRow row2)
        {
            if (!row1.ProductId.Equals(row2.ProductId))
                throw new ArgumentException("Rows must have the same id");
            return new PricedProductListRow(row1.ProductId, row1.Amount + row2.Amount, row1.PricePerOne);
        }

        protected override string GetRowId(PricedProductListRow row)
        {
            return row.ProductId;
        }
    }
}