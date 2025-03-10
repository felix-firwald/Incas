using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.AutoUI;
using IncasEngine.ObjectiveEngine.Classes;
using System.Windows.Input;
using static IncasEngine.ObjectiveEngine.FieldComponents.TableFieldData;

namespace Incas.Objects.ViewModels
{
    public class TableColumnViewModel : BaseViewModel
    {
        public TableFieldColumnData Source { get; set; }
        public delegate void Action(TableColumnViewModel vm);
        public event Action OnMoveDownRequested;
        public event Action OnMoveUpRequested;
        public event Action OnRemoveRequested;
        public TableColumnViewModel(TableFieldColumnData fieldData)
        {
            this.Source = fieldData;
            this.Source.SetId();
            this.OpenFieldSettings = new Command(this.DoOpenFieldSettings);
            this.RemoveField = new Command(this.DoRemoveField);
            this.MoveUpField = new Command(this.DoMoveUpField);
            this.MoveDownField = new Command(this.DoMoveDownField);
        }
        
        #region Commands
        public ICommand OpenFieldSettings { get; set; }
        public ICommand RemoveField { get; set; }
        public ICommand MoveUpField { get; set; }
        public ICommand MoveDownField { get; set; }
        public void DoOpenFieldSettings(object value)
        {
            TableFieldColumnData f = this.Source;
            string name = $"Настройки колонки [{f.Name}]";
            switch (f.FieldType)
            {
                case FieldType.String:
                case FieldType.Text:
                    TextColumnSettings tc = new(f);
                    tc.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Green);
                    break;
                case FieldType.LocalEnumeration:
                    LocalEnumerationColumnSettings le = new(f);
                    le.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case FieldType.GlobalEnumeration:
                    GlobalEnumerationColumnSettings ge = new(f);
                    ge.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
            }
        }
        private void DoRemoveField(object obj)
        {
            this.OnRemoveRequested?.Invoke(this);
        }
        private void DoMoveUpField(object obj)
        {
            this.OnMoveUpRequested?.Invoke(this);
        }
        private void DoMoveDownField(object obj)
        {
            this.OnMoveDownRequested?.Invoke(this);
        }
        #endregion
        public string VisibleName
        {
            get => this.Source.VisibleName;
            set
            {
                this.Source.VisibleName = value;
                this.OnPropertyChanged(nameof(this.VisibleName));
            }
        }
        public string InternalName
        {
            get => this.Source.Name;
            set
            {
                if (value != this.Source.Name)
                {
                    this.Source.Name = value
                        .Replace(" ", "_")
                        .Replace(".", "_")
                        .Replace("$", "_");
                    this.OnPropertyChanged(nameof(this.InternalName));
                }
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
        public FieldType Type
        {
            get
            {
                return this.Source.FieldType;
            }
            set
            {
                this.Source.FieldType = value;
                this.OnPropertyChanged(nameof(this.Type));
            }
        }
    }
}
