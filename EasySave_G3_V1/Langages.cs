using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            ListLangage.Add(langage);
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
    }
}
