using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class Parameters
{
	public string FormatLog { get; set; } = "JSON";
	public List<string> ExtensionsChiffrees { get; set; } = new();
	public List<string> CheminsLogiciels { get; set; } = new();
	public string Langue { get; set; } = "Français";
}

 
public class ParametersManager
{
    private const string FileName = "settings.json";
    public Parameters Parametres { get; private set; }

    public ParametersManager()
    {
        Load();
    }

    public void Load()
    {
        try
        {
            if (File.Exists(FileName))
            {
                string json = File.ReadAllText(FileName);
                Parametres = JsonSerializer.Deserialize<Parameters>(json);
                if (Parametres == null)
                    Parametres = new Parameters();
            }
            else
            {
                Parametres = new Parameters();
                Save();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur chargement paramètres : " + ex.Message);
            Parametres = new Parameters();
        }
    }

    public void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(Parametres, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FileName, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur sauvegarde paramètres : " + ex.Message);
        }
    }


    public void AjouterExtension(string extension)
    {
        if (!extension.StartsWith("."))
            extension = "." + extension;

        if (!Parametres.ExtensionsChiffrees.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            Parametres.ExtensionsChiffrees.Add(extension);
            Save();
        }
    }

    public void SupprimerExtension(string extension)
    {
        if (Parametres.ExtensionsChiffrees.RemoveAll(e => e.Equals(extension, StringComparison.OrdinalIgnoreCase)) > 0)
        {
            Save();
        }
    }

    public void ModifierFormatLog(string format)
    {
        Parametres.FormatLog = format;
        Save();
    }

	public void AjouterCheminLogiciel(string chemin)
	{
		if (!Parametres.CheminsLogiciels.Contains(chemin))
		{
			Parametres.CheminsLogiciels.Add(chemin);
			Save();
		}
	}


	public void ModifierLangue(string langue)
    {
        Parametres.Langue = langue;
        Save();
    }
}
