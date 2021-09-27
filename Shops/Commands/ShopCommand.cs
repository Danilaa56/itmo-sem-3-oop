using System.Collections.Immutable;
using System.Linq;
using Shops.Entities;
using Shops.Service;
using Shops.Tools;

namespace Shops.Commands
{
    public class ShopCommand : Command
    {
        private ShopManager _manager;
        private CommandResponse _usage = Response("shop <create|destroy|list>");

        public ShopCommand(Context context)
        {
            _manager = context.ShopManager;
        }

        public override CommandResponse ProcCommand(string[] args)
        {
            if (args.Length == 1)
            {
                return _usage;
            }

            switch (args[1].ToLower())
            {
                case "create":
                    return Create(args);
                case "destroy":
                    return Destroy(args);
                case "list":
                    return List(args);
                default:
                    return _usage;
            }
        }

        private CommandResponse Create(string[] args)
        {
            if (args.Length != 5)
                return Response("shop create SHOP_ID SHOP_NAME SHOP_ADDRESS");

            var shop = new Shop(args[2], args[3], args[4]);
            _manager.RegisterShop(shop);
            return Response("Shop was created");
        }

        private CommandResponse Destroy(string[] args)
        {
            if (args.Length != 3)
                return Response("shop create SHOP_ID");

            _manager.UnregisterShop(args[2]);
            return Response($"Shop '{args[2]}' was destroyed");
        }

        private CommandResponse List(string[] args)
        {
            ImmutableList<Shop> shops = _manager.GetShops();
            return Response(
                shops.ConvertAll(shop => shop.Id + "\t" + shop.Name + "\t" + shop.Address)
                .Insert(0, "Shops count: " + shops.Count).ToArray());
        }
    }
}