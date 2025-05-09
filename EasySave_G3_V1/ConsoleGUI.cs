// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Reflection;
namespace EasySave_G3_V1;

class Programm
{
    static void Main()
    {
        Programm p = new Programm();
        Langage langage = new Langage("French.json", "C:\\Users\\User\\OneDrive - Association Cesi Viacesi mail\\CESI\\FISE A3\\Genie logiciel\\Projet\\EasySave_G3_V1\\EasySave_G3_V1\\Langages\\French.json");
        p.Begin(langage);
        
    }
    void Begin(Langage Langue)
    {
        // Loading the French langage
        Langages ListLangage = new Langages();
        ListLangage.SearchLangages();
        Langage L = new Langage();
        foreach (Langage l in ListLangage.GetListLangage())
        {
            if (l.GetTitle() == Langue.GetTitle())
            {
                l.LoadLangage();
                L = l;
            }
        }
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["Welcome"]);
        Console.WriteLine(L.GetElements()["SelectAction"]);
        int result = int.Parse(Console.ReadLine());
        switch (result)
        {
            case 1: SelectScript(L); break;
            case 2: UpdateScript(L); break;
            case 3: SelectLangage(L,ListLangage.GetListLangage()); break;
            case 4:break;
            default: ErrorEntry(L);break ;
        }
    }

    void ErrorEntry(Langage L)
    {
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine("Error bad entry. Please try again.");
        Begin(L);
    }
    void SelectScript(Langage L)
    {
        ScenarioList scenarioList = new ScenarioList();
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["SelectScript"]);
        Console.WriteLine(L.GetElements()["ExempleScript"]);
        Console.WriteLine(L.GetElements()["Separator"].Substring(0, 20));

        foreach (Scenario scenario in scenarioList.Load("C:\\Users\\theop\\OneDrive - Association Cesi Viacesi mail\\CESI\\FISE A3\\Genie logiciel\\Projet\\EasySave_G3_V1\\EasySave_G3_V1\\scenarios.json"))
        {
            Console.WriteLine(scenario.Id + "     " + scenario.Name + "     " + scenario.Type + "     " + scenario.Source + " --> " + scenario.Target);
        }

        string result = Console.ReadLine();
        if (IsRange(result))
        {
            // Extract Begin and end range from the input  
            string[] rangeParts = result.Split('-');
            int Begin = int.Parse(rangeParts[0].Trim());
            int end = int.Parse(rangeParts[1].Trim());
            // Call RunRange with the extracted parameters  
            scenarioList.RunRange(Begin, end);
        }
        else if (IsList(result)) 
        {
            // Extract Begin and end range from the input  
            string[] rangeParts = result.Split(',');
            foreach (string i in rangeParts)
            {
                int Begin = int.Parse(i.Trim());
                // Call RunRange with the extracted parameters  
                scenarioList.RunList(new int[] { Begin });
            }
        }
        else
        {
            if (int.TryParse(result, out int id))
            { 
                scenarioList.RunList([int.Parse(result)]);
            }
            
        }
    }

    void UpdateScript(Langage L)
    {
        Console.WriteLine(L.GetElements()["Separator"]);
    }
    void SelectLangage(Langage L, List<Langage> listLangages) 
    {
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["ChangeLangages"]);
        int i = 1;
        foreach (Langage l in listLangages)
        {
            Console.WriteLine(i+"    "+l.GetTitle().Split(".")[0]);
            i++;
        }
        int result = int.Parse(Console.ReadLine());
        Begin(listLangages[result-1]);
    }
    static bool IsRange(string texte)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(texte, @"^\d+\s*-\s*\d+$");
    }

    static bool IsList(string texte)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(texte, @"^\d+(\s*,\s*\d+)+$");
    }
}