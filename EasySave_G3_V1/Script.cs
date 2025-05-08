using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave_G3_V1
{
    public class Script
    {
        private int Id;
        private string Source;
        private string Target;
        private bool State;
        private string Description;

        public Script()
        {
            Id = -1;
            Source = string.Empty;
            Target = string.Empty;
            State = false;
            Description = string.Empty;
        }
        public Script(string source, string target, bool state, string description)
        {
            this.Id = -1;
            Source = source;
            Target = target;
            State = state;
            Description = description;
        }
        public int GetId()
        {
            return Id;
        }
        public string GetSource()
        {
            return Source;
        }
        public string GetTarget()
        {
            return Target;
        }
        public bool GetState()
        {
            return State;
        }
        public string GetDescription()
        {
            return Description;
        }
        public void SetId(int id)
        {
            Id = id;
        }
        public void SetSource(string source)
        {
            Source = source;
        }
        public void SetTarget(string target)
        {
            Target = target;
        }
        public void SetState(bool state)
        {
            State = state;
        }
        public void SetDescription(string description)
        {
            Description = description;
        }
    }
}