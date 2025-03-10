using Incas.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.ViewModels
{
    public class View_Model1 : BaseViewModel
    {
        public object Source { get; set; }
        public View_Model1(object source)
        {
            this.Source = source;
        }
    }
}
