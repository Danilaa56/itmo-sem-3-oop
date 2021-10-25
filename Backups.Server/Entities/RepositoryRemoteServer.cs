using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Backups.Entities;
using Backups.Server.Entities.Actions;
using Backups.Tools;

namespace Backups.Server.Entities
{
    public class RepositoryRemoteServer
    {
        private readonly int _port;
        private readonly RepositoryLocal _repositoryLocal;
        private Task _serverTask;

        private readonly Dictionary<RepositoryRemote.ActionCode, IAction> _actions =
            new Dictionary<RepositoryRemote.ActionCode, IAction>();

        public RepositoryRemoteServer(int port, string repoPath)
        {
            if (port is < 0 or > 65535)
                throw new ArgumentException("Invalid port", nameof(port));
            _port = port;
            _repositoryLocal = new RepositoryLocal(repoPath);

            _actions[RepositoryRemote.ActionCode.CreateStorage] = new ActionCreateStorage();
            _actions[RepositoryRemote.ActionCode.GetStorages] = new ActionGetStorages();
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
            var tcpListener = new TcpListener(IPAddress.Loopback, _port);
            tcpListener.Start();

            while (true)
            {
                try
                {
                    Console.WriteLine("[Main loop]: waiting for a client");
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    Console.WriteLine($"[Main loop]: {tcpClient.Client.RemoteEndPoint} connected");
                    Task.Run(() => ServerTcpClient(tcpClient));
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
                RepositoryRemote.ActionCode actionCodeCode = StreamUtils.ReadAction(stream);

                if (_actions.TryGetValue(actionCodeCode, out IAction action))
                {
                    continueServing = action.Proc(prefix, stream, _repositoryLocal);
                }
                else
                {
                    Console.WriteLine(prefix + "unsupported action code, aborting connection");
                    continueServing = false;
                }
            }

            stream.Close();
            tcpClient.Close();
        }
    }
}