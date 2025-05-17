using EasySave_G3_V1;
using System.Text.Json.Serialization;
using System.Text.Json;
using EasySave.Core;
public class ScenarioList
{
    private List<Scenario> items = new List<Scenario>();
    private int scenarioStart;
    private int scenarioEnd;

    public ScenarioList()
    {
        scenarioStart = 1;
        scenarioEnd = 5;
    }

    public List<Scenario> Load(string filePath)
    {
        var json = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        List<Scenario> scenarios = JsonSerializer.Deserialize<List<Scenario>>(json, options);
        this.items = scenarios;
        return scenarios;
    }

    public Dictionary<Scenario, List<string>> RunRange(int start, int end, LogFormat logFormat)
    {
        if (start < 1 || end > items.Count || start > end)
            throw new ArgumentOutOfRangeException("Plage invalide.");

        var result = new Dictionary<Scenario, List<string>>();

        for (int i = start; i <= end; i++)
        {
            var scenario = items[i - 1];
            if (scenario != null)
            {

                var messages = scenario.Execute(logFormat); 
                result.Add(scenario, messages);
            }
        }

        return result;
    }


    public Dictionary<Scenario, List<string>> RunList(int[] ids, LogFormat logFormat)
    {
        var result = new Dictionary<Scenario, List<string>>();

        foreach (int i in ids)
        {
            if (i >= 1 && i <= items.Count && items[i - 1] != null)
            {
                var scenario = items[i - 1];
                var messages = scenario.Execute(logFormat);
                result.Add(scenario, messages);
            }
        }

        return result;
    }


    public bool Modify(int index, int? newId = null, string newName = null, string newSource = null,
                   string newTarget = null, EasySave.Core.BackupType? newType = null, string newDesc = null)
    {
        if (index <= 0 || index > items.Count || items[index - 1] == null)
            throw new IndexOutOfRangeException("Index invalide ou scénario vide.");

        var current = items[index - 1];

        // ID handling
        if (newId.HasValue)
        {
            var existing = items.FirstOrDefault(s => s != null && s.GetId() == newId.Value && s != current);
            if (existing != null)
            {
                int temp = current.GetId();
                current.SetId(existing.GetId());
                existing.SetId(temp);
            }
            else
            {
                current.SetId(newId.Value);
            }
        }

        if (!string.IsNullOrWhiteSpace(newName)) current.SetName(newName);
        if (!string.IsNullOrWhiteSpace(newSource)) current.SetSource(newSource);
        if (!string.IsNullOrWhiteSpace(newTarget)) current.SetTarget(newTarget);
        if (newType.HasValue) current.SetType(newType.Value);
        if (!string.IsNullOrWhiteSpace(newDesc)) current.SetDescription(newDesc);

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(items.Where(i => i != null).ToList(), options);
        string filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\scenarios.json"));

        File.WriteAllText(filePath, json);


        return true;
    }


    public Scenario CreateScenario(string name, string source, string target, EasySave.Core.BackupType type, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Le nom ne peut pas être vide.");
        if (string.IsNullOrWhiteSpace(source))
            throw new ArgumentException("La source ne peut pas être vide.");
        if (string.IsNullOrWhiteSpace(target))
            throw new ArgumentException("La destination ne peut pas être vide.");

        var usedIds = items.Where(s => s != null).Select(s => s.GetId()).ToHashSet();

        int newId = 1;
        while (usedIds.Contains(newId)) newId++;

        var newScenario = new Scenario(newId, name, source, target, type, description);
        items.Add(newScenario);

        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(items.Where(i => i != null).ToList(), options);
        string filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\scenarios.json"));

        File.WriteAllText(filePath, json);

        return newScenario;
    }


    public bool RemoveScenario(int id)
    {
        var scenario = items.FirstOrDefault(s => s != null && s.GetId() == id);
        if (scenario == null)
            return false;

        items.Remove(scenario);
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(items.Where(i => i != null).ToList(), options);
        string filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\scenarios.json"));

        File.WriteAllText(filePath, json);

        return true;
    }

    public List<Scenario> Get() 
    {
        return items;
    }

    public ScenarioList Search(string keyword)
    {
        var results = items
            .Where(s => s != null &&
                        (s.GetName()?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true ||
                         s.GetDescription()?.Contains(keyword, StringComparison.OrdinalIgnoreCase) == true))
            .ToList();

        var newList = new ScenarioList();
        newList.items = results;

        return newList;
    }
}
