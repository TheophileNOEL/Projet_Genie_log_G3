using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using EasySave.Core;

namespace EasySave_G3_V1
{
    public class Langages
    {
        private string Source;
        private List<Langage> ListLangage;

        public Langages()
        {
            Source = string.Empty;
            ListLangage = new List<Langage>();
        }

        public Langages(string source)
        {
            Source = source;
            ListLangage = new List<Langage>();
        }
        public void AddLangage(Langage langage)
        {
            this.ListLangage.Add(langage);
        }
        public void RemoveLangage(Langage langage)
        {
            ListLangage.Remove(langage);
        }
        public List<Langage> GetListLangage()
        {
            return ListLangage;
        }
        public string GetSource()
        {
            return Source;
        }
        public void SetSource(string source)
        {
            Source = source;
        }
        public void SetListLangage(List<Langage> listLangage)
        {
            ListLangage = listLangage;
        }
        public void ClearListLangage()
        {
            ListLangage.Clear();
        }
        public void SearchLangages()
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string TargetFolder = Path.Combine(exePath, @"..\..\..\Langages");
            string[] fichiers = Directory.GetFiles(TargetFolder);
            foreach (string fichier in fichiers)
            {
                const string Separator = @"\";
                int last_element = fichier.Split(Separator).Length;
                string Title = fichier.Split(Separator)[last_element-1];
                this.AddLangage(new Langage(Title, fichier));
            }
        }
    }
}
