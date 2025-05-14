// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
namespace EasySave_G3_V1;

class Programm
{
    static void Main(string[] args)
    {
        Programm p = new Programm();
        string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ConsoleViewModel consoleViewModel = new ConsoleViewModel();
        consoleViewModel.GetLangages().SearchLangages();
        Langage langage = new Langage("French.json", Path.Combine(exePath, @"..\\..\\..\\Langages\\French.json"));
        langage.LoadLangage();
        if (args.Length > 0)
        {
            p.Selectscenario(consoleViewModel, langage, args[0]);
        }
        p.Begin(consoleViewModel, langage);
    }
    void Begin(ConsoleViewModel consoleViewModel, Langage L)
    {
        string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        // Loading the French langage
        if (L.GetElements().Count <= 1)
        {
            L.LoadLangage();
        }
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["Welcome"]);
        Console.WriteLine(L.GetElements()["SelectAction"]);
        int result = int.Parse(Console.ReadLine());
        switch (result)
        {
            case 1: Selectscenario(consoleViewModel, L); break;
            case 2: Updatescenario(consoleViewModel, L); break;
            case 3: SelectLangage(consoleViewModel, L,consoleViewModel.GetLangages().GetListLangage()); break;
            case 4: Environment.Exit(0);break ;
            default: ErrorEntry(consoleViewModel, L);break ;
        }
    }

    void ErrorEntry(ConsoleViewModel consoleViewModel, Langage L)
    {
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine("Error bad entry. Please try again.");
        Begin(consoleViewModel, L);
    }
    void Selectscenario(ConsoleViewModel consoleViewModel, Langage L, string result = "")
    {
        ScenarioList scenarioList = consoleViewModel.GetScenarioList();
        string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        scenarioList.Load(Path.Combine(exePath,@"..\\..\\..\\scenarios.json"));
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["Selectscenario"]);
        Console.WriteLine(L.GetElements()["Exemplescenario"]);
        Console.WriteLine(L.GetElements()["Separator"].Substring(0, 20));
        var c = 1;
        foreach (Scenario scenario in scenarioList.Get())
        {
            Console.WriteLine(scenario.GetId() + "     " + scenario.GetName() + "     " + scenario.GetSceanrioType() + "     " + scenario.GetSource() + " --> " + scenario.GetTarget());
            c++;
        }
        Console.WriteLine(c + "     " + L.GetElements()["Back"]);
        result = Console.ReadLine();
        Console.WriteLine(L.GetElements()["Separator"]);
        if (int.Parse(result) == c) 
             Begin(consoleViewModel, L);
        else if (IsRange(result))
        {
            // Extract start and end range from the input  
            string[] rangeParts = result.Split('-');
            int Start = int.Parse(rangeParts[0].Trim());
            int end = int.Parse(rangeParts[1].Trim());
            // Call RunRange with the extracted parameters  
            scenarioList.RunRange(Start, end);
            Begin(consoleViewModel, L);
        }

        else if (IsList(result)) 
        {
            // Extract start and end range from the input  
            string[] rangeParts = result.Split(',');
            foreach (string i in rangeParts)
            {
                int Start = int.Parse(i.Trim());
                // Call RunRange with the extracted parameters  
                scenarioList.RunList(new int[] { Start });
            }
            Begin(consoleViewModel, L);
        }

        else
        {
            if (int.TryParse(result, out int id))
            { 
                scenarioList.RunList([int.Parse(result)]);
            }
            Begin(consoleViewModel, L);
        }

        foreach (Scenario scenario in scenarioList.Get())
        {
            Console.WriteLine(scenario.GetLog().Display());
        }
    }

    void Updatescenario(ConsoleViewModel consoleViewModel, Langage L)
    {
        ScenarioList scenarioList = consoleViewModel.GetScenarioList();
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["Selectscenario"]);
        string exePath = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location);
        scenarioList.Load(Path.Combine(exePath, @"..\\..\\..\\scenarios.json"));
        int j = 1;
        foreach (Scenario scenario in scenarioList.Get())
        {
            Console.WriteLine(scenario.GetId() + "     " + scenario.GetName() + "     " + scenario.GetSceanrioType() + "     " + scenario.GetSource() + " --> " + scenario.GetTarget());
            j++;
        }
        Console.WriteLine(j + "     " + L.GetElements()["Back"]);
        int result = int.Parse(Console.ReadLine());
        if (result == j)
            Begin(consoleViewModel, L);
        else 
        {
            Console.WriteLine(L.GetElements()["Separator"]);
            Console.WriteLine(L.GetElements()["UpdateScenarioID"]);
            string id = Console.ReadLine();
            Console.WriteLine(L.GetElements()["UpdateScenarioName"]);
            string name = Console.ReadLine();
            Console.WriteLine(L.GetElements()["UpdateScenarioSource"]);
            string source = Console.ReadLine();
            Console.WriteLine(L.GetElements()["UpdateScenarioTarget"]);
            string target = Console.ReadLine();
            Console.WriteLine(L.GetElements()["UpdateScenarioType"]);
            string type = Console.ReadLine();
            BackupType backupType;
            switch (type)
            {
                case "Full":
                    backupType = BackupType.Full;
                    break;
                case "Differential":
                    backupType = BackupType.Differential;
                    break;
                default:
                    Console.WriteLine(L.GetElements()["ErrorType"]);
                    return;
            }
            Console.WriteLine(L.GetElements()["UpdateScenarioDescription"]);
            string description = Console.ReadLine();
            scenarioList.Modify(result,int.Parse(id), name, source, target, backupType ,description); 
        }
        Console.WriteLine(L.GetElements()["Separator"]);
        foreach (Scenario scenario in scenarioList.Get())
        {
            Console.WriteLine(scenario.GetId() + "     " + scenario.GetName() + "     " + scenario.GetSceanrioType() + "     " + scenario.GetSource() + " --> " + scenario.GetTarget());
        }
        Begin(consoleViewModel, L);
    }
    void SelectLangage(ConsoleViewModel consoleViewModel, Langage L, List<Langage> listLangages) 
    {
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["ChangeLangages"]);
        int i = 1;
        foreach (Langage l in listLangages)
        {
            Console.WriteLine(i+"    "+l.GetTitle().Split(".")[0]);
            i++;
        }
        Console.WriteLine(i + "    " + L.GetElements()["Back"]);
        int result = int.Parse(Console.ReadLine());
        if (result == i)
            Begin(consoleViewModel, L);
        else
            Begin(consoleViewModel, listLangages[result - 1]);
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