using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace EasySave.Core
{
    public class LogEntry
    {
        private DateTime timestamp;
        private List<Folder> listFolder;
        private string jobName;
        private BackupType backupType;
        private string sourceUNC;
        private string targetUNC;
        private long fileSizeBytes;
        private long durationMs;
        private BackupState state;
        private string description;

        private static readonly object fileLock = new object();

        public LogEntry()
        {
            timestamp = DateTime.Now;
            listFolder = new List<Folder>();
            jobName = string.Empty;
            backupType = BackupType.Full;
            sourceUNC = string.Empty;
            targetUNC = string.Empty;
            fileSizeBytes = 0;
            durationMs = 0;
            state = BackupState.Pending;
            description = string.Empty;
        }

        public LogEntry(DateTime timestamp,
                        string jobName,
                        BackupType backupType,
                        string sourceUNC,
                        string targetUNC,
                        long fileSizeBytes,
                        long durationMs,
                        BackupState state,
                        string description,
                        List<Folder> listFolder)
        {
            this.timestamp = timestamp;
            this.jobName = jobName;
            this.backupType = backupType;
            this.sourceUNC = sourceUNC;
            this.targetUNC = targetUNC;
            this.durationMs = durationMs;
            this.state = state;
            this.description = description;
            this.listFolder = listFolder ?? new List<Folder>();
            this.fileSizeBytes = fileSizeBytes > 0
                                  ? fileSizeBytes
                                  : CalculateTotalSize(this.listFolder);
        }

        public DateTime GetTimestamp() => timestamp;
        public string GetJobName() => jobName;
        public BackupType GetBackupType() => backupType;
        public string GetSourceUNC() => sourceUNC;
        public string GetTargetUNC() => targetUNC;
        public long GetFileSizeBytes() => fileSizeBytes;
        public long GetDurationMs() => durationMs;
        public BackupState GetState() => state;
        public string Getdescription() => description;
        public List<Folder> GetListFolder() => listFolder;

        public void SetTimestamp(DateTime v) { timestamp = v; }
        public void SetJobName(string v) { jobName = v; }
        public void SetBackupType(BackupType v) { backupType = v; }
        public void SetSourceUNC(string v) { sourceUNC = v; }
        public void SetTargetUNC(string v) { targetUNC = v; }
        public void SetDurationMs(long v) { durationMs = v; }
        public void SetState(BackupState v) { state = v; }
        public void Setdescription(string v) { description = v; }
        public void SetListFolder(List<Folder> v) { listFolder = v; }

        public void AddFolder(Folder f) { listFolder.Add(f); }
        public void RemoveFolder(Folder f) { listFolder.Remove(f); }

        private long CalculateTotalSize(List<Folder> lf)
        {
            long sum = 0;
            foreach (var f in lf) sum += f.GetSize();
            return sum;
        }
        public long TotalSize() => fileSizeBytes;

        public string Display()
        {
            var s = $"Timestamp          : {timestamp}\n"
                   + $"Job Name           : {jobName}\n"
                   + $"Backup Type        : {backupType}\n"
                   + $"Source UNC         : {sourceUNC}\n"
                   + $"Target UNC         : {targetUNC}\n"
                   + $"Duration (ms)      : {durationMs}\n"
                   + $"State              : {state}\n"
                   + $"description        : {description}\n"
                   + $"Total Size (Bytes) : {fileSizeBytes}\n"
                   + $"Nb Items           : {listFolder.Count}\n";
            foreach (var f in listFolder)
            {
                string type = f.GetIsFile() ? "file" : "folder";
                s += $"  - {f.GetPath()} ({f.GetSize()} o) [{type}]\n";
            }
            return s;
        }

        public string ToJson(bool indent = false)
        {
            var anon = new
            {
                timestamp = timestamp.ToString("o"),
                jobName,
                backupType = backupType.ToString(),
                sourceUNC,
                targetUNC,
                fileSizeBytes,
                durationMs,
                state,
                description,
                totalSize = fileSizeBytes,
                listFolder = listFolder.ConvertAll(f => new
                {
                    path = f.GetPath(),
                    size = f.GetSize(),
                    type = f.GetIsFile() ? "file" : "folder"
                })
            };
            var opts = new JsonSerializerOptions { WriteIndented = indent };
            return JsonSerializer.Serialize(anon, opts) + Environment.NewLine;
        }
        public string ToXml(bool indent = false)
        {
            XElement entry = new XElement("logEntry",
                new XElement("timestamp", timestamp.ToString("o")),
                new XElement("jobName", jobName),
                new XElement("backupType", backupType),
                new XElement("sourceUNC", sourceUNC),
                new XElement("targetUNC", targetUNC),
                new XElement("fileSizeBytes", fileSizeBytes),
                new XElement("durationMs", durationMs),
                new XElement("state", state),
                new XElement("description", description),
                new XElement("listFolder",
                    listFolder.ConvertAll(f => new XElement("item",
                        new XAttribute("path", f.GetPath()),
                        new XAttribute("size", f.GetSize()),
                        new XAttribute("type", f.GetIsFile() ? "file" : "folder")
                    ))
                )
            );

            return indent
                   ? entry.ToString(SaveOptions.None) + Environment.NewLine
                   : entry.ToString(SaveOptions.DisableFormatting) + Environment.NewLine;
        }

        public void AppendToFile(LogFormat format = LogFormat.Json, bool indent = false)
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string logsFolder = Path.GetFullPath(Path.Combine(exePath, @"..\..\..\Logs"));

            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);

            string datePart = DateTime.Now.ToString("yyyy-MM-dd");
            string logFileName = format == LogFormat.Json
                                 ? $"log-{datePart}.json"
                                 : $"log-{datePart}.xml";

            string finalPath = Path.Combine(logsFolder, logFileName);

            string content = format == LogFormat.Json
                             ? ToJson(indent)
                             : ToXml(indent);

            lock (fileLock)
            {
                File.AppendAllText(finalPath, content, Encoding.UTF8);
            }
        }
    }
}