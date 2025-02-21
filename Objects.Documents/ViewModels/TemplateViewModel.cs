using Incas.Core.ViewModels;
using Incas.Objects.ViewModels;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Documents.ViewModels
{
    public class TemplateViewModel : BaseViewModel
    {
        public delegate List<Field> PropertyAccessToFields();
        public event PropertyAccessToFields OnFieldsRequested;

        public ClassViewModel ClassViewModel { get; set; }
        public Template Source;
        public TemplateViewModel(ClassViewModel cvm, Template field)
        {
            this.Source = field;
            this.ClassViewModel = cvm;
            this.Properties = new();
            if (field.Properties is not null)
            {
                foreach (TemplateProperty property in field.Properties)
                {
                    this.AddProperty(property);
                }
            }            
        }
        public void AddProperty(TemplateProperty property)
        {
            PropertyViewModel propVM = new(property);
            propVM.OnRemoveRequested += this.PropVM_OnRemoveRequested;
            propVM.OnMoveUpRequested += this.PropVM_OnMoveUpRequested;
            propVM.OnMoveDownRequested += this.PropVM_OnMoveDownRequested;
            propVM.OnFieldsRequested += this.PropVM_OnFieldsRequested;
            this.Properties.Add(propVM);
        }

        private List<Field> PropVM_OnFieldsRequested()
        {
            return this.ClassViewModel.GetFields();
        }

        private void PropVM_OnMoveDownRequested(PropertyViewModel sender)
        {
            this.properties.MoveDown(sender);
        }

        private void PropVM_OnMoveUpRequested(PropertyViewModel sender)
        {
            this.properties.MoveUp(sender);
        }

        private void PropVM_OnRemoveRequested(PropertyViewModel sender)
        {
            this.properties.Remove(sender);
        }

        public string Name
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public string FilePath
        {
            get
            {
                return this.Source.File;
            }
            set
            {
                this.Source.File = value;
                this.OnPropertyChanged(nameof(this.FilePath));
                this.OnPropertyChanged(nameof(this.TemplatePathSelected));
            }
        }
        public bool TemplatePathSelected
        {
            get
            {
                if (string.IsNullOrEmpty(this.FilePath))
                {
                    return false;
                }
                return true;
            }
        }
        public bool RecursiveStreamingEnabled
        {
            get
            {
                return this.Source.StreamingEnabled;
            }
            set
            {
                this.Source.StreamingEnabled = value;
                this.OnPropertyChanged(nameof(this.RecursiveStreamingEnabled));
            }
        }
        private ObservableCollection<PropertyViewModel> properties { get; set; }
        public ObservableCollection<PropertyViewModel> Properties
        {
            get
            {
                return this.properties;
            }
            set
            {
                this.properties = value;
                this.OnPropertyChanged(nameof(this.Properties));
            }
        }
        public void Save()
        {
            this.Source.Properties = new();
            if (this.Name is null)
            {
                throw new FieldDataFailed("Одному из шаблонов не присвоено имя.");
            }
            if (this.FilePath is null)
            {
                throw new FieldDataFailed($"Шаблон [{this.Name}] не имеет привязанного файла.");
            }
            foreach (PropertyViewModel vm in this.Properties)
            {
                TemplateProperty prop = vm.Source;
                if (string.IsNullOrEmpty(prop.Name))
                {
                    throw new FieldDataFailed($"Одному из свойств шаблона [{this.Name}] не присвоено имя.");
                }
                if (prop.Type == TemplateProperty.CalculationType.Switch && prop.Switcher is null)
                {
                    throw new FieldDataFailed($"Свойство [{prop.Name}] шаблона [{this.Name}] не настроено.");
                }
                this.Source.Properties.Add(prop);
            }           
        }
    }
}
