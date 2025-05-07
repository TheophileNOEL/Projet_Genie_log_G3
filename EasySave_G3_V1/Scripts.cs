using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_G3_V1
{
    public class Scripts
    {
        private string Source;
        private List<Script> ListScript;

        public Scripts()
        {
            Source = string.Empty;
            ListScript = new List<Script>();
        }
        public Scripts(string source)
        {
            Source = source;
            ListScript = new List<Script>();
        }
        public void AddScript(Script script)
        {
            ListScript.Add(script);
        }
        public void RemoveScript(Script script)
        {
            ListScript.Remove(script);
        }
        public List<Script> GetListScript()
        {
            return ListScript;
        }
        public string GetSource()
        {
            return Source;
        }
        public void SetSource(string source)
        {
            Source = source;
        }
    }
}
