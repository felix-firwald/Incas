using Incas.Core.Classes;
using Incas.Core.ViewModels;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Workspace;
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

        public Visibility EditingWorkspaceVisibility
        {
            get
            {
                return this.FromBool(this.EditingWorkspaceEnabled);
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

        public List<ParameterItem> Constants
        {
            get
            {
                using Parameter p = new();
                return p.GetConstants();
            }
        }
        public List<ParameterItem> Enumerations
        {
            get
            {
                using Parameter p = new();
                return p.GetEnumerators();
            }
        }
        private ParameterItem selectedConstant;
        public ParameterItem SelectedConstant
        {
            get => this.selectedConstant;
            set
            {
                this.selectedConstant = value;
                this.OnPropertyChanged(nameof(this.SelectedConstant));
            }
        }
        private ParameterItem selectedEnumeration;
        public ParameterItem SelectedEnumeration
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
        public string SelectedEnumerationName => this.selectedEnumeration.Id == Guid.Empty ? "(не выбрано)" : this.SelectedEnumeration.Name;
        public List<string> SelectedEnumValues
        {
            get
            {
                if (this.selectedEnumeration.Id == Guid.Empty)
                {
                    return new();
                }
                return ProgramState.GetEnumeration(this.SelectedEnumeration.Id);
            }
        }
        public List<WorkspaceComponent> ClassesCategories
        {
            get
            {
                return ProgramState.CurrentWorkspace.CurrentGroup.GetAvailableComponents();
            }
        }
        private WorkspaceComponent selectedCategory;
        public WorkspaceComponent SelectedCategory
        {
            get
            {
                if (this.selectedCategory is null)
                {
                    List<WorkspaceComponent> categories = this.ClassesCategories;
                    if (categories?.Count > 0)
                    {
                        return categories[0];
                    }                   
                }
                return this.selectedCategory;
            }
            set
            {
                this.selectedCategory = value;
                this.OnPropertyChanged(nameof(this.SelectedCategory));
                this.classes = null;
                this.OnPropertyChanged(nameof(this.Classes));
                this.OnPropertyChanged(nameof(this.SelectedClass));
            }
        }
        private List<ClassItem> classes;
        public List<ClassItem> Classes
        {
            get
            {
                if (this.classes == null)
                {
                    using Class cl = new();
                    this.classes = cl.GetAllClassItemsByComponent(this.SelectedCategory);
                }
                return this.classes;
            }
        }
        private ClassItem selectedClass;
        public ClassItem SelectedClass
        {
            get
            {
                return this.selectedClass;
            }
            set
            {
                this.selectedClass = value;
                this.OnPropertyChanged(nameof(this.SelectedClass));
            }
        }
        public void UpdateClasses()
        {
            this.OnPropertyChanged(nameof(this.ClassesCategories));

            this.OnPropertyChanged(nameof(this.SelectedCategory));
            this.classes = null;
            this.OnPropertyChanged(nameof(this.Classes));
            this.SelectedClass = new();
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
            this.OnPropertyChanged(nameof(this.SelectedConstant));
        }
        public void UpdateEnumerations()
        {
            this.OnPropertyChanged(nameof(this.Enumerations));
        }
    }
}
