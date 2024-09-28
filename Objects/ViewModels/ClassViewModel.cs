using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Components;
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
        public ClassType Type
        {
            get
            {
                return this.SourceData.ClassType;
            }
            set
            {
                this.SourceData.ClassType = value;
                this.OnPropertyChanged(nameof(this.Type));
            }
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
        public string NameTemplate
        {
            get
            {
                return this.SourceData.NameTemplate;
            }
            set
            {
                this.SourceData.NameTemplate = value;
                this.OnPropertyChanged(nameof(this.NameTemplate));
            }
        }
        public bool ShowCard
        {
            get
            {
                return this.SourceData.ShowCard;
            }
            set
            {
                this.SourceData.ShowCard = value;
                this.OnPropertyChanged(nameof(this.ShowCard));
            }
        }
        public void SetData(List<Field> fields)
        {
            this.SourceData.fields = fields;
            this.Source.SetClassData(this.SourceData);
        }
    }
}
