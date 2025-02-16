using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Models;
using System.Windows;

namespace Incas.Objects.ViewModels
{
    public class FieldViewModel : BaseViewModel
    {
        public Field Source;
        public FieldViewModel(Field field)
        {
            this.Source = field;
        }
        public FieldViewModel()
        {
            this.Source = new();
        }
        public string VisibleName
        {
            get => this.Source.VisibleName;
            set
            {
                this.Source.VisibleName = value;
                this.OnPropertyChanged(nameof(this.VisibleName));
            }
        }
        public bool BelongsThisClass
        {
            get
            {
                return this.Source.Owner is null;
            }
        }
        public Visibility BelongsVisibility
        {
            get
            {
                return this.FromBool(this.BelongsThisClass);
            }
        }

        public string SelectedValue
        {
            get => this.Source.Value;
            set
            {
                this.Source.Value = value;
                this.OnPropertyChanged(nameof(this.SelectedValue));
            }
        }

        public string Name
        {
            get => this.Source.Name;
            set
            {
                if (value != this.Source.Name)
                {
                    this.Source.Name = value
                        .Replace(" ", "_")
                        .Replace(".", "_")
                        .Replace(":", "_")
                        .Replace("$", "_");
                    this.OnPropertyChanged(nameof(this.Name));
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
                return this.Source.Type;
            }
            set
            {
                this.Source.Type = value;
                this.OnPropertyChanged(nameof(this.Type));
            }
        }
        public bool ListVisibility
        {
            get
            {
                return this.Source.ListVisibility;
            }
            set
            {
                this.Source.ListVisibility = value;
                this.OnPropertyChanged(nameof(this.ListVisibility));
            }
        }
        public bool IsUnique
        {
            get
            {
                return this.Source.IsUnique;
            }
            set
            {
                this.Source.IsUnique = value;
                this.OnPropertyChanged(nameof(this.IsUnique));
            }
        }
        public bool ActionBindingEnabled
        {
            get
            {
                switch (this.Source.Type)
                {
                    case FieldType.LocalConstant:
                    case FieldType.GlobalConstant:
                    case FieldType.HiddenField:
                        return false;
                    default:
                        return true;
                }
            }
        }
        public bool EventBindingEnabled
        {
            get
            {
                switch (this.Source.Type)
                {
                    case FieldType.LocalConstant:
                    case FieldType.GlobalConstant:
                        return false;
                    default:
                        return true;
                }
            }
        }
        #region Not Standart Properties
    }
}
#endregion