using EasySave_G3_V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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

    // Load scenario 

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

    // Execute scenarios from (1,3)
    public void RunRange(int start, int end)
    {
        Console.WriteLine("Execution de la plage de scénario : " + start + " à " + end);
        if (start < 0 || end >= items.Count+1 || start > end)
            throw new ArgumentOutOfRangeException("Plage invalide.");

        for (int i = start; i <= end; i++)
        {
            if (items[i-1] != null)
            {
                items[i-1].Execute();
            }
        }
    }

    // Exectute sceanrios [1,2]
    public void RunList(int[] ids)
    {
        foreach (int i in ids)
        {
            if (i-1 >= 0 && i-1 < items.Count+1 && items[i-1] != null)
            {
                items[i-1].Execute();
            }
        }
    }

    public void Modify(int index, int? newId = null, string newName = null, string newSource = null,
                   string newTarget = null, BackupType? newType = null, string newDesc = null)
    {
        if (index <= 0 || index > items.Count || items[index - 1] == null)
            throw new IndexOutOfRangeException("Index invalide ou scénario vide.");

        var current = items[index - 1];

        if (newId.HasValue)
        {
            var existing = items.FirstOrDefault(s => s != null && s.GetId() == newId && s != current);
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
    }


    public List<Scenario> Get()
    {
        return items;
    }


    public ScenarioList Search(string keyword)
    {
        var results = items
            .Where(s => s != null && (s.GetName().Contains(keyword, StringComparison.OrdinalIgnoreCase) || s.GetDescription().Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        var newList = new ScenarioList();
        for (int i = 0; i < results.Length && i < 5; i++)
            newList.items[i] = results[i];

        return newList;
    }
}
