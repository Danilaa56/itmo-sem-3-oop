using System;
using System.Linq;
using Shops.Entities;

namespace Shops.Commands
{
    public class DeliveryListCommand : Command
    {
        private Context _context;
        private string[] _usage = Response("deliverylsit <clear|show|buy|person>");

        public DeliveryListCommand(Context context)
        {
            _context = context;
        }

        public override string[] ProcCommand(string[] args)
        {
            if (args.Length == 1)
            {
                return _usage;
            }

            try
            {
                switch (args[1].ToLower())
                {
                    case "clear":
                        return Clear();
                    case "show":
                        return Show();
                    case "add":
                        return Add(args);
                    case "deliver":
                        return Deliver(args);
                }
            }
            catch (Exception e)
            {
                return Response(e.Message);
            }

            return _usage;
        }

        private string[] Clear()
        {
            _context.DeliveryList.Clear();
            return Response("Delivery list was cleared");
        }

        private string[] Show()
        {
            var rows = _context.DeliveryList.GetRows();
            return rows.ConvertAll(row => row.ProductId + "\t" + row.Amount + "\t" + row.PricePerOne)
                .Insert(0, "Different product types count: " + rows.Count).ToArray();
        }

        private string[] Add(string[] args)
        {
            if (args.Length != 5)
                return Response("deliverylist add PRODUCT_ID PRODUCT_AMOUNT PRICE_PER_ONE");

            _context.DeliveryList.Add(args[2], decimal.Parse(args[3]), decimal.Parse(args[4]));
            return Response("Product was added to the list");
        }

        private string[] Deliver(string[] args)
        {
            if (args.Length != 3)
                return Response("deliverylist deliver SHOP_ID");

            Shop shop = _context.ShopManager.GetShopById(args[2]);

            shop.Deliver(_context.DeliveryList);
            return Response("Products were successfully delivered");
        }
    }
}