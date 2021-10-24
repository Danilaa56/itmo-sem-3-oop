using System;

namespace Backups.Server
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 2)
                throw new ArgumentException("Few arguments, at least 2 required");

            int port = int.Parse(args[0]);
            var repoServer = new RepositoryRemoteServer(port, args[1]);
            repoServer.Start();
            
        }
    }
}