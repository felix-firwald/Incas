using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Field = Incas.Objects.Models.Field;

namespace Incas.Objects.ViewModels
{
    internal class ClassViewModel : BaseViewModel
    {
        private Class source;
        public ClassViewModel(Class source)
        {
            this.source = source;
            if (this.source.data == null)
            {
                //this.source.GetClassData = new();
                //this.source.data.fields = new();
            }
        }
    }
}
