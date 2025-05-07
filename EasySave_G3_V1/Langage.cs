using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_G3_V1
{
    public class Langage
    {
        private string Title;
        private string Source;

        public Langage()
        {
            Title = string.Empty;
            Source = string.Empty;
        }

        public Langage(string title, string source)
        {
            Title = title;
            Source = source;
        }
        public string GetTitle()
        {
            return Title;
        }
        public string GetSource()
        {
            return Source;
        }
        public void SetTitle(string title)
        {
            Title = title;
        }
        public void SetSource(string source)
        {
            Source = source;
        }
    }
}
