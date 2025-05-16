using System;
using System.Collections.Generic;
using System.IO;
using EasySave.Core;
namespace EasySave_G3_V1
{
    public class Scenario
    {
        // Properties of the backup scenario
        public int Id { get; set; }             // Unique identifier for the scenario
        public string Name { get; set; }        // Name of the scenario (Description or title)
        public string Source { get; set; }      // Source directory or file to backup
        public string Target { get; set; }      // Target directory or location to store the backup
        public BackupType Type { get; set; }    // Type of backup (Full or Differential)
        public BackupState State { get; set; }  // Current state of the backup (Pending, Running, etc.)
        public string Description { get; set; } // Additional Description about the backup scenario
        public LogEntry Log { get; set; }       // Log entry associated with the backup scenario

        public int GetId() => Id;
        public void SetId(int value) => Id = value;

        public string GetName() => Name;
        public void SetName(string value) => Name = value;

        public string GetSource() => Source;
        public void SetSource(string value) => Source = value;

        public string GetTarget() => Target;
        public void SetTarget(string value) => Target = value;

        public BackupType GetSceanrioType() => Type;
        public void SetType(BackupType value) => Type = value;

        public BackupState GetState() => State;
        public void SetState(BackupState value) => State = value;

        public string GetDescription() => Description;
        public void SetDescription(string value) => Description = value;

        public LogEntry GetLog() => Log;
        public void SetLog(LogEntry value) => Log = value;

        public Scenario()
        {
            Id = -1;
            Name = string.Empty;
            Source = string.Empty;
            Target = string.Empty;
            Type = BackupType.Full;
            State = BackupState.Pending;
            Description = string.Empty;
            Log = new LogEntry();
        }

        public Scenario(int id, string name, string source, string target, BackupType type, string description)
        {
            Id = id;
            Name = name;
            Source = source;
            Target = target;
            Type = type;
            State = BackupState.Pending;
            Description = description;
            Log = new LogEntry();
        }

        public List<string> Execute()
        {
            List<string> messages = new List<string>();

            try
            {
                State = BackupState.Running;
                messages.Add($"Backup '{Name}' is running...");

                string result = RunSave();
                if (!string.IsNullOrWhiteSpace(result))
                    messages.Add(result);

                State = BackupState.Completed;
                messages.Add($"Backup '{Name}' completed successfully.");
            }
            catch (Exception ex)
            {
                State = BackupState.Failed;
                messages.Add($"Error during backup '{Name}': {ex.Message}");
            }

            return messages;
        }

        private string RunSave()
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                if (!Directory.Exists(Source))
                    throw new DirectoryNotFoundException($"Source path '{Source}' not found.");

                if (!Directory.Exists(Target))
                    throw new DirectoryNotFoundException($"Target path '{Target}' not found.");

                List<Folder> folders = new List<Folder>();

                foreach (string filePath in Directory.GetFiles(Source, "*", SearchOption.AllDirectories))
                {
                    string relativePath = Path.GetRelativePath(Source, filePath);
                    string targetPath = Path.Combine(Target, relativePath);

                    FileInfo fileInfo = new FileInfo(filePath);

                    folders.Add(new Folder(filePath, File.GetLastWriteTime(filePath), Path.GetFileName(filePath), true, fileInfo.Length));

                    bool shouldCopy = Type switch
                    {
                        BackupType.Full => true,
                        BackupType.Differential => !File.Exists(targetPath) ||
                                                   File.GetLastWriteTimeUtc(filePath) > File.GetLastWriteTimeUtc(targetPath),
                        _ => false
                    };

                    if (shouldCopy)
                    {
                        try
                        {
                            //EncryptIfNeeded(targetPath, ".txt", "cle123");
                            File.Copy(filePath, targetPath, true);
                        }
                        catch (Exception copyEx)
                        {
                            return $"Erreur lors de la copie du fichier : {filePath} - {copyEx.Message}";
                        }
                    }
                }

                stopwatch.Stop();

                Log = new LogEntry(
                    DateTime.Now,
                    Name,
                    Type,
                    Source,
                    Target,
                    1,
                    (int)stopwatch.ElapsedMilliseconds,
                    State,
                    Description,
                    folders
                );

                Log.SetDurationMs((int)stopwatch.ElapsedMilliseconds);
                Log.AppendToFile();
                return null;
            }
            catch (Exception ex)
            {
                return $"Une erreur est survenue pendant la sauvegarde : {ex.Message}";
            }
        }

        public string Cancel()
        {
            if (State == BackupState.Running)
            {
                State = BackupState.Cancelled;
                return $"Backup '{Name}' has been cancelled.";
            }
            else
            {
                return $"Cannot cancel backup '{Name}' as it is not currently running.";
            }
        }

        //private void EncryptIfNeeded(string targetDirectory, string extension, string encryptionKey)
        //{
        //    if (!Directory.Exists(targetDirectory)) return;

        //    var filesToEncrypt = Directory.GetFiles(targetDirectory, $"*{extension}", SearchOption.AllDirectories);

        //    foreach (var file in filesToEncrypt)
        //    {
        //        try
        //        {
        //            var encryptor = new CryptoSoft.FileManager(file, encryptionKey);
        //            encryptor.TransformFile();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Erreur lors du chiffrement du fichier {file} : {ex.Message}");
        //        }
        //    }
        //}

    }
}
