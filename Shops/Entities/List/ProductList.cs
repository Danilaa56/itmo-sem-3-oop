using System;

namespace Shops.Entities.List
{
    public class ProductList : AbstractProductList<ProductListRow>
    {
        public ProductList Add(string productId, decimal amount)
        {
            Add(new ProductListRow(productId, amount));
            return this;
        }

        protected override ProductListRow MergeRows(ProductListRow row1, ProductListRow row2)
        {
            if (!row1.ProductId.Equals(row2.ProductId))
                throw new ArgumentException("Rows must have the same id");
            return new ProductListRow(row1.ProductId, row1.Amount + row2.Amount);
        }

        protected override string GetRowId(ProductListRow row)
        {
            return row.ProductId;
        }
    }
}