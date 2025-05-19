using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace EasySave_G3_V1
{
    public class Langage
    {
        private string Title;
        private string Source;
        private Dictionary<string, string> Elements;

        public Langage()
        {
            Elements = new Dictionary<string, string>();

            try
            {
                // Lire settings.json
                string settingsContent = File.ReadAllText("settings.json");
                var settings = JsonSerializer.Deserialize<Settings>(settingsContent);

                Title = settings.Langue;
                Source = $"Lang/{Title}.json";
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erreur de chargement des paramètres : {e.Message}");
                Title = "Français";
                Source = "Lang/Français.json";
            }
        }

        public Langage(string title, string source)
        {
            Title = title;
            Source = source;
            Elements = new Dictionary<string, string>();
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

        public Dictionary<string, string> GetElements()
        {
            return Elements;
        }

        public void AddElement(Dictionary<string, string> element)
        {
            foreach (var kvp in element)
            {
                this.Elements[kvp.Key] = kvp.Value;
            }
        }

        public string LoadLangage()
        {
            try
            {
                string jsonContent = File.ReadAllText(this.GetSource());
                Dictionary<string, string> messages = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                this.AddElement(messages);
                return "\n";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }

    public class Settings
    {
        public string FormatLog { get; set; }
        public List<string> ExtensionsChiffrees { get; set; }
        public List<string> CheminsLogiciels { get; set; }
        public string Langue { get; set; }
    }
}
