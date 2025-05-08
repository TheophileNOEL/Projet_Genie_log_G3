// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Reflection;
namespace EasySave_G3_V1;

class Progamm
{
    static void Main()
    {
        Langages ListLanges = new Langages();
        ListLanges.SearchLangages();
        Console.WriteLine("Liste de langues :");
        foreach (Langage l in ListLanges.GetListLangage())
        {
            Console.WriteLine(l.GetTitle()+" : "+l.GetSource());
        }
        Console.WriteLine("________________");
    }
}