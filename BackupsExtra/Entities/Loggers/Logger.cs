using System;
using System.IO;
using System.Threading;

namespace BackupsExtra.Entities.Loggers
{
    public class Logger
    {
        // 2021-11-09 00:00:02.298000 [serializer] INFO  r.y.i.s.yp.ObjectLifeCycleManagerImpl - multi_cluster_replica_set - update id 'testpalm-pr-1609.sqs' finished successfully
        private string _dateFormat = "yyyy-MM-dd HH:mm:ss:ffffff";
        private TextWriter _writer = Console.Out;

        public Logger(string label)
        {
            Label = label ?? throw new ArgumentNullException(nameof(label));
        }

        public string Label { get; private init; }

        public string DateFormat
        {
            get => _dateFormat;
            set => _dateFormat = value ?? throw new ArgumentNullException();
        }

        public bool ShouldWriteDate { get; set; } = true;
        public bool ShouldWriteThreadName { get; set; } = true;

        public TextWriter TextWriter
        {
            get => _writer;
            set => _writer = value ?? throw new ArgumentNullException();
        }

        public void Info(string msg)
        {
            Log(msg, "INFO");
        }

        public void Warn(string msg)
        {
            Log(msg, "WARN");
        }

        public void Error(string msg)
        {
            Log(msg, "ERROR");
        }

        public void Flush()
        {
            _writer.Flush();
        }

        public void Close()
        {
            _writer.Close();
        }

        private void Log(string msg, string mode)
        {
            string dateStr = string.Empty;
            if (ShouldWriteDate)
                dateStr = $"{DateTime.Now.ToString(_dateFormat)} ";
            string threadStr = string.Empty;
            if (ShouldWriteThreadName)
                threadStr = $"[{Thread.CurrentThread.Name}] ";

            WriteLine($"{dateStr}{threadStr}{mode} - {msg}");
        }

        private void WriteLine(string line)
        {
            _writer.WriteLine(line);
        }
    }
}