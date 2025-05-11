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
    private int scriptStart;
    private int scriptEnd;

    public ScenarioList()
    {
        scriptStart = 1;
        scriptEnd = 5;
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

    public void Modify(int index, Langage L)
    {
        if (index <= 0 || index >= items.Count || items[index-1] == null)
            throw new IndexOutOfRangeException("Index invalide ou scénario vide.");

        var current = items[index-1];

        Console.WriteLine($"Modification du scénario à l'index {index} (actuel : {current.GetName()})");

        // ID
        Console.Write("Nouvel ID (laisser vide pour garder) : ");
        var newIdStr = Console.ReadLine();
        int newId = current.GetId();
        if (!string.IsNullOrWhiteSpace(newIdStr) && int.TryParse(newIdStr, out int parsedId))
        {
            var existing = items.FirstOrDefault(s => s != null && s.GetId() == parsedId && s != current);
            if (existing != null)
            {
                Console.WriteLine($"ID {parsedId} already used. Inverting ID.");
                int temp = current.GetId();
                current.SetId(existing.GetId());
                existing.SetId(temp);
            }
            else
            {
                newId = parsedId;
            }
        }


        Console.Write("Nouveau nom (laisser vide pour garder) : ");
        var newName = Console.ReadLine();


        Console.Write("Nouvelle source (laisser vide pour garder) : ");
        var newSource = Console.ReadLine();

        Console.Write("Nouvelle destination (laisser vide pour garder) : ");
        var newTarget = Console.ReadLine();


        Console.Write("Nouveau type (Full ou Differential) (laisser vide pour garder) : ");
        var newTypeStr = Console.ReadLine();
        BackupType newType = current.GetType();
        if (!string.IsNullOrWhiteSpace(newTypeStr))
        {
            if (Enum.TryParse<BackupType>(newTypeStr, true, out var parsedType))
            {
                newType = parsedType;
            }
            else
            {
                Console.WriteLine("Type invalide, type actuel conservé.");
            }
        }


        Console.Write("Nouvelle description (laisser vide pour garder) : ");
        var newDesc = Console.ReadLine();


        current.SetId(newId);
        if (!string.IsNullOrWhiteSpace(newName)) current.SetName(newName);
        if (!string.IsNullOrWhiteSpace(newSource)) current.SetSource(newSource);
        if (!string.IsNullOrWhiteSpace(newTarget)) current.SetTarget(newTarget);
        current.SetType(newType);
        if (!string.IsNullOrWhiteSpace(newDesc)) current.SetDescription(newDesc);

        Console.WriteLine("Modification terminée.");
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
