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
            if (tcpClient.Client.RemoteEndPoint is null)
                throw new ArgumentException("Tcp client must have remote address", nameof(tcpClient));
            _logger = new Logger(tcpClient.Client.RemoteEndPoint.ToString());
            _stream = tcpClient.GetStream();
        }

        public void Handle()
        {
            while (_continueServing)
            {
                _logger.Info("reading action code");
                RepositoryRemote.ActionCode actionCode = StreamUtils.ReadAction(_stream);

                switch (actionCode)
                {
                    case RepositoryRemote.ActionCode.CreateStorage:
                        _createStorage();
                        break;
                    case RepositoryRemote.ActionCode.GetStorages:
                        _getStorages();
                        break;
                    default:
                        _unknownActionCode();
                        break;
                }
            }
        }

        private void _createStorage()
        {
            _logger.Info("create storage, reading data");
            byte[] data = StreamUtils.ReadByteArray(_stream);
            string storageId = _repo.CreateStorage(data);
            _logger.Info($"storage {storageId} was created, sending");
            StreamUtils.WriteString(_stream, storageId);
            _logger.Info($"storage id was sent, closing connection");
            _continueServing = false;
        }

        private void _getStorages()
        {
            _logger.Info("get storages, sending data");
            ImmutableArray<string> storageIds = _repo.GetStorages();
            StreamUtils.WriteStringList(_stream, storageIds);
            _logger.Info("storage ids was sent, closing connection");
            _continueServing = false;
        }

        private void _unknownActionCode()
        {
            _logger.Error("unsupported action code, aborting connection");
            _continueServing = false;
        }
    }
}