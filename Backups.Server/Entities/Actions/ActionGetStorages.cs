using System;
using System.Collections.Immutable;
using System.IO;
using Backups.Entities;
using Backups.Tools;

namespace Backups.Server.Entities.Actions
{
    public class ActionGetStorages : IAction
    {
        public bool Proc(string prefix, Stream stream, IRepository repo)
        {
            Console.WriteLine(prefix + "action code: get storages; sending data");
            ImmutableArray<string> storageIds = repo.GetStorages();
            StreamUtils.WriteStringList(stream, storageIds);
            Console.WriteLine($"{prefix}storages id is sent, closing connection");
            return false;
        }
    }
}