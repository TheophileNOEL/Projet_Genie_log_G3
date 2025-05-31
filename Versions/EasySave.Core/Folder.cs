using System;
using System.IO;

namespace EasySave.Core
{
	public class Folder
	{
		private string path;
		private DateTime date;
		private string name;
		private bool isMounted;
		private long sizeBytes;
		private bool isFile;
        private long encryptionTimeMs;

        public Folder()
		{
			path = string.Empty;
			date = DateTime.Now;
			name = string.Empty;
			isMounted = false;
			sizeBytes = 0;
			isFile = false;
            encryptionTimeMs = 0;
        }

		public Folder(string path,
					  DateTime date,
					  string name,
					  bool isMounted,
					  long sizeBytes)
		{
			this.path = path;
			this.date = date;
			this.name = name;
			this.isMounted = isMounted;
			this.sizeBytes = sizeBytes;
			this.isFile = DetectIsFile(path);
            encryptionTimeMs = 0;
        }

		public string GetPath() { return path; }
		public DateTime GetDate() { return date; }
		public string GetName() { return name; }
		public bool GetIsMounted() { return isMounted; }
		public long GetSize() { return sizeBytes; }
		public bool GetIsFile() { return isFile; }

		public void SetPath(string v)
		{
			path = v;
			isFile = DetectIsFile(v);
		}
		public void SetDate(DateTime v) { date = v; }
		public void SetName(string v) { name = v; }
		public void SetIsMounted(bool v) { isMounted = v; }
        public long GetEncryptionTimeMs() { return encryptionTimeMs; }
        public void SetSize(long v) { sizeBytes = v; }
        public void SetEncryptionTimeMs(long v) { encryptionTimeMs = v; }

        public bool IsSame(Folder other)
		{
			return other != null
				&& string.Equals(path, other.GetPath(), StringComparison.InvariantCultureIgnoreCase);
		}

		public bool Move(Folder dest)
		{
			if (dest == null) return false;
			try
			{
				if (isFile)
					System.IO.File.Move(path, dest.GetPath());
				else
					System.IO.Directory.Move(path, dest.GetPath());

				path = dest.GetPath();
				date = DateTime.Now;
				sizeBytes = isFile ? new FileInfo(path).Length : 0;
				return true;
			}
			catch
			{
				return false;
			}
		}

		private bool DetectIsFile(string p)
		{
			if (System.IO.File.Exists(p)) return true;
			if (System.IO.Directory.Exists(p)) return false;
			return System.IO.Path.HasExtension(p);
		}
	}
}