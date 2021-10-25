using System;
using System.Net.Sockets;
using Backups.Tools;

namespace Backups.Entities
{
    public class RepositoryRemote : Repository
    {
        public RepositoryRemote(string address, int port)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            if (port is < 0 or > 65535)
                throw new ArgumentException("Invalid port", nameof(address));
            Port = port;
        }

        public enum Action : byte
        {
            CreateStorage = 0,
        }

        public string Address { get; }
        public int Port { get; }

        public override string CreateStorage(byte[] data)
        {
            try
            {
                var tcpClient = new TcpClient(Address, Port);
                NetworkStream stream = tcpClient.GetStream();

                StreamUtils.WriteAction(stream, Action.CreateStorage);
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
    }
}