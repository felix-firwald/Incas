using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Common
{
    [Export]
    public static class PluginManager
    {
        public static void GetAddIns()
        {
            //string[] addInAssemlies = Directory.GetFiles(ProgramState.AddIns, "*.dll");
            //List<IAddIn> addInTypes = new();
            //AppDomain app2 = AppDomain.CreateDomain("Plugins");

        }
    }
}
