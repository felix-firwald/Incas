using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.Core.ViewModels;
using Incas.Users.Models;
using Mono.Unix;
using System.Data;
using System.Collections.Generic;

namespace Incas.Admin.ViewModels
{
    public class WorkspaceParametersViewModel : BaseViewModel
    {
        public WorkspaceParametersViewModel()
        {

        }

        public DataTable Constants
        {
            get
            {
                using (Parameter p = new())
                {
                    return p.GetConstants();
                }
            }
        }
        public DataTable Enumerations
        {
            get
            {
                using (Parameter p = new())
                {
                    return p.GetEnumerators();
                }
            }
        }
        private DataRow selectedConstant;
        public DataRow SelectedConstant
        {
            get
            {
                return this.selectedConstant;
            }
            set
            {
                this.selectedConstant = value;
                this.OnPropertyChanged(nameof(this.SelectedConstant));
            }
        }
        private string selectedEnumeration;
        public string SelectedEnumeration
        {
            get
            {
                return this.selectedEnumeration;
            }
            set
            {
                this.selectedEnumeration = value;
                this.OnPropertyChanged(nameof(this.SelectedEnumeration));
                this.OnPropertyChanged(nameof(this.SelectedEnumValues));
            }
        }

        public string SelectedEnumValues
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.SelectedEnumeration))
                {
                    return "";
                }
                List<string> list = ProgramState.GetEnumeration(this.SelectedEnumeration);
                string result = "";
                int counter = 1;
                foreach (string value in list)
                {
                    result += $"({counter}) {value}\n";
                    counter++;
                }
                //DialogsManager.ShowInfoDialog(result);
                return result;
            }
        }
        public void UpdateConstants()
        {
            this.OnPropertyChanged(nameof(this.Constants));
        }
        public void UpdateEnumerations()
        {
            this.OnPropertyChanged(nameof(this.Enumerations));
        }
    }
}
