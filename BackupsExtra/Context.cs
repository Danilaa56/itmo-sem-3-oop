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

        private readonly string _workDirectoryPath;

        private List<BackupJob> _backupJobs;

        public Context(string path)
        {
            DirectoryInfo workDirectory = Directory.CreateDirectory(path);
            _workDirectoryPath = workDirectory + Path.DirectorySeparatorChar.ToString();
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
            });
            File.WriteAllText(_workDirectoryPath + ConfigurationFileName, json);
        }

        private void Load()
        {
            if (File.Exists(_workDirectoryPath + ConfigurationFileName))
            {
                string jsonConfiguration = File.ReadAllText(_workDirectoryPath + ConfigurationFileName);
                _backupJobs = JsonConvert.DeserializeObject<List<BackupJob>>(jsonConfiguration,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                    });
            }
            else
            {
                _backupJobs = new List<BackupJob>();
            }
        }
    }
}