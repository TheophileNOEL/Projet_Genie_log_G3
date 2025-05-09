// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Reflection;
namespace EasySave_G3_V1;

class Progamm
{
    static void Main()
    {
        Langages ListLangage = new Langages();
        ListLangage.SearchLangages();
        Console.WriteLine("Liste de langues :");
        foreach (Langage l in ListLangage.GetListLangage())
        {
            l.LoadLangage();
            foreach (KeyValuePair<string,string> k in l.GetElements())
            {
                Console.WriteLine(k.Key);
                Console.WriteLine(k.Value);
            }
        }
        Console.WriteLine("________________");
    }
}