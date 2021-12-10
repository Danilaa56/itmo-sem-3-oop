using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Entities;
using Newtonsoft.Json;

namespace BackupsExtra
{
    public class Context
    {
        private const string ConfigurationFileName = "backups.conf.json";

        private readonly string _configurationFilePath;

        private List<BackupJob> _backupJobs;

        public Context(string path)
        {
            DirectoryInfo workDirectory = Directory.CreateDirectory(path);
            _configurationFilePath = Path.Combine(workDirectory.FullName, ConfigurationFileName);
            Load();
        }

        public void AddBackupJob(BackupJob backupJob)
        {
            _backupJobs.Add(backupJob ?? throw new ArgumentNullException(nameof(backupJob)));
        }

        public List<BackupJob> BackupJobs()
        {
            return _backupJobs.ToList();
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(_backupJobs, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                PreserveReferencesHandling = PreserveReferencesHandling.All,
            });
            File.WriteAllText(_configurationFilePath, json);
        }

        private void Load()
        {
            if (File.Exists(_configurationFilePath))
            {
                string jsonConfiguration = File.ReadAllText(_configurationFilePath);
                _backupJobs = JsonConvert.DeserializeObject<List<BackupJob>>(
                    jsonConfiguration,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        PreserveReferencesHandling = PreserveReferencesHandling.All,
                    });
            }
            else
            {
                _backupJobs = new List<BackupJob>();
            }
        }
    }
}