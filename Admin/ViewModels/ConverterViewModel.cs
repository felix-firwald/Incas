using Incas.Core.ViewModels;
using IncasEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Admin.ViewModels
{
    public class ConverterViewModel : BaseViewModel
    {
        public Converter Source { get; set; }
        public ConverterViewModel()
        {
            this.Source = new();
        }
        public ConverterViewModel(Converter conv)
        {
            this.Source = conv;
        }

    }
}
