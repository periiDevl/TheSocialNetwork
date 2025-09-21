using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuNet
{
    internal class ModulesManager
    {
        /// <summary>
        /// Section 0 :USER DATABASE
        /// Section 1 :FILE DATABASE (FTP)
        /// Section 2 :CHAT
        /// </summary>
        private List<Module> modules = new List<Module>();
        public ModulesManager() { }
        public void add(Module module) { modules.Add(module); }
        public void remove(Module module) { modules.Remove(module); }
        public List<Module> getModules() { return modules; }
        public string containsWhat()
        {
            
            if (modules.Count == 0)
            {
                return "There are no modules here...";
            }
            string text = $"There are {modules.Count} modules in this server.\n";
            text += "Contains:\n";
            int section = 0;
            foreach (Module module in modules)
            {
                text += section + 1 + ": " + module.ToString() + "\n";
                section++;
            }

            text += "Finished.\n";
            return text;
        }

    }
}
