using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
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
        public bool IsSelected { get; set; }
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

        public Scenario() // default values for scenario
        {
            Id = -1;
            Name = string.Empty;
            Source = string.Empty;
            Target = string.Empty;
            Type = BackupType.Full;
            State = BackupState.Pending;
            Description = string.Empty;
            IsSelected = false;
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
            IsSelected = false;
            Log = new LogEntry();
        }

        public List<string> Execute() 
        {
            List<string> messages = new List<string>();

            try // try to run the save
            {
                State = BackupState.Running;
                messages.Add($"Backup '{Name}' is running...");

                string result = RunSave();
                if (!string.IsNullOrWhiteSpace(result))
                    messages.Add(result);

                State = BackupState.Completed;
                messages.Add($"Backup '{Name}' completed successfully.");
            }
            catch (Exception ex) // return the error
            {
                State = BackupState.Failed;
                messages.Add($"Error during backup '{Name}': {ex.Message}");
            }

            return messages;
        }

        private bool IsBusinessSoftwareRunning() // check for the business software
            {
                try
                {
                    string settingsPath = "settings.json";
                    if (!File.Exists(settingsPath))
                        return false; 

                    string json = File.ReadAllText(settingsPath);
                    using JsonDocument doc = JsonDocument.Parse(json);
                    JsonElement root = doc.RootElement;

                    if (!root.TryGetProperty("CheminsLogiciels", out JsonElement logicielsArray) || logicielsArray.ValueKind != JsonValueKind.Array)
                        return false;

                    foreach (JsonElement logicielElement in logicielsArray.EnumerateArray())
                    {
                        string exePath = logicielElement.GetString();
                        if (string.IsNullOrWhiteSpace(exePath))
                            continue;

                        string exeName = Path.GetFileNameWithoutExtension(exePath)?.ToLower();
                        if (string.IsNullOrEmpty(exeName))
                            continue;

                        var runningProcesses = Process.GetProcessesByName(exeName);
                        if (runningProcesses.Length > 0)
                        {
                            Console.WriteLine($"Logiciel métier détecté : {exeName}");
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la lecture de settings.json : {ex.Message}");
                }

                return false;
            }



    private string RunSave()  
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                if (IsBusinessSoftwareRunning())
                    return "Backup blocked: a business software is currently running."; // cancel save if a business software is running

                if (!Directory.Exists(Source))
                    return $"Source path '{Source}' not found.";
                if (!Directory.Exists(Target))
                    return $"Target path '{Target}' not found.";

                List<Folder> folders = new List<Folder>();
                string encryptionKey = "cle123"; // Encryption key (might add it in settings later)

                foreach (string filePath in Directory.GetFiles(Source, "*", SearchOption.AllDirectories))
                {
                    string relativePath = Path.GetRelativePath(Source, filePath);
                    string targetPath = Path.Combine(Target, relativePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);   

                    FileInfo fileInfo = new FileInfo(filePath);

                    folders.Add(new Folder(filePath, File.GetLastWriteTime(filePath), Path.GetFileName(filePath), true, fileInfo.Length));

                    bool shouldCopy = Type switch
                    {
                        BackupType.Full => true,
                        BackupType.Differential => !File.Exists(targetPath) ||
                                                   File.GetLastWriteTimeUtc(filePath) > File.GetLastWriteTimeUtc(targetPath), 
                        _ => false // check if the save is needed for each file
                    };

                    if (shouldCopy)
                    {
                        try
                        {
                            File.Copy(filePath, targetPath, true);
                            EncryptIfNeeded(targetPath, encryptionKey); // XOR encryption
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
                    folders.Count,
                    (int)stopwatch.ElapsedMilliseconds,
                    State,
                    Description,
                    folders
                );

                Log.SetDurationMs((int)stopwatch.ElapsedMilliseconds);
                Log.AppendToFile();

                return "done";
            }
            catch (Exception ex)
            {
                return $"Une erreur est survenue pendant la sauvegarde : {ex.Message}";
            }
        }


        public string Cancel()  // needs to be implemented
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

        private void EncryptIfNeeded(string targetDirectory, string encryptionKey)  // Encryption with Cryptosoft
        {
            if (!Directory.Exists(targetDirectory)) return;

            string[] extensionsToEncrypt = Array.Empty<string>();

            try
            {
                string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string settingsPath = Path.Combine(exePath, @"..\..\..\settings.json");

                if (File.Exists(settingsPath))
                {
                    string jsonSettings = File.ReadAllText(settingsPath);
                    using JsonDocument doc = JsonDocument.Parse(jsonSettings);
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("ExtensionsChiffrees", out JsonElement extElem) && extElem.ValueKind == JsonValueKind.Array)
                    {
                        List<string> extensions = new List<string>();
                        foreach (var item in extElem.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.String)
                                extensions.Add(item.GetString());
                        }
                        extensionsToEncrypt = extensions.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }

            foreach (var extension in extensionsToEncrypt)
            {
                var filesToEncrypt = Directory.GetFiles(targetDirectory, $"*{extension}", SearchOption.AllDirectories);

                foreach (var file in filesToEncrypt)
                {
                    try
                    {
                        var encryptor = new CryptoSoft.FileManager(file, encryptionKey);
                        encryptor.TransformFile();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Erreur lors du chiffrement" + ex.Message);
                    }
                }
            }
        }
    }


}

