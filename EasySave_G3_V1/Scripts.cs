using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;


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
        public void LoadScript()
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string TargetFolder = Path.Combine(exePath, @"..\..\..\Script");
            string[] fichiers = Directory.GetFiles(TargetFolder);
            foreach (string fichier in fichiers)
            {
                const string Separator = @"\";
                int last_element = fichier.Split(Separator).Length;
                string Title = fichier.Split(Separator)[last_element - 1];
                this.AddScript(new Script())
            }
        }
    }
}
