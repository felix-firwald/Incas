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
        public ClassData SourceData { get; set; }
        public Class Source;
        public ClassViewModel(Class source)
        {
            this.Source = source;
            this.SourceData = this.Source.GetClassData();
        }
        public string NameOfClass
        {
            get
            {
                return this.Source.name;
            }
            set
            {
                this.Source.name = value;
                this.OnPropertyChanged(nameof(this.NameOfClass));
            }
        }
        public string CategoryOfClass
        {
            get
            {
                return this.Source.category;
            }
            set
            {
                this.Source.category = value;
                this.OnPropertyChanged(nameof(this.CategoryOfClass));
            }
        }
        public void SetData(List<Field> fields)
        {
            this.SourceData.fields = fields;
            this.Source.SetClassData(this.SourceData);
        }
    }
}
