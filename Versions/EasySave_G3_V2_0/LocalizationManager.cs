using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace EasySave_G3_V2_0.Services
{
    public static class LocalizationManager
    {
        // Index où on injecte notre dictionnaire de ressources
        private const int ResourceIndex = 0;

        /// <summary>
        /// Charge la langue au démarrage ou lors d'un changement.
        /// </summary>
        /// <param name="langage">Le nom du fichier de langue (ex: "French", "English")</param>
        public static void ChangeLanguage(string langage)
        {
            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Langages");
            string filePath = Path.Combine(basePath, $"{langage}.json");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Fichier de langue introuvable : {filePath}");

            string json = File.ReadAllText(filePath);
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            // Créer un ResourceDictionary WPF
            var rd = new ResourceDictionary();
            foreach (var kvp in dict)
            {
                rd[kvp.Key] = kvp.Value;
            }

            // Insérer ou remplacer dans les ressources de l'App
            var appResources = Application.Current.Resources.MergedDictionaries;
            if (appResources.Count > ResourceIndex)
                appResources[ResourceIndex] = rd;
            else
                appResources.Add(rd);
        }
    }
}
