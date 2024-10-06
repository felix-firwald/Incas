using Incas.Core.ViewModels;
using Incas.Objects.Components;
using Incas.Objects.Models;
using System.Collections.Generic;
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
            get => this.SourceData.ClassType;
            set
            {
                this.SourceData.ClassType = value;
                this.OnPropertyChanged(nameof(this.Type));
            }
        }
        public string NameOfClass
        {
            get => this.Source.name;
            set
            {
                this.Source.name = value;
                this.OnPropertyChanged(nameof(this.NameOfClass));
            }
        }
        public string CategoryOfClass
        {
            get => this.Source.category;
            set
            {
                this.Source.category = value;
                this.OnPropertyChanged(nameof(this.CategoryOfClass));
            }
        }
        public string NameTemplate
        {
            get => this.SourceData.NameTemplate;
            set
            {
                this.SourceData.NameTemplate = value;
                this.OnPropertyChanged(nameof(this.NameTemplate));
            }
        }
        public bool ShowCard
        {
            get => this.SourceData.ShowCard;
            set
            {
                this.SourceData.ShowCard = value;
                this.OnPropertyChanged(nameof(this.ShowCard));
            }
        }
        public bool EditByAuthorOnly
        {
            get => this.SourceData.EditByAuthorOnly;
            set
            {
                this.SourceData.EditByAuthorOnly = value;
                this.OnPropertyChanged(nameof(this.EditByAuthorOnly));
            }
        }
        public void SetData(List<Field> fields)
        {
            this.SourceData.Fields = fields;
            this.Source.SetClassData(this.SourceData);
        }
    }
}
