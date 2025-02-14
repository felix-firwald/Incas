using Incas.Core.ViewModels;
using IncasEngine.Core.ExtensionMethods;
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
        public Template Source;
        public TemplateViewModel(Template field)
        {
            this.Source = field;
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
            this.Properties.Add(propVM);
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
            foreach (PropertyViewModel vm in this.Properties)
            {
                this.Source.Properties.Add(vm.Source);
            }           
        }
    }
}
