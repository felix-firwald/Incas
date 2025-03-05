using Incas.Core.ViewModels;
using IncasEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Admin.ViewModels
{
    public class GeneralizatorViewModel : BaseViewModel
    {
        public Generalizator Source { get; set; }
        public GeneralizatorViewModel()
        {
            this.Source = new();
        }
        public GeneralizatorViewModel(Generalizator gen)
        {
            this.Source = gen;
        }
    }
}
