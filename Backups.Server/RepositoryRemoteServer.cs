using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Backups.Entities;

namespace Backups.Server
{
    public class RepositoryRemoteServer
    {
        private int port;
        private RepositoryLocal repositoryLocal;

        public RepositoryRemoteServer(int port, string repoPath)
        {
            if (port < 0 || port > 65535)
                throw new ArgumentException("Invalid port", nameof(port));
            this.port = port;
            repositoryLocal = new RepositoryLocal(repoPath);
        }

        public void Start()
        {
            var tcpListener = new TcpListener(port);

            while (true)
            {
                try
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    var task = Task.Run(() => ServerTcpClient(tcpClient));
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failed to serve connection: ");
                    Console.Error.WriteLine(e.Message);
                }
            }
        }

        public void ServerTcpClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();

            while (true)
            {
                RepositoryRemote.Action action = ReadAction(stream);
                switch (action)
                {
                    case RepositoryRemote.Action.CREATE_STORAGE:
                        int size = ReadInt(stream);
                        byte[] cache = new byte[size];
                        int howMushRead = 0;
                        while (howMushRead < size)
                        {
                            howMushRead += stream.Read(cache, howMushRead, size - howMushRead);
                        }

                        string storageId = repositoryLocal.CreateStorage(cache);
                        WriteString(stream, storageId);
                        break;
                }
            }
        }

        public int ReadInt(NetworkStream stream)
        {
            byte[] bytes = new byte[4];
            stream.Read(bytes, 0, 4);
            return (bytes[0] << 24) | (bytes[1] << 16) | (bytes[2] << 8) | bytes[3];
        }

        public RepositoryRemote.Action ReadAction(NetworkStream stream)
        {
            return (RepositoryRemote.Action) stream.ReadByte();
        }


        public void WriteInt(NetworkStream stream, int num)
        {
            byte[] bytes = {(byte) num, (byte) (num >> 8), (byte) (num >> 16), (byte) (num >> 24)};
            stream.Write(bytes,0, 4);
        }

        public void WriteString(NetworkStream stream, string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            WriteInt(stream, bytes.Length);
            stream.Write(bytes);
        }
    }
}