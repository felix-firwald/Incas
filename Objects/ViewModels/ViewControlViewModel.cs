using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Core.Views.Windows;
using Incas.Objects.AutoUI;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            this.OpenSettings = new Command(this.DoOpenSettings);
        }

        private void DoOpenSettings(object obj)
        {
            switch (this.Type)
            {
                case ControlType.Text:
                    ViewControlTextSettings ts = new(this.Source.TextSettings);
                    if (ts.ShowDialog("Настройки текста"))
                    {
                        this.Source.TextSettings = ts.GetResult();
                    }
                    break;
            }
        }

        public ICommand OpenSettings { get; set; }
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
                    case ControlType.Table:
                        return true;
                }
                return false;
            }
        }
        
        public Visibility ContainerSettingsVisibility
        {
            get
            {
                switch (this.Type)
                {
                    case ControlType.Table:
                    case ControlType.FieldFiller:
                    case ControlType.Button:
                    case ControlType.Text:
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
        public Visibility ButtonSettingsVisibility
        {
            get
            {
                switch (this.Type)
                {
                    case ControlType.Text:
                        return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        public void AddChild(ViewControlViewModel vc)
        {
            if (this.Type is ControlType.Button)
            {
                return;
            }
            if (this.Type is ControlType.Table && vc.Type is not ControlType.Button)
            {
                return;
            }
            if (!this.SupportsMultipleChildren)
            { 
                this.Children?.Clear();
            }
            if (this.Children is null)
            {
                this.Children = new();
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
        public ViewControlViewModel FindParent(ViewControlViewModel vc)
        {
            foreach (ViewControlViewModel vm in this.Children)
            {
                if (vm.Children is not null)
                {
                    if (vm.Children.Contains(vc))
                    {
                        return vm;
                    }
                    else
                    {
                        ViewControlViewModel result = vm.FindParent(vc);
                        if (result is not null)
                        {
                            return result;
                        }
                    }
                }
            }
            return null;
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
