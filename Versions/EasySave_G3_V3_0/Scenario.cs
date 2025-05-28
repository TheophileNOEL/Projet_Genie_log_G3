using EasySave.Core;
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
using System.Threading.Tasks;
using System.Windows;

namespace EasySave_G3_V1
{
    public class Scenario : INotifyPropertyChanged
    {
        // Permet de faire en sorte que CryptoSoft ne s'exécute qu'une seule fois à la fois,
        // même si plusieurs threads ou processus appellent EncryptIfNeeded.
        private static readonly Mutex _cryptoMutex =
            new Mutex(initiallyOwned: false,
                      name: @"Global\CryptoSoft_Singleton");

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

        // Progress bar
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

        /// <summary>Lance le RunSave() sur un thread.</summary>
        public List<string> Execute()
        {
            var messages = new List<string>();
            try
            {
                State = BackupState.Running;
                messages.Add($"Backup '{Name}' is running...");
                string result = null;

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
            catch { /* ignore */ }
            return false;
        }

        /// <summary>Core de la sauvegarde + priorité + mesure de chiffrement</summary>
        private string RunSave()
        {
            try
            {
                // Chronomètre global
                var swTotal = Stopwatch.StartNew();

                // 1) Vérifications préalables
                if (IsBusinessSoftwareRunning())
                    return "Backup blocked: a business software is currently running.";
                if (!Directory.Exists(Source))
                    return $"Source path '{Source}' not found.";
                if (!Directory.Exists(Target))
                    return $"Target path '{Target}' not found.";

                // 2) Chargement des paramètres
                var pm = new ParametersManager();

                // 2a) Extensions à chiffrer
                var toEncrypt = new HashSet<string>(
                    pm.Parametres.ExtensionsChiffrees
                      .Select(e => e.StartsWith(".") ? e.ToLower() : "." + e.ToLower())
                );

                // 2b) Extensions prioritaires
                var prioExt = new HashSet<string>(
                    pm.Parametres.ExtensionsPrioritaires
                      .Select(e => e.StartsWith(".") ? e.ToLower() : "." + e.ToLower())
                );

                // 3) Collecte et réordonnancement (prioritaires en tête)
                var allFiles = Directory
                    .GetFiles(Source, "*", SearchOption.AllDirectories)
                    .OrderBy(f => prioExt.Contains(Path.GetExtension(f).ToLower()) ? 0 : 1)
                    .ToList();

                // 4) Préparation du log & chiffrement
                var folders = new List<Folder>();
                string key = "cle123"; // TODO : remplacer par pm.Parametres.CléCryptage
                int total = allFiles.Count;
                int done = 0;

                // 5) Boucle de traitement
                foreach (var src in allFiles)
                {
                    // Pour bien visualiser la progression (optionnel)
                    Thread.Sleep(1000);

                    // a) Calcul du chemin de destination et création du dossier
                    string rel = Path.GetRelativePath(Source, src);
                    string dst = Path.Combine(Target, rel);
                    Directory.CreateDirectory(Path.GetDirectoryName(dst)!);

                    // b) Détermination Full vs Differential
                    bool shouldCopy = Type switch
                    {
                        BackupType.Full => true,
                        BackupType.Differential =>
                            !File.Exists(dst) ||
                            File.GetLastWriteTimeUtc(src) > File.GetLastWriteTimeUtc(dst),
                        _ => false
                    };

                    // c) Copie et chiffrement (le cas échéant)
                    long encTimeMs = 0;
                    if (shouldCopy)
                    {
                        // 1) Copie
                        File.Copy(src, dst, true);

                        // 2) Chiffrement via EncryptIfNeeded() + mesure
                        var swEnc = Stopwatch.StartNew();
                        int encResult = EncryptIfNeeded(dst, key);
                        swEnc.Stop();

                        switch (encResult)
                        {
                            case 1:  // au moins un fichier chiffré
                                     // si tu veux ajouter 1 s “fantôme” :
                                encTimeMs = swEnc.ElapsedMilliseconds + 1000;
                                break;
                            case 0:  // aucun fichier à chiffrer
                                encTimeMs = 0;
                                break;
                            default:  // -1 : erreur
                                encTimeMs = -1;
                                break;
                        }
                    }


                    // d) Enregistrement des infos pour le log
                    var fi = new FileInfo(src);
                    var entry = new Folder(
                        src,
                        fi.LastWriteTime,
                        fi.Name,
                        true,
                        fi.Length
                    );
                    entry.SetEncryptionTimeMs(encTimeMs);
                    folders.Add(entry);

                    // e) Mise à jour de la barre de progression
                    done++;
                    Progress = 100.0 * done / total;
                }

                // Arrêt du chrono global
                swTotal.Stop();

                // 6) Création et écriture du log final
                Log = new LogEntry(
                    DateTime.Now,
                    Name,
                    Type,
                    Source,
                    Target,
                    folders.Count,
                    (int)swTotal.ElapsedMilliseconds,
                    State,
                    folders
                );
                Log.SetDurationMs((int)swTotal.ElapsedMilliseconds);
                Log.AppendToFile();

                return "done";
            }
            catch (Exception ex)
            {
                return $"Une erreur est survenue pendant la sauvegarde : {ex.Message}";
            }
        }




        /// <summary>Annule le job en cours.</summary>
        public string Cancel()
        {
            if (State == BackupState.Running)
            {
                State = BackupState.Cancelled;
                return $"Backup '{Name}' has been cancelled.";
            }
            return $"Cannot cancel backup '{Name}' as it is not currently running.";
        }

        /// <summary>
        /// Retourne 1 si au moins un fichier chiffré, 0 si aucun,
        /// -1 en cas d’erreur.
        /// </summary>
        /// <summary>
        /// Tente de chiffrer le fichier ciblé, en s'assurant qu'un seul appel
        /// à TransformFile() de CryptoSoft peut s'exécuter simultanément.
        /// Retourne :
        ///   1  si chiffrement exécuté avec succès,
        ///   0  si pas d'extension à chiffrer ou fichier introuvable,
        ///  -1  en cas d'erreur (mutex ou chiffrement).
        /// </summary>
        private int EncryptIfNeeded(string filePath, string encryptionKey)
        {
            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"[Encrypt] File not found: {filePath}");
                return 0;
            }

