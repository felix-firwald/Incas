using Incas.Core.ViewModels;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Incas.Objects.ViewModels
{
    public class ViewControlViewModel : BaseViewModel
    {
        public delegate void ControlAction(ViewControlViewModel vm);
        public delegate void Update();
        public event Update OnDrawCalling;
        public event ControlAction OnRemoveRequested;
        public event ControlAction OnMoveDownRequested;
        public event ControlAction OnMoveUpRequested;
        public ViewControl Source { get; set; }
        public ViewControlViewModel(ViewControl control)
        {
            this.Source = control;
            if (control.Children is not null)
            {
                this.Children = new();
                foreach (ViewControl vc in control.Children)
                {
                    this.AddChild(new(vc));
                }
            }           
        }
        private bool selected;
        public bool IsSelected
        {
            get
            {
                return this.selected;
            }
            set
            {
                this.selected = value;
                this.OnPropertyChanged(nameof(this.IsSelected));
            }
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
                this.OnDrawCalling?.Invoke();
            }
        }
        private ObservableCollection<ViewControlViewModel> children;
        public ObservableCollection<ViewControlViewModel> Children
        {
            get
            {
                return this.children;
            }
            set
            {
                this.children = value;
                this.OnPropertyChanged(nameof(this.Children));
            }
        }
        public ControlType Type
        {
            get
            {
                return this.Source.Type;
            }
            set
            {
                this.Source.Type = value;
                this.OnPropertyChanged(nameof(this.Type));
                this.OnDrawCalling?.Invoke();
            }
        }
        public bool SupportsMultipleChildren
        {
            get
            {
                switch (this.Type)
                {
                    case ControlType.VerticalStack:
                    case ControlType.HorizontalStack:
                    case ControlType.Grid:
                    case ControlType.Tab:
                        return true;
                }
                return false;
            }
        }
        
        public Visibility ContainerSettingsVisibility
        {
            get
            {
                if (this.Type == ControlType.FieldFiller)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }
        public Visibility FieldSettingsVisibility
        {
            get
            {
                if (this.Type == ControlType.FieldFiller)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public void AddChild(ViewControlViewModel vc)
        {
            if (this.Type is ControlType.FieldFiller or ControlType.Button or ControlType.Table)
            {
                return;
            }
            if (!this.SupportsMultipleChildren)
            {
                this.Children.Clear();
            }
            this.Children.Add(vc);
            vc.OnDrawCalling += this.Vc_OnDrawCalling;
            vc.OnRemoveRequested += this.Vc_OnRemoveRequested;
            vc.OnMoveDownRequested += this.Vc_OnMoveDownRequested;
            vc.OnMoveUpRequested += this.Vc_OnMoveUpRequested;
            this.OnDrawCalling?.Invoke();
        }

        private void Vc_OnMoveUpRequested(ViewControlViewModel vm)
        {
            this.Children.MoveUp(vm);
            this.OnDrawCalling?.Invoke();
        }

        private void Vc_OnMoveDownRequested(ViewControlViewModel vm)
        {
            this.Children.MoveDown(vm);
            this.OnDrawCalling?.Invoke();
        }

        private void Vc_OnRemoveRequested(ViewControlViewModel vm)
        {
            this.Children?.Remove(vm);
            this.OnDrawCalling?.Invoke();
        }

        private void Vc_OnDrawCalling()
        {
            this.OnDrawCalling?.Invoke();
        }

        
        public void Save()
        {
            if (this.Children is not null)
            {
                this.Source.Children = new();
                foreach (ViewControlViewModel vm in this.Children)
                {
                    vm.Save();                   
                    this.Source.Children.Add(vm.Source);
                }
            }           
        }
        public void RemoveFromParent()
        {
            this.OnRemoveRequested?.Invoke(this);
        }
        public void RemoveField(Guid field)
        {
            if (this.Children is not null)
            {
                foreach (ViewControlViewModel vm in this.Children)
                {
                    if (vm.Source.Field == field)
                    {
                        this.children.Remove(vm);
                        return;
                    }
                    vm.RemoveField(field);
                }
            }            
        }
        public void RemoveMethod(Guid method)
        {
            if (this.Children is not null)
            {
                foreach (ViewControlViewModel vm in this.Children)
                {
                    if (vm.Source.RunMethod == method)
                    {
                        this.children.Remove(vm);
                        return;
                    }
                    vm.RemoveField(method);
                }
            }
        }
        public void RemoveTable(Guid table)
        {
            if (this.Children is not null)
            {
                foreach (ViewControlViewModel vm in this.Children)
                {
                    if (vm.Source.Table == table)
                    {
                        this.children.Remove(vm);
                        return;
                    }
                    vm.RemoveField(table);
                }
            }
        }
        public void MoveDown()
        {
            this.OnMoveDownRequested?.Invoke(this);
        }
        public void MoveUp()
        {
            this.OnMoveUpRequested?.Invoke(this);
        }
    }
}
