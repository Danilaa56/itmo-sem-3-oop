using System;
using System.Collections.Immutable;
using System.Net.Sockets;
using Backups.Tools;

namespace Backups.Entities.Repository
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
            RemoveStorage = 2,
            ReadStorage = 3,
        }

        public string Address { get; }
        public int Port { get; }

        public string CreateStorage(byte[] data)
        {
            try
            {
                using var tcpClient = new TcpClient(Address, Port);
                using NetworkStream stream = tcpClient.GetStream();

                stream.WriteAction(ActionCode.CreateStorage);
                stream.WriteByteArray(data);

                return stream.ReadString();
            }
            catch (SocketException e)
            {
                throw new BackupException("Failed send file to the remote repo", e);
            }
        }

        public void RemoveStorage(string storageId)
        {
            if (storageId is null)
                throw new ArgumentNullException(nameof(storageId));
            try
            {
                using var tcpClient = new TcpClient(Address, Port);
                using NetworkStream stream = tcpClient.GetStream();

                stream.WriteAction(ActionCode.RemoveStorage);
                stream.WriteString(storageId);
            }
            catch (SocketException e)
            {
                throw new BackupException("Failed to remove storage from the remote repo", e);
            }
        }

        public byte[] ReadStorage(string storageId)
        {
            try
            {
                using var tcpClient = new TcpClient(Address, Port);
                using NetworkStream stream = tcpClient.GetStream();

                stream.WriteAction(ActionCode.ReadStorage);
                stream.WriteString(storageId);

                return stream.ReadByteArray();
            }
            catch (SocketException e)
            {
                throw new BackupException("Failed get storages list from the remote repo", e);
            }
        }

        public ImmutableArray<string> GetStorages()
        {
            try
            {
                using var tcpClient = new TcpClient(Address, Port);
                using NetworkStream stream = tcpClient.GetStream();

                stream.WriteAction(ActionCode.GetStorages);

                return stream.ReadStringList();
            }
            catch (SocketException e)
            {
                throw new BackupException("Failed get storages list from the remote repo", e);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((RepositoryRemote)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Address, Port);
        }

        protected bool Equals(RepositoryRemote other)
        {
            return Address == other.Address && Port == other.Port;
        }
    }
}