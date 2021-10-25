using System.IO;
using Backups.Entities;

namespace Backups.Server.Entities.Actions
{
    public interface IAction
    {
        bool Proc(string prefix, Stream stream, IRepository repo);
    }
}