            var pm = new ParametersManager();
            var toEncrypt = pm.Parametres.ExtensionsChiffrees
                              .Select(e => e.StartsWith(".")
                                           ? e.ToLower()
                                           : "." + e.ToLower())
                              .ToHashSet();

            string ext = Path.GetExtension(filePath).ToLower();
            if (!toEncrypt.Contains(ext))
            {
                Debug.WriteLine($"[Encrypt] Extension not in list: {ext}");
                return 0;
            }

            try
            {
                Debug.WriteLine($"[Encrypt] Thread {Thread.CurrentThread.ManagedThreadId} waiting mutex…");
                _cryptoMutex.WaitOne();
                Debug.WriteLine($"[Encrypt] Thread {Thread.CurrentThread.ManagedThreadId} acquired mutex");

                try
                {
                    var swEnc = Stopwatch.StartNew();
                    new CryptoSoft.FileManager(filePath, encryptionKey).TransformFile();
                    swEnc.Stop();

                    Debug.WriteLine($"[Encrypt] Thread {Thread.CurrentThread.ManagedThreadId} encrypted '{filePath}' in {swEnc.ElapsedMilliseconds}ms");
                    return 1;
                }
                finally
                {
                    _cryptoMutex.ReleaseMutex();
                    Debug.WriteLine($"[Encrypt] Thread {Thread.CurrentThread.ManagedThreadId} released mutex");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Encrypt] Thread {Thread.CurrentThread.ManagedThreadId} ERROR: {ex.Message}");
                return -1;
            }
        }


        /// <summary>Exécution async si besoin.</summary>
        public Task<List<string>> ExecuteAsync() =>
            Task.Run(() => Execute());
    }
}
