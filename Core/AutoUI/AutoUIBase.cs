using Incas.Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Core.AutoUI
{
    public abstract class AutoUIBase
    {
        public bool ShowDialog(string title, Icon icon)
        {
            return DialogsManager.ShowSimpleFormDialog(this, title, icon);
        }
    }
}
