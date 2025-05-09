using System;
using System.Collections.Generic;

namespace EasySave_G3_V1
{
    public class ConsoleViewModel
    {
        private Scripts scripts;
        private Langages langages;

        public ConsoleViewModel()
        {
            scripts = new Scripts();
            langages = new Langages();
        }

        // Loading available scripts
        public void LoadScripts()
        {
            scripts.LoadScript();
        }

        // Loading language files
        public void LoadLangages()
        {
            langages.SearchLangages();
        }

        // Send a language to the view or application logic
        public void SendLangage(Langage langage)
        {
            Console.WriteLine($"Langue sélectionnée : {langage.GetTitle()}");
        }

        // Execute a script using an identifier (name, path, or index)
        public void RunScript(string scriptName)
        {
            List<Script> list = scripts.GetListScript();
            Script scriptToRun = list.Find(s => s.GetTitle().Equals(scriptName, StringComparison.OrdinalIgnoreCase));
            if (scriptToRun != null)
            {
                //   To be implemented: call a method to execute within the Script class
                                Console.WriteLine($"Exécution du script : {scriptToRun.GetTitle()}");
            }
            else
            {
                Console.WriteLine("Script introuvable.");
            }
        }

        // Retrieve the list of languages
        public List<Langage> GetLangages()
        {
            return langages.GetListLangage();
        }

        // Retrieve the list of scripts
        public List<Script> GetScripts()
        {
            return scripts.GetListScript();
        }

        // Manually add a language (useful for debugging or testing
        public void AddLangage(Langage langage)
        {
            langages.AddLangage(langage);
        }
    }
}
