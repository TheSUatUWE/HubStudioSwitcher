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
        private int preset;

        public StudioOption(string name, string command, int preset)
        {
            this.name = name;
            this.command = command;
            this.preset = preset;
        }

        public string GetName()
        {
            return this.name;
        }

        public string GetCommand()
        {
            return this.command;
        }

        public int GetPreset()
        {
            return this.preset;
        }
    }
}
