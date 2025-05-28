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
        }

        public LogEntry(
            DateTime timestamp,
            string jobName,
            BackupType backupType,
            string sourceUNC,
            string targetUNC,
            long fileSizeBytes,
            long durationMs,
            BackupState state,
            List<Folder> listFolder)
        {
            this.timestamp = timestamp;
            this.jobName = jobName;
            this.backupType = backupType;
            this.sourceUNC = sourceUNC;
            this.targetUNC = targetUNC;
            this.durationMs = durationMs;
            this.state = state;
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
        public List<Folder> GetListFolder() => listFolder;

        public void SetTimestamp(DateTime v) => timestamp = v;
        public void SetJobName(string v) => jobName = v;
        public void SetBackupType(BackupType v) => backupType = v;
        public void SetSourceUNC(string v) => sourceUNC = v;
        public void SetTargetUNC(string v) => targetUNC = v;
        public void SetDurationMs(long v) => durationMs = v;
        public void SetState(BackupState v) => state = v;
        public void SetListFolder(List<Folder> v) => listFolder = v;

        public void AddFolder(Folder f) => listFolder.Add(f);
        public void RemoveFolder(Folder f) => listFolder.Remove(f);

        private long CalculateTotalSize(List<Folder> lf)
        {
            long sum = 0;
            foreach (var f in lf)
                sum += f.GetSize();
            return sum;
        }

        public long TotalSize() => fileSizeBytes;

        /// <summary>
        /// Affichage console sans "Description".
        /// </summary>
        public string Display()
        {
            var s = $"Timestamp          : {timestamp}\n"
                   + $"Job Name           : {jobName}\n"
                   + $"Backup Type        : {backupType}\n"
                   + $"Source UNC         : {sourceUNC}\n"
                   + $"Target UNC         : {targetUNC}\n"
                   + $"Duration (ms)      : {durationMs}\n"
                   + $"State              : {state}\n"
                   + $"Total Size (Bytes) : {fileSizeBytes}\n"
                   + $"Nb Items           : {listFolder.Count}\n";
            foreach (var f in listFolder)
            {
                string type = f.GetIsFile() ? "file" : "folder";
                s += $"  - {f.GetPath()} ({f.GetSize()} o) [{type}]\n";
            }
            return s;
        }

        /// <summary>
        /// Sérialisation JSON sans "Description" ni "totalSize".
        /// </summary>
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
                listFolder = listFolder.ConvertAll(f => new
                {
                    path = f.GetPath(),
                    size = f.GetSize(),
                    type = f.GetIsFile() ? "file" : "folder",
                    encryptionTimeMs = f.GetEncryptionTimeMs()
                })
            };
            var opts = new JsonSerializerOptions { WriteIndented = indent };
            return JsonSerializer.Serialize(anon, opts) + Environment.NewLine;
        }

        /// <summary>
        /// Sérialisation XML sans "Description".
        /// </summary>
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
                new XElement("listFolder",
                    listFolder.ConvertAll(f => new XElement("item",
                        new XAttribute("path", f.GetPath()),
                        new XAttribute("size", f.GetSize()),
                        new XAttribute("type", f.GetIsFile() ? "file" : "folder"),
                        new XAttribute("encryptionTimeMs", f.GetEncryptionTimeMs())
                    ))
                )
            );

            return indent
                   ? entry.ToString(SaveOptions.None) + Environment.NewLine
                   : entry.ToString(SaveOptions.DisableFormatting) + Environment.NewLine;
        }

        /// <summary>
        /// Écrit l'entrée de log dans un fichier JSON ou XML selon FormatLog.
        /// </summary>
        public void AppendToFile(LogFormat format = LogFormat.Json)
        {
            // 1) Préparer la représentation JSON indentée de cette entrée
            //    ToJson(true) produit un objet multi-lignes, sans saut de ligne final
            string rawEntry = ToJson(indent: true).TrimEnd();
            //    On ajoute deux espaces devant chaque ligne pour l'indenter dans le tableau
            var indentedEntry = rawEntry
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Select(line => "  " + line)
                .Aggregate((a, b) => a + Environment.NewLine + b);

            // 2) Préparer le chemin du fichier
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string logsFolder = Path.GetFullPath(Path.Combine(exePath, @"..\..\..\Logs"));
            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);

            string datePart = DateTime.Now.ToString("yyyy-MM-dd");
            string fileName = format == LogFormat.Json
                ? $"log-{datePart}.json"
                : $"log-{datePart}.xml";
            string finalPath = Path.Combine(logsFolder, fileName);

            lock (fileLock)
            {
                if (format == LogFormat.Json)
                {
                    if (!File.Exists(finalPath) || new FileInfo(finalPath).Length == 0)
                    {
                        // Premier objet : on crée [ <entry> ]
                        var first = "[\n" + indentedEntry + "\n]";
                        File.WriteAllText(finalPath, first, Encoding.UTF8);
                    }
                    else
                    {
                        // On lit tout, on retire la ] finale, on ajoute ,\n<entry>\n]
                        string existing = File.ReadAllText(finalPath, Encoding.UTF8);
                        int idx = existing.LastIndexOf(']');
                        if (idx >= 0)
                        {
                            string head = existing.Substring(0, idx).TrimEnd();
                            string updated =
                                head
                                + ",\n"
                                + indentedEntry
                                + "\n]";
                            File.WriteAllText(finalPath, updated, Encoding.UTF8);
                        }
                        else
                        {
                            // Si mal formé, on réinitialise
                            var fresh = "[\n" + indentedEntry + "\n]";
                            File.WriteAllText(finalPath, fresh, Encoding.UTF8);
                        }
                    }
                }
                else
                {
                    // XML reste inchangé
                    string xml = ToXml(indent: true) + Environment.NewLine;
                    File.AppendAllText(finalPath, xml, Encoding.UTF8);
                }
            }
        }
    }

}
