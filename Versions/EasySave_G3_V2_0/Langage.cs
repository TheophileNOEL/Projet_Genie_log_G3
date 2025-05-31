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
                // reading language from settings.json
                using FileStream fs = File.OpenRead("settings.json");
                using JsonDocument doc = JsonDocument.Parse(fs);

                JsonElement root = doc.RootElement;
                if (root.TryGetProperty("Langue", out JsonElement langueElement))
                {
                    Title = langueElement.GetString();
                    Source = $"Langages/{Title}.json";
                }
                else
                {
                    Title = "Français";
                    Source = "Langages/Français.json";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erreur de chargement de settings.json : {e.Message}");
                Title = "Français";
                Source = "Langages/Français.json";
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
}
