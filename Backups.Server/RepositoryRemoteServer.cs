using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Backups.Entities;
using Backups.Tools;

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
            tcpListener.Start();

            while (true)
            {
                try
                {
                    Console.WriteLine("[Main loop]: waiting for a client");
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    Console.WriteLine($"[Main loop]: {tcpClient.Client.RemoteEndPoint} connected");
                    var task = Task.Run(() => ServerTcpClient(tcpClient));
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Failed to serve connection: ");
                    Console.Error.WriteLine(e.Message);
                }
            }
        }

        private void ServerTcpClient(TcpClient tcpClient)
        {
            string prefix = $"[{tcpClient.Client.RemoteEndPoint}]: ";
            NetworkStream stream = tcpClient.GetStream();

            bool continueServing = true;

            while (continueServing)
            {
                Console.WriteLine(prefix + "reading action code");
                RepositoryRemote.Action action = StreamUtils.ReadAction(stream);
                switch (action)
                {
                    case RepositoryRemote.Action.CreateStorage:
                        Console.WriteLine(prefix + "action code: create storage; reading data");
                        byte[] data = StreamUtils.ReadByteArray(stream);
                        string storageId = repositoryLocal.CreateStorage(data);
                        Console.WriteLine( $"{prefix}storage {storageId} was created, sending");
                        StreamUtils.WriteString(stream, storageId);
                        Console.WriteLine( $"{prefix}storage id is sent, closing connection");
                        continueServing = false;
                        break;
                    default:
                        Console.WriteLine(prefix + "unsupported action code, aborting connection");
                        continueServing = false;
                        break;
                }
            }

            stream.Close();
            tcpClient.Close();
        }
    }
}