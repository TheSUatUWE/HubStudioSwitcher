using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubStudioSwitcher
{
    class StudioOption
    {
        private string name;
        private string command;

        public StudioOption(string name, string command)
        {
            this.name = name;
            this.command = command;
        }

        public string GetName()
        {
            return this.name;
        }

        public string GetCommand()
        {
            return this.command;
        }
    }
}
