using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Incas.DialogSimpleForm.Components
{
    class ResourceInstance : FrameworkElement
    {
        public static Style FindStyle(string name)
        {
            ResourceInstance inst = new();
            return inst.FindResource(name) as Style;
        }
    }
}
