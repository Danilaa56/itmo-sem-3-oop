using System;
using System.Collections.Immutable;
using System.Net.Sockets;
using Backups.Tools;

namespace Backups.Entities
{
    public class RepositoryRemote : IRepository
    {
        public RepositoryRemote(string address, int port)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            if (port is < 0 or > 65535)
                throw new ArgumentException("Invalid port", nameof(address));
            Port = port;
        }

        public enum ActionCode : byte
        {
            CreateStorage = 0,
            GetStorages = 1,
        }

        public string Address { get; }
        public int Port { get; }

        public string CreateStorage(byte[] data)
        {
            try
            {
                var tcpClient = new TcpClient(Address, Port);
                NetworkStream stream = tcpClient.GetStream();

                StreamUtils.WriteAction(stream, ActionCode.CreateStorage);
                StreamUtils.WriteByteArray(stream, data);
                string storageId = StreamUtils.ReadString(stream);

                stream.Close();
                tcpClient.Close();
                return storageId;
            }
            catch (SocketException e)
            {
                throw new BackupException("Failed send file to the remote repo", e);
            }
        }

        public ImmutableArray<string> GetStorages()
        {
            try
            {
                var tcpClient = new TcpClient(Address, Port);
                NetworkStream stream = tcpClient.GetStream();

                StreamUtils.WriteAction(stream, ActionCode.GetStorages);

                ImmutableArray<string> storageIds = StreamUtils.ReadStringList(stream);

                stream.Close();
                tcpClient.Close();
                return storageIds;
            }
            catch (SocketException e)
            {
                throw new BackupException("Failed get storages list from the remote repo", e);
            }
        }
    }
}