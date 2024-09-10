using Incas.Core.ViewModels;
using Incas.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.ViewModels
{
    public class FieldViewModel : BaseViewModel
    {
        private Field source;
        public FieldViewModel(Field field)
        {
            this.source = field;
        }
        public FieldViewModel()
        {
            this.source = new();
        }
    }
}
