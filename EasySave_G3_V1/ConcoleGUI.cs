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
        p.Start(langage);
        
    }
    void Start(Langage Langue)
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
        }
    }

    void SelectScript(Langage L)
    {
        Console.WriteLine(L.GetElements()["Separator"]);
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
        Start(listLangages[result-1]);
    }
}