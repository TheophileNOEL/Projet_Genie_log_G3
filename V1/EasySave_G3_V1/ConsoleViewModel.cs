using System;
using System.Collections.Generic;
using EasySave.Core;
namespace EasySave_G3_V1
{
    public class ConsoleViewModel
    {
        private ScenarioList scenarioList;
        private Langages langages;

        public ConsoleViewModel()
        {
            scenarioList = new ScenarioList();
            langages = new Langages();
        }
        public Langages GetLangages()
        {
            return langages;
        }
        public void SetLangages(Langages langages)
        {
            this.langages = langages;
        }
        public ScenarioList GetScenarioList()
        {
            return scenarioList;
        }
        public void SetScenarioList(ScenarioList scenarioList)
        {
            this.scenarioList = scenarioList;
        }
    }
}
