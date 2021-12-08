using System;
using System.Collections.Immutable;
using System.Net.Sockets;
using Backups.Entities.Repository;
using Backups.Server.Tools;
using Backups.Tools;

namespace Backups.Server.Entities
{
    public class ClientHandler
    {
        private readonly IRepository _repo;
        private readonly Logger _logger;
        private readonly NetworkStream _stream;
        private bool _continueServing = true;

        public ClientHandler(RepositoryLocal repo, TcpClient tcpClient)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            if (tcpClient is null)
                throw new ArgumentNullException(nameof(tcpClient));
            if (tcpClient.Client is null)
                throw new ArgumentException("Tcp client must have non-null Client property", nameof(tcpClient));
            if (tcpClient.Client.RemoteEndPoint is null)
                throw new ArgumentException("Tcp client must have remote address", nameof(tcpClient));
            _logger = new Logger(tcpClient.Client.RemoteEndPoint.ToString());
            _stream = tcpClient.GetStream();
        }

        public void Handle()
        {
            try
            {
                while (_continueServing)
                {
                    _logger.Info("reading action code");
                    RepositoryRemote.ActionCode actionCode = _stream.ReadAction();

                    switch (actionCode)
                    {
                        case RepositoryRemote.ActionCode.CreateStorage:
                            CreateStorage();
                            break;
                        case RepositoryRemote.ActionCode.GetStorages:
                            GetStorages();
                            break;
                        case RepositoryRemote.ActionCode.RemoveStorage:
                            RemoveStorage();
                            break;
                        case RepositoryRemote.ActionCode.ReadStorage:
                            ReadStorage();
                            break;
                        default:
                            UnknownActionCode();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("An error acquired while handling the connection");
                _logger.Error(e.Message);
            }
        }

        private void CreateStorage()
        {
            _logger.Info("create storage, reading data");
            byte[] data = _stream.ReadByteArray();
            string storageId = _repo.CreateStorage(data);
            _logger.Info($"storage {storageId} was created, sending");
            _stream.WriteString(storageId);
            _logger.Info($"storage id was sent, closing connection");
            _continueServing = false;
        }

        private void GetStorages()
        {
            _logger.Info("get storages, sending data");
            ImmutableArray<string> storageIds = _repo.GetStorages();
            _stream.WriteStringList(storageIds);
            _logger.Info("storage ids was sent, closing connection");
            _continueServing = false;
        }

        private void RemoveStorage()
        {
            _logger.Info("remove storage, reading storage id");
            string storageId = _stream.ReadString();
            _logger.Info($"storage id: {storageId}");
            _repo.RemoveStorage(storageId);
            _logger.Info("storage was removed, closing connection");
            _continueServing = false;
        }

        private void ReadStorage()
        {
            _logger.Info("read storage, reading storage id");
            string storageId = _stream.ReadString();
            _logger.Info($"storage id: {storageId}");
            byte[] data = _repo.ReadStorage(storageId);
            _stream.WriteByteArray(data);
            _logger.Info("storage was sent, closing connection");
            _continueServing = false;
        }

        private void UnknownActionCode()
        {
            _logger.Error("unsupported action code, aborting connection");
            _continueServing = false;
        }
    }
}