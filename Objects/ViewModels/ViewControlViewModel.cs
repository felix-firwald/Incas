using Incas.Core.ViewModels;
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
        public delegate void Remove(ViewControlViewModel vm);
        public delegate void Update();
        public event Update OnDrawCalling;
        public event Remove OnRemoveRequested;
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
        public void AddChild(ViewControlViewModel vc)
        {
            if (this.Type is ControlType.FieldFiller)
            {
                return;
            }
            this.Children.Add(vc);
            vc.OnDrawCalling += this.Vc_OnDrawCalling;
            vc.OnRemoveRequested += this.Vc_OnRemoveRequested;
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
            //if (this == vm)
            //{
            //    this.OnDrawCalling?.Invoke();
            //    return true;
            //}
            //if (this.Children is not null)
            //{
            //    foreach (ViewControlViewModel vm2 in this.Children)
            //    {
            //        if (vm2.RemoveIfExists(vm))
            //        {
            //            this.OnDrawCalling?.Invoke();
            //            return true;
            //        }
            //        if (vm2 == vm)
            //        {
            //            this.Children.Remove(vm2);
            //            this.OnDrawCalling?.Invoke();
            //            return true;
            //        }
            //    }
            //}
            //return false;
        }
    }
}
