using Incas.Core.ViewModels;
using Incas.Objects.Documents.AutoUI;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Incas.Objects.Documents.ViewModels
{
    public class PropertyViewModel : BaseViewModel
    {
        public delegate void PropertyAction(PropertyViewModel sender);
        public delegate List<Field> PropertyAccessToFields();
        
        public event PropertyAction OnRemoveRequested;
        public event PropertyAction OnMoveUpRequested;
        public event PropertyAction OnMoveDownRequested;
        public event PropertyAction OnOpenSettingsRequested;
        public event PropertyAccessToFields OnFieldsRequested;

        public TemplateProperty Source { get; set; }
        public PropertyViewModel(TemplateProperty prop)
        {
            this.Source = prop;
            if (string.IsNullOrEmpty(prop.Name))
            {
                this.IsExpanded = true;
            }
            this.SetUpCommands();
        }
        private void SetUpCommands()
        {
            this.Remove = new Command(this.DoRemove);
            this.OpenSettings = new Command(this.DoOpenSettings);
            this.MoveUp = new Command(this.DoMoveUp);
            this.MoveDown = new Command(this.DoMoveDown);
        }
        #region Commands
        public ICommand Remove { get; set; }
        public ICommand OpenSettings { get; set; }
        public ICommand MoveUp { get; set; }
        public ICommand MoveDown { get; set; }

        public void DoRemove(object obj)
        {
            this.OnRemoveRequested?.Invoke(this);
        }
        public void DoOpenSettings(object obj)
        {
            switch (this.Source.Type)
            {
                case TemplateProperty.CalculationType.Constant:
                    PropertyConstantSettings settings = new(this.Source);
                    settings.ShowDialog($"Настройки свойства [{this.Source.Name}]");
                    break;
                case TemplateProperty.CalculationType.Switch:
                    PropertySwitcherSettings settings2 = new(this.Source, this.OnFieldsRequested?.Invoke());
                    settings2.ShowDialog($"Настройки свойства [{this.Source.Name}]");
                    break;
            }
        }
        public void DoMoveUp(object obj)
        {
            this.OnMoveUpRequested?.Invoke(this);
        }
        public void DoMoveDown(object obj)
        {
            this.OnMoveDownRequested?.Invoke(this);
        }
        #endregion
        public string PropertyName
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value;
                this.OnPropertyChanged(nameof(this.PropertyName));
            }
        }
        public TemplateProperty.CalculationType Type
        {
            get
            {
                return this.Source.Type;
            }
            set
            {
                this.Source.Type = value;
                this.OnPropertyChanged(nameof(this.Type));
            }
        }
        private bool expanded = false;
        public bool IsExpanded
        {
            get
            {
                return this.expanded;
            }
            set
            {
                this.expanded = value;
                this.OnPropertyChanged(nameof(this.IsExpanded));
            }
        }
        public bool IsBeforeFields
        {
            get
            {
                return this.Source.Order == TemplateProperty.RenderingOrder.BeforeFields;
            }
            set
            {
                this.Source.Order = value == true ? TemplateProperty.RenderingOrder.BeforeFields : TemplateProperty.RenderingOrder.AfterFields;
            }
        }
        public static List<TemplateProperty.CalculationType> PropertyTypes
        {
            get
            {
                return new() 
                { 
                    TemplateProperty.CalculationType.Constant, 
                    TemplateProperty.CalculationType.Switch, 
                    TemplateProperty.CalculationType.Script, 
                    TemplateProperty.CalculationType.Replication 
                };
            }
        }
    }
}
