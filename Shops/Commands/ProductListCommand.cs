using System.Linq;
using Shops.Entities;
using Shops.Tools;

namespace Shops.Commands
{
    public class ProductListCommand : Command
    {
        private Context _context;
        private CommandResponse _usage = Response("productlist <clear|show|add|lowestprice|buy>");

        public ProductListCommand(Context context)
        {
            _context = context;
        }

        public override CommandResponse ProcCommand(string[] args)
        {
            if (args.Length == 1)
            {
                return _usage;
            }

            switch (args[1].ToLower())
            {
                case "clear":
                    return Clear();
                case "show":
                    return Show();
                case "add":
                    return Add(args);
                case "lowestprice":
                    return LowestPrice();
                case "buy":
                    return Buy(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Clear()
        {
            _context.ProductList.Clear();
            return Response("Product list was cleared");
        }

        private CommandResponse Show()
        {
            var rows = _context.ProductList.GetRows();
            return Response(
                rows.ConvertAll(row => row.ProductId + "\t" + row.Amount)
                    .Insert(0, "Different product types count: " + rows.Count).ToArray());
        }

        private CommandResponse LowestPrice()
        {
            decimal lowestPrice = _context.ShopManager.FindShopWithTheLowestPrice(_context.ProductList, out Shop shop);
            if (lowestPrice == -1)
                return Response("There is no shop with such products");

            return Response($"Lowest price of the list is {lowestPrice} in the '{shop.Name}' ('{shop.Id}')");
        }

        private CommandResponse Add(string[] args)
        {
            if (args.Length != 4)
                return Response("productlist add PRODUCT_ID PRODUCT_AMOUNT");

            _context.ProductList.Add(args[2], decimal.Parse(args[3]));
            return Response("Product was added to the list");
        }

        private CommandResponse Buy(string[] args)
        {
            if (args.Length != 4)
                return Response("productlist buy PERSON_WHO_BUYS_ID SHOP_WHERE_BUYS_ID");

            Person person = _context.PeopleRegistry.GetById(args[2]);
            Shop shop = _context.ShopManager.GetShopById(args[3]);

            _context.ShopManager.PersonBuyIn(person, shop, _context.ProductList);
            return Response("Products were successfully bought");
        }
    }
}