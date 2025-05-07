using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_G3_V1
{
    public class Log
    {
        private string Title;
        private List<Folder> ListFolders;
        private int Id;
        private int Duration;
        private DateTime Date;
        private bool Status;
        private string Source;
        private string Target;
        private string Description;

        public Log()
        {
            Title = string.Empty;
            ListFolders = new List<Folder>();
            Id = -1;
            Duration = 0;
            Date = DateTime.Now;
            Status = false;
            Source = string.Empty;
            Target = string.Empty;
            Description = string.Empty;
        }
        public Log(string title, List<Folder> listFolders, int id, int duration, DateTime date, bool status, string source, string target, string description)
        {
            Title = title;
            ListFolders = listFolders;
            Id = id;
            Duration = duration;
            Date = date;
            Status = status;
            Source = source;
            Target = target;
            Description = description;
        }
        public string GetTitle()
        {
            return Title;
        }
        public List<Folder> GetListFolders()
        {
            return ListFolders;
        }
        public int GetId()
        {
            return Id;
        }
        public int GetDuration()
        {
            return Duration;
        }
        public DateTime GetDate()
        {
            return Date;
        }
        public bool GetStatus()
        {
            return Status;
        }
        public string GetSource()
        {
            return Source;
        }
        public string GetTarget()
        {
            return Target;
        }
        public string GetDescription()
        {
            return Description;
        }
        public void SetTitle(string title)
        {
            Title = title;
        }
        public void SetListFolders(List<Folder> listFolders)
        {
            ListFolders = listFolders;
        }
        public void SetId(int id)
        {
            Id = id;
        }
        public void SetDuration(int duration)
        {
            Duration = duration;
        }
        public void SetDate(DateTime date)
        {
            Date = date;
        }
        public void SetStatus(bool status)
        {
            Status = status;
        }
        public void SetSource(string source)
        {
            Source = source;
        }
        public void SetTarget(string target)
        {
            Target = target;
        }
        public void SetDescription(string description)
        {
            Description = description;
        }
        public void AddFolder(Folder folder)
        {
            ListFolders.Add(folder);
        }
        public void RemoveFolder(Folder folder)
        {
            ListFolders.Remove(folder);
        }
    }
}
