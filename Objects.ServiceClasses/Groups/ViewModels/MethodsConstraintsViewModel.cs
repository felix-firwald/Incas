using DocumentFormat.OpenXml.Bibliography;
using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.ServiceClasses.Groups.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class MethodsConstraintsViewModel : BaseViewModel
    {
        public class MethodConstraintInfo : BaseViewModel
        {
            public Method Method { get; private set; }
            private bool restrict = false;
            public bool Restrict
            {
                get
                {
                    return this.restrict;
                }
                set
                {
                    this.restrict = value;
                    this.OnPropertyChanged(nameof(this.Restrict));
                }
            }
            public MethodConstraintInfo(Method m, bool r)
            {
                this.Method = m;
                this.Restrict = r;
            }
        }

        /// <summary>
        /// Model
        /// </summary>
        public GroupClassPermissionSettings Source { get; set; }

        public MethodsConstraintsViewModel(Class cl, GroupClassPermissionSettings source)
        {
            this.Source = source;
            this.Methods = new();
            if (source.RestrictedMethods == null)
            {
                source.RestrictedMethods = new();
            }
            foreach (Method m in cl.GetClassData().Methods)
            {
                MethodConstraintInfo constraint = new(m, source.RestrictedMethods.Contains(m.Id));
                this.Methods.Add(constraint);
            }          
        }

        private List<MethodConstraintInfo> methods;
        public List<MethodConstraintInfo> Methods
        {
            get
            {
                return this.methods;
            }
            set
            {
                this.methods = value;
                this.OnPropertyChanged(nameof(this.Methods));
            }
        }

        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>
        public void Save()
        {
            this.Source.RestrictedMethods = new();
            foreach (MethodConstraintInfo methodElement in this.Methods)
            {
                if (methodElement.Restrict)
                {
                    this.Source.RestrictedMethods.Add(methodElement.Method.Id);
                }
            }
        }
    }
}
