using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Incas.Objects.ServiceClasses.Groups.ViewModels
{
    public class GroupClassPermissionViewModel : BaseViewModel
    {
        private ClassItem item;
        private GroupClassPermissionSettings settings;
        public GroupClassPermissionSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                this.settings = value;
                this.OnPropertyChanged(nameof(this.Settings));
            }
        }
        public GroupClassPermissionViewModel(ClassItem target)
        {
            this.item = target;
            this.AcceptAll = new Command(this.DoAcceptAll);
            this.RejectAll = new Command(this.DoRejectAll);
        }

        private void DoRejectAll(object obj)
        {
            this.CreateOperations = false;
            this.ReadOperations = false;
            this.ViewOperations = false;
            this.UpdateOperations = false;
            this.DeleteOperations = false;
            this.ConfidentialAccess = false;
        }

        private void DoAcceptAll(object obj)
        {
            this.CreateOperations = true;
            this.ReadOperations = true;
            this.ViewOperations = true;
            this.UpdateOperations = true;
            this.DeleteOperations = true;
            this.ConfidentialAccess = true;
        }

        public ICommand AcceptAll { get; set; }
        public ICommand RejectAll { get; set; }
        public ClassItem Item
        {
            get
            {
                return this.item;
            }
            set
            {
                this.item = value;
                this.OnPropertyChanged(nameof(this.Item));
            }
        }
        public bool CreateOperations
        {
            get
            {
                return this.settings.CreateOperations;
            }
            set
            {
                this.settings.CreateOperations = value;
                this.OnPropertyChanged(nameof(this.CreateOperations));
            }
        }
        public bool ViewOperations
        {
            get
            {
                return this.settings.ViewOperations;
            }
            set
            {
                this.settings.ViewOperations = value;
                this.OnPropertyChanged(nameof(this.ViewOperations));
            }
        }
        public bool ReadOperations
        {
            get
            {
                return this.settings.ReadOperations;
            }
            set
            {
                this.settings.ReadOperations = value;
                this.OnPropertyChanged(nameof(this.ReadOperations));
            }
        }
        public bool UpdateOperations
        {
            get
            {
                return this.settings.UpdateOperations;
            }
            set
            {
                this.settings.UpdateOperations = value;
                this.OnPropertyChanged(nameof(this.UpdateOperations));
            }
        }
        public bool DeleteOperations
        {
            get
            {
                return this.settings.DeleteOperations;
            }
            set
            {
                this.settings.DeleteOperations = value;
                this.OnPropertyChanged(nameof(this.DeleteOperations));
            }
        }
        public bool ConfidentialAccess
        {
            get
            {
                return this.settings.ConfidentialAccess;
            }
            set
            {
                this.settings.ConfidentialAccess = value;
                this.OnPropertyChanged(nameof(this.ConfidentialAccess));
            }
        }
        public GroupClassPermissionSettings GetResult()
        {
            GroupClassPermissionSettings result = new()
            {
                CreateOperations = this.settings.CreateOperations,
                ViewOperations = this.settings.ViewOperations,
                ReadOperations = this.settings.ReadOperations,
                UpdateOperations = this.settings.UpdateOperations,
                DeleteOperations = this.settings.DeleteOperations,
                ConfidentialAccess = this.settings.ConfidentialAccess
            };
            return result;
        }
    }
}
