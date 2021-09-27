using Shops.Entities;
using Shops.Entities.List;
using Shops.Service;

namespace Shops
{
    public class Context
    {
        public ShopManager ShopManager { get; } = new ShopManager();
        public PeopleRegistry PeopleRegistry { get; } = new PeopleRegistry();
        public ProductList ProductList { get; } = new ProductList();
        public PricedProductList DeliveryList { get; } = new PricedProductList();
    }
}