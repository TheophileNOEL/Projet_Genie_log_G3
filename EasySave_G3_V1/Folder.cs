using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_G3_V1
{
    public class Folder
    {
        private string Source;
        private DateTime Date;
        private string Name;
        private bool isDrived;
        private int Size;
        private bool isFile;

        public Folder()
        {
            Source = string.Empty;
            Date = DateTime.Now;
            Name = string.Empty;
            isDrived = false;
            Size = 0;
            isFile = false;
        }

        public Folder(string source, DateTime date, string name, bool isDrived, int size, bool isFile)
        {
            Source = source;
            Date = date;
            Name = name;
            this.isDrived = isDrived;
            Size = size;
            this.isFile = isFile;
        }
        public string GetSource()
        {
            return Source;
        }
        public DateTime GetDate()
        {
            return Date;
        }
        public string GetName()
        {
            return Name;
        }
        public bool GetIsDrived()
        {
            return isDrived;
        }
        public int GetSize()
        {
            return Size;
        }
        public bool GetIsFile()
        {
            return isFile;
        }
        public void SetSource(string source)
        {
            Source = source;
        }
        public void SetDate(DateTime date)
        {
            Date = date;
        }
        public void SetName(string name)
        {
            Name = name;
        }
        public void SetIsDrived(bool isDrived)
        {
            this.isDrived = isDrived;
        }
        public void SetSize(int size)
        {
            Size = size;
        }
        public void SetIsFile(bool isFile)
        {
            this.isFile = isFile;
        }
        public bool IsSame(Folder Folder)
        {
            return true;
        }
        public bool Move(Folder Folder)
        {
            return true;
        }
    }
}
