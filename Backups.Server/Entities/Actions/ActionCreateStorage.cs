using System;
using System.IO;
using Backups.Entities;
using Backups.Tools;

namespace Backups.Server.Entities.Actions
{
    public class ActionCreateStorage : IAction
    {
        public bool Proc(string prefix, Stream stream, IRepository repo)
        {
            Console.WriteLine(prefix + "action code: create storage; reading data");
            byte[] data = StreamUtils.ReadByteArray(stream);
            string storageId = repo.CreateStorage(data);
            Console.WriteLine($"{prefix}storage {storageId} was created, sending");
            StreamUtils.WriteString(stream, storageId);
            Console.WriteLine($"{prefix}storage id is sent, closing connection");
            return false;
        }
    }
}