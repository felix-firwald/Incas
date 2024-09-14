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
        public void ShowDialog(string title, Icon icon)
        {
            DialogsManager.ShowSimpleFormDialog(this, title, icon);
        }
    }
}
