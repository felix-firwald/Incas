using Incas.Core.ViewModels;
using Incas.Objects.ViewModels;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.Structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Admin.ViewModels
{
    public class StructureViewModel : BaseViewModel
    {
        public Structure Source { get; set; }
        public StructureData SourceData { get; set; }
        public StructureViewModel()
        {
            this.Source = new();
            this.SourceData = new();
        }
        public StructureViewModel(Structure structure)
        {
            this.Source = structure;
            this.SourceData = structure.Data;
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
        private ObservableCollection<FieldViewModel> fields;
        public ObservableCollection<FieldViewModel> Fields
        {
            get
            {
                return this.fields;
            }
            set
            {
                this.fields = value;
                this.OnPropertyChanged(nameof(this.Fields));
            }
        }
        private static List<FieldType> fieldTypes = new()
        {
            FieldType.String,
            FieldType.Text,
            FieldType.Boolean,
            FieldType.Integer,
            FieldType.Float,
            FieldType.Date,
            FieldType.LocalEnumeration,
            FieldType.GlobalEnumeration,
        };
        public List<FieldType> FieldTypes
        {
            get
            {
                return fieldTypes;
            }
        }
    }
}
