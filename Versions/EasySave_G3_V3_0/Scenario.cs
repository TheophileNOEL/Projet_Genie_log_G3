using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using EasySave.Core;

namespace EasySave_G3_V1
{
    public class Scenario : INotifyPropertyChanged
    {
        // Propriétés principales
        public int Id { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public BackupType Type { get; set; }
        public BackupState State { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
        public LogEntry Log { get; set; }

        // Accesseurs existants pour compatibilité
        public int GetId() => Id;
        public void SetId(int v) => Id = v;
        public string GetName() => Name;
        public void SetName(string v) => Name = v;
        public string GetSource() => Source;
        public void SetSource(string v) => Source = v;
        public string GetTarget() => Target;
        public void SetTarget(string v) => Target = v;
        public BackupType GetSceanrioType() => Type;
        public void SetType(BackupType v) => Type = v;
        public BackupState GetState() => State;
        public void SetState(BackupState v) => State = v;
        public string GetDescription() => Description;
        public void SetDescription(string v) => Description = v;
        public LogEntry GetLog() => Log;
        public void SetLog(LogEntry v) => Log = v;

        // Nouvelle propriété pour la progression
        private double _progress;
        public double Progress
        {
            get => _progress;
            private set { _progress = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public Scenario()
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
            _progress = 0;
        }

        public Scenario(int id, string name, string source, string target, BackupType type, string description)
            : this()
        {
            Id = id;
            Name = name;
            Source = source;
            Target = target;
            Type = type;
            Description = description;
        }

        /// <summary>
        /// Appelé par l'UI : lance la sauvegarde sur un thread dédié pour ne pas bloquer.
        /// </summary>
        public List<string> Execute()
        {
            var messages = new List<string>();
            try
            {
                State = BackupState.Running;
                messages.Add($"Backup '{Name}' is running...");
                string result = null;

                // Lancement sur thread séparé
                var t = new Thread(() => result = RunSave());
                t.Start();
                t.Join();

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

        private bool IsBusinessSoftwareRunning()
        {
            try
            {
                const string settingsPath = "settings.json";
                if (!File.Exists(settingsPath))
                    return false;

                using var doc = JsonDocument.Parse(File.ReadAllText(settingsPath));
                if (!doc.RootElement.TryGetProperty("CheminsLogiciels", out var arr) ||
                    arr.ValueKind != JsonValueKind.Array)
                    return false;

                foreach (var elem in arr.EnumerateArray())
                {
                    var exe = elem.GetString();
                    if (string.IsNullOrWhiteSpace(exe)) continue;
                    var name = Path.GetFileNameWithoutExtension(exe)!.ToLower();
                    if (Process.GetProcessesByName(name).Any())
                        return true;
                }
            }
            catch
            {
                // ignore
            }
            return false;
        }

        /// <summary>
        /// Exécute la sauvegarde, en traitant d'abord les fichiers prioritaires par réordonnancement,
        /// et met à jour <see cref="Progress"/> à chaque fichier.
        /// </summary>
        private string RunSave()
        {
            try
            {
                var sw = Stopwatch.StartNew();

                if (IsBusinessSoftwareRunning())
                    return "Backup blocked: a business software is currently running.";
                if (!Directory.Exists(Source))
                    return $"Source path '{Source}' not found.";
                if (!Directory.Exists(Target))
                    return $"Target path '{Target}' not found.";

                // Lecture des extensions prioritaires
                var pm = new ParametersManager();
                var prio = pm.Parametres.ExtensionsPrioritaires
                              .Select(e => e.StartsWith(".") ? e.ToLower() : "." + e.ToLower())
                              .ToHashSet();

                // Collecte et réordonnancement
                var all = Directory.GetFiles(Source, "*", SearchOption.AllDirectories).ToList();
                var p = all.Where(f => prio.Contains(Path.GetExtension(f).ToLower())).ToList();
                var np = all.Where(f => !prio.Contains(Path.GetExtension(f).ToLower())).ToList();
                var list = p.Concat(np).ToList();

                // Préparation log & chiffrement
                var folders = new List<Folder>();
                string key = "cle123"; // TODO : extraire de pm.Parametres
                int totalFiles = list.Count;
                int doneCount = 0;

                // Boucle de sauvegarde
                foreach (var f in list)
                {
                    // Création du dossier cible
                    var rel = Path.GetRelativePath(Source, f);
                    var dest = Path.Combine(Target, rel);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);

                    // Enregistrement pour log
                    var info = new FileInfo(f);
                    folders.Add(new Folder(
                        f,
                        File.GetLastWriteTime(f),
                        Path.GetFileName(f),
                        true,
                        info.Length));

                    // Full ou différentiel
                    bool copy = Type switch
                    {
                        BackupType.Full => true,
                        BackupType.Differential =>
                            !File.Exists(dest) ||
                            File.GetLastWriteTimeUtc(f) > File.GetLastWriteTimeUtc(dest),
                        _ => false
                    };

                    if (copy)
                    {
                        File.Copy(f, dest, true);
                        EncryptIfNeeded(dest, key);
                    }

                    // Mise à jour de la ProgressBar
                    doneCount++;
                    Progress = 100.0 * doneCount / totalFiles;
                }

                sw.Stop();

                // Création du log final
                Log = new LogEntry(
                    DateTime.Now,
                    Name,
                    Type,
                    Source,
                    Target,
                    folders.Count,
                    (int)sw.ElapsedMilliseconds,
                    State,
                    Description,
                    folders
                );
                Log.SetDurationMs((int)sw.ElapsedMilliseconds);
                Log.AppendToFile();

                return "done";
            }
            catch (Exception ex)
            {
                return $"Une erreur est survenue pendant la sauvegarde : {ex.Message}";
            }
        }

        /// <summary>Arrête immédiatement la sauvegarde en cours.</summary>
        public string Cancel()
        {
            if (State == BackupState.Running)
            {
                State = BackupState.Cancelled;
                return $"Backup '{Name}' has been cancelled.";
            }
            return $"Cannot cancel backup '{Name}' as it is not currently running.";
        }

        private void EncryptIfNeeded(string targetDirectory, string encryptionKey)
        {
            if (!Directory.Exists(targetDirectory)) return;

            string[] exts = Array.Empty<string>();
            try
            {
                var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                var settingsPath = Path.Combine(exePath, @"..\..\..\settings.json");
                if (File.Exists(settingsPath))
                {
                    using var doc = JsonDocument.Parse(File.ReadAllText(settingsPath));
                    if (doc.RootElement.TryGetProperty("ExtensionsChiffrees", out var arr))
                    {
                        exts = arr.EnumerateArray()
                                  .Where(x => x.ValueKind == JsonValueKind.String)
                                  .Select(x => x.GetString()!)
                                  .ToArray();
                    }
                }
            }
            catch
            {
                // ignore
            }

            foreach (var ext in exts)
            {
                var files = Directory.GetFiles(targetDirectory, $"*{ext}", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    new CryptoSoft.FileManager(file, encryptionKey).TransformFile();
                }
            }
        }
    }
}
