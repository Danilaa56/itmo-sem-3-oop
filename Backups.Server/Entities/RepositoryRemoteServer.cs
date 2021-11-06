using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Backups.Entities.Repository;
using Backups.Server.Tools;
using Backups.Tools;

namespace Backups.Server.Entities
{
    public class RepositoryRemoteServer
    {
        private readonly int _port;
        private readonly RepositoryLocal _repositoryLocal;
        private Task _serverTask;

        public RepositoryRemoteServer(int port, string repoPath)
        {
            if (port is < 0 or > 65535)
                throw new ArgumentException("Invalid port", nameof(port));
            _port = port;
            _repositoryLocal = new RepositoryLocal(repoPath);
        }

        public void Start()
        {
            if (_serverTask is not null)
                throw new BackupException("Server has already been started");
            _serverTask = Task.Run(Run);
        }

        public void Wait()
        {
            _serverTask.Wait();
        }

        private void Run()
        {
            var logger = new Logger("Main loop");
            var tcpListener = new TcpListener(IPAddress.Loopback, _port);
            tcpListener.Start();

            while (true)
            {
                try
                {
                    logger.Info("waiting for a client");
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    logger.Info($"{tcpClient.Client.RemoteEndPoint} connected");
                    var clientHandler = new ClientHandler(_repositoryLocal, tcpClient);
                    Task.Run(() => clientHandler.Handle());
                }
                catch (Exception e)
                {
                    logger.Error("Failed to accept connection: ");
                    logger.Error(e.Message);
                }
            }
        }
    }
}