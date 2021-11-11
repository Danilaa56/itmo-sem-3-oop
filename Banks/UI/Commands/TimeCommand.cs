// namespace Banks.UI.Commands
// {
//     public class TimeCommand : Command
//     {
//         private Context _context;
//         private CommandResponse _usage = Response("time <passed>");
//
//         public TimeCommand(Context context)
//         {
//             _context = context;
//         }
//
//         public override CommandResponse ProcessCommand(string[] args)
//         {
//             if (args.Length == 1)
//             {
//                 return _usage;
//             }
//
//             switch (args[1].ToLower())
//             {
//                 case "passed":
//                     return Passed(args);
//                 default:
//                     return _usage;
//             }
//         }
//
//         private CommandResponse Passed(string[] args)
//         {
//             if (args.Length != 3)
//                 return Response("person passed SECONDS");
//
//             long seconds = long.Parse(args[2]);
//
//             _context.CentralBank.TimePassed(seconds * 1000);
//             return Response($"{seconds} seconds passed");
//         }
//     }
// }