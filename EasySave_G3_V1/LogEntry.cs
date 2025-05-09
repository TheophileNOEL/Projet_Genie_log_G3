using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EasySave_G3_V1
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
		private bool state;
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
			state = false;
			description = string.Empty;
		}

		public LogEntry(DateTime timestamp,
						string jobName,
						BackupType backupType,
						string sourceUNC,
						string targetUNC,
						long fileSizeBytes,
						long durationMs,
						bool state,
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
		public bool GetState() => state;
		public string GetDescription() => description;
		public List<Folder> GetListFolder() => listFolder;

		public void SetTimestamp(DateTime v) { timestamp = v; }
		public void SetJobName(string v) { jobName = v; }
		public void SetBackupType(BackupType v) { backupType = v; }
		public void SetSourceUNC(string v) { sourceUNC = v; }
		public void SetTargetUNC(string v) { targetUNC = v; }
		public void SetDurationMs(long v) { durationMs = v; }
		public void SetState(bool v) { state = v; }
		public void SetDescription(string v) { description = v; }
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
				   + $"Description        : {description}\n"
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
				listFolder = listFolder.ConvertAll(f => new {
					path = f.GetPath(),
					size = f.GetSize(),
					type = f.GetIsFile() ? "file" : "folder"
				})
			};
			var opts = new JsonSerializerOptions { WriteIndented = indent };
			return JsonSerializer.Serialize(anon, opts) + Environment.NewLine;
		}

		public void AppendToFile(string filePath, bool indent = false)
		{
			var dir = Path.GetDirectoryName(filePath);
			if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			lock (fileLock)
				File.AppendAllText(filePath, ToJson(indent), System.Text.Encoding.UTF8);
		}
	}
}
