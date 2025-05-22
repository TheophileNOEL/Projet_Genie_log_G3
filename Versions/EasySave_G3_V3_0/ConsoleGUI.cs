// See https://aka.ms/new-console-template for more information
using System.Reflection;
using System.IO;
using EasySave.Core;
namespace EasySave_G3_V1;

class Program
{
    static void main(string[] args)
    {
        Program p = new Program();
        string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        ConsoleViewModel consoleViewModel = new ConsoleViewModel();
        consoleViewModel.GetLangages().SearchLangages();
        Langage langage = new Langage("French.json", Path.Combine(exePath, @"..\\..\\..\\Langages\\French.json"));
        ScenarioList scenarioList = consoleViewModel.GetScenarioList();
        scenarioList.Load(Path.Combine(exePath, @"..\\..\\..\\scenarios.json"));
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
            case 1: Scenario(consoleViewModel, L); break;
            case 2: SelectLangage(consoleViewModel, L, consoleViewModel.GetLangages().GetListLangage()); break;
            case 3: Environment.Exit(0); break;
            default: ErrorEntry(consoleViewModel, L); break;
        }
    }

    void ErrorEntry(ConsoleViewModel consoleViewModel, Langage L)
    {
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["ErrorEntry"]);
        Begin(consoleViewModel, L);
    }
    void Scenario(ConsoleViewModel consoleViewModel, Langage L)
    {
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["ScenarioMenu"]);
        Console.WriteLine(L.GetElements()["ScenarioMenuOptions"]);
        string result = Console.ReadLine();
        switch (int.Parse(result))
        {
            case 1: Selectscenario(consoleViewModel, L); break;
            case 2: AddScenario(consoleViewModel, L); break;
            case 3: Updatescenario(consoleViewModel, L); break;
            case 4: DeleteScenario(consoleViewModel, L); break;
            case 5: Begin(consoleViewModel, L); break;
            default: ErrorEntry(consoleViewModel, L); break;
        }
    }
    void Selectscenario(ConsoleViewModel consoleViewModel, Langage L, string result = "")
    {
        ScenarioList scenarioList = consoleViewModel.GetScenarioList();
        string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        scenarioList.Load(Path.Combine(exePath, @"..\\..\\..\\scenarios.json"));

        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["Selectscenario"]);
        Console.WriteLine(L.GetElements()["Exemplescenario"]);
        Console.WriteLine(L.GetElements()["Separator"].Substring(0, 20));

        int c = 1;
        foreach (Scenario scenario in scenarioList.Get())
        {
            Console.WriteLine($"{scenario.GetId()}     {scenario.GetName()}     {scenario.GetSceanrioType()}     {scenario.GetSource()} --> {scenario.GetTarget()}");
            c++;
        }
        Console.WriteLine($"{c}     {L.GetElements()["Back"]}");

        result = Console.ReadLine();
        Console.WriteLine(L.GetElements()["Separator"]);

        if (int.TryParse(result, out int parsedResult) && parsedResult == c)
        {
            Begin(consoleViewModel, L);
            return;
        }

        if (IsRange(result))
        {
            string[] parts = result.Split('-');
            if (parts.Length == 2
                && int.TryParse(parts[0].Trim(), out int start)
                && int.TryParse(parts[1].Trim(), out int end)
                && start <= end)
            {
                var message = scenarioList.RunRange(start, end);
                foreach (var kvp in message)
                    Console.WriteLine(kvp.Key.GetLog().Display());
            }
            else
            {
                Console.WriteLine("Format de plage invalide.");
            }
            Begin(consoleViewModel, L);
            return;
        }

        if (IsList(result))
        {
            try
            {
                int[] ids = result.Split(';').Select(x => int.Parse(x.Trim())).ToArray();
                var message = scenarioList.RunList(ids);
                foreach (var kvp in message)
                    Console.WriteLine(kvp.Key.GetLog().Display());
            }
            catch
            {
                Console.WriteLine("Format de liste invalide.");
            }
            Begin(consoleViewModel, L);
            return;
        }

        if (int.TryParse(result, out int id))
        {
            var message = scenarioList.RunList(new int[] { id });
            foreach (var kvp in message)
                Console.WriteLine(kvp.Key.GetLog().Display());
            Begin(consoleViewModel, L);
            return;
        }
        Console.WriteLine("Entrée invalide.");
        Begin(consoleViewModel, L);
    }
    void AddScenario(ConsoleViewModel consoleViewModel, Langage L)
    {
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["AddScenarioPrompt"]);
        Console.WriteLine(L.GetElements()["ScenarioName"]);
        string name = Console.ReadLine();
        Console.WriteLine(L.GetElements()["Source"]);
        string source = Console.ReadLine();
        Console.WriteLine(L.GetElements()["Target"]);
        string target = Console.ReadLine();
        Console.WriteLine(L.GetElements()["BackupType"]);
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
                Begin(consoleViewModel, L);
                return;
        }
        Console.WriteLine(L.GetElements()["Description"]);
        string description = Console.ReadLine();
        Scenario scenario = consoleViewModel.GetScenarioList().CreateScenario(name, source, target, backupType, description);
        Begin(consoleViewModel, L);
        return;
    }

    void DeleteScenario(ConsoleViewModel consoleViewModel, Langage L)
    {
        Console.WriteLine(L.GetElements()["Separator"]);
        List<int> IDS = new List<int>();
        foreach (Scenario scenario in consoleViewModel.GetScenarioList().Get())
        {
            Console.WriteLine(scenario.GetId() + "     " + scenario.GetName() + "     " + scenario.GetSceanrioType() + "     " + scenario.GetSource() + " --> " + scenario.GetTarget());
            IDS.Add(scenario.GetId());
        }
        Console.WriteLine(L.GetElements()["DeleteScenarioPrompt"]);
        string id = Console.ReadLine();
        if (IDS.Contains(int.Parse(id)))
        {
            Console.WriteLine(string.Format(L.GetElements()["DeleteScenarioConfirm"], id));
            switch (Console.ReadLine())
            {
                case "y":
                    consoleViewModel.GetScenarioList().RemoveScenario(int.Parse(id));
                    break;
                case "n":
                    break;
                default:
                    Console.WriteLine(L.GetElements()["ErrorType"]);
                    Begin(consoleViewModel, L);
                    return;
            }
        }
        else
        {
            Console.WriteLine(L.GetElements()["ErrorType"]);
            Begin(consoleViewModel, L);
            return;
        }
        Begin(consoleViewModel, L);
        return;
    }
    void Updatescenario(ConsoleViewModel consoleViewModel, Langage L)
    {
        ScenarioList scenarioList = consoleViewModel.GetScenarioList();
        Console.WriteLine(L.GetElements()["Separator"]);
        List<int> IDS = new List<int>();
        int j = 1;
        foreach (Scenario scenario in scenarioList.Get())
        {
            Console.WriteLine(scenario.GetId() + "     " + scenario.GetName() + "     " + scenario.GetSceanrioType() + "     " + scenario.GetSource() + " --> " + scenario.GetTarget());
            IDS.Add(scenario.GetId());
            j++;
        }
        Console.WriteLine(j + "     " + L.GetElements()["Back"]);
        Console.WriteLine(L.GetElements()["UpdateScenarioPrompt"]);
        int id = int.Parse(Console.ReadLine());
        if (id == j)
        {
            Begin(consoleViewModel, L);
        }
        else if (IDS.Contains(id))
        {
            Console.WriteLine(L.GetElements()["AddScenarioPrompt"]);
            Console.WriteLine(L.GetElements()["ScenarioName"]);
            string name = Console.ReadLine();
            Console.WriteLine(L.GetElements()["Source"]);
            string source = Console.ReadLine();
            Console.WriteLine(L.GetElements()["Target"]);
            string target = Console.ReadLine();
            Console.WriteLine(L.GetElements()["BackupType"]);
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
                    Begin(consoleViewModel, L);
                    return;
            }

            Console.WriteLine(L.GetElements()["Description"]);
            string description = Console.ReadLine();
            consoleViewModel.GetScenarioList().Modify(id, id, name, source, target, backupType, description);
            Begin(consoleViewModel, L);
            return;
        }
        else
        {
            Begin(consoleViewModel, L);
            Console.WriteLine(L.GetElements()["ErrorType"]);
            return;
        }
    }
    void SelectLangage(ConsoleViewModel consoleViewModel, Langage L, List<Langage> listLangages)
    {
        Console.WriteLine(L.GetElements()["Separator"]);
        Console.WriteLine(L.GetElements()["ChangeLangages"]);
        int i = 1;
        foreach (Langage l in listLangages)
        {
            Console.WriteLine(i + "    " + l.GetTitle().Split(".")[0]);
            i++;
        }
        Console.WriteLine(i + "    " + L.GetElements()["Back"]);
        int result = int.Parse(Console.ReadLine());
        if (result == i)
            Begin(consoleViewModel, L);
        else if (result > 0 && result < i)
            Begin(consoleViewModel, listLangages[result - 1]);
        else
            ErrorEntry(consoleViewModel, L);
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