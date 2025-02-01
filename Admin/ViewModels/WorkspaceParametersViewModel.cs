using Incas.Core.Classes;
using Incas.Core.Models;
using Incas.Core.ViewModels;
using Incas.Objects.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Incas.Admin.ViewModels
{
    public class WorkspaceParametersViewModel : BaseViewModel
    {
        public WorkspaceParametersViewModel()
        {

        }
        public bool EditingWorkspaceEnabled
        {
            get
            {
                return ProgramState.CurrentWorkspace.CurrentGroup.Data.GeneralSettingsEditing;
            }
        }
        public Visibility CreatingClassVisibility
        {
            get
            {
                return this.FromBool(ProgramState.CurrentWorkspace.CurrentGroup.Data.CreatingClasses);
            }
        }
        public Visibility UpdatingClassVisibility
        {
            get
            {
                return this.FromBool(ProgramState.CurrentWorkspace.CurrentGroup.Data.UpdatingClasses);
            }
        }
        public Visibility RemovingClassVisibility
        {
            get
            {
                return this.FromBool(ProgramState.CurrentWorkspace.CurrentGroup.Data.RemovingClasses);
            }
        }

        public DataTable Constants
        {
            get
            {
                using Parameter p = new();
                return p.GetConstants();
            }
        }
        public DataTable Enumerations
        {
            get
            {
                using Parameter p = new();
                return p.GetEnumerators();
            }
        }
        private DataRow selectedConstant;
        public DataRow SelectedConstant
        {
            get => this.selectedConstant;
            set
            {
                this.selectedConstant = value;
                this.OnPropertyChanged(nameof(this.SelectedConstant));
            }
        }
        private DataRow selectedEnumeration;
        public DataRow SelectedEnumeration
        {
            get => this.selectedEnumeration;
            set
            {
                this.selectedEnumeration = value;
                this.OnPropertyChanged(nameof(this.SelectedEnumeration));
                this.OnPropertyChanged(nameof(this.SelectedEnumerationName));
                this.OnPropertyChanged(nameof(this.SelectedEnumValues));
            }
        }
        public string SelectedEnumerationName => this.selectedEnumeration == null ? "(не выбрано)" : this.SelectedEnumeration["Наименование перечисления"].ToString();
        private string enumvals;
        public string SelectedEnumValues
        {
            get
            {
                if (this.selectedEnumeration == null)
                {
                    return "";
                }
                string source = this.SelectedEnumeration["Идентификатор"].ToString();
                List<string> list = ProgramState.GetEnumeration(Guid.Parse(source));
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
            set
            {
                this.enumvals = value;
                this.OnPropertyChanged(nameof(this.SelectedEnumValues));
            }
        }
        public DataTable Classes
        {
            get
            {
                using Class cl = new();
                return cl.GetAllClassesAsDataTable();
            }
        }
        public void UpdateClasses()
        {
            this.OnPropertyChanged(nameof(this.Classes));
        }
        //private DataRow selectedRow;
        //public DataRow SelectedClass
        //{
        //    get
        //    {
        //        return this.selectedRow;
        //    }
        //    set
        //    {
        //        this.selectedRow = value;
        //        this.OnPropertyChanged(nameof(this.SelectedClass));
        //    }
        //}

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
