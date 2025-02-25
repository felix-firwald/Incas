using Incas.Core.Classes;
using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Incas.Objects.ViewModels
{
    public class CustomDatabaseViewModel : BaseViewModel
    {
        private WorkspaceComponent selectedCategory;
        private Class selectedClass;
        public delegate void SelectedClassDelegate(Class selectedClass);
        public event SelectedClassDelegate OnClassSelected;

        public CustomDatabaseViewModel()
        {
            ProgramState.DatabasePage = this;
        }
        public List<WorkspaceComponent> Categories
        {
            get
            {
                return ProgramState.CurrentWorkspace.GetDefinition().Components;
            }
        }
        public string CategoryName => this.selectedCategory.Name;
        public void UpdateAll()
        {
            this.OnPropertyChanged(nameof(this.Categories));
            this.OnPropertyChanged(nameof(this.SelectedCategory));
            this.OnPropertyChanged(nameof(this.Classes));
            this.OnPropertyChanged(nameof(this.SelectedClass));
        }
        public WorkspaceComponent SelectedCategory
        {
            get => this.selectedCategory;
            set
            {
                this.selectedCategory = value;
                this.OnPropertyChanged(nameof(this.SelectedCategory));
                this.OnPropertyChanged(nameof(this.Classes));
            }
        }
        public List<Class> Classes
        {
            get
            {
                using Class cl = new();
                return cl.GetClassesByComponent(this.SelectedCategory);
            }
        }
        public Class SelectedClass
        {
            get => this.selectedClass;
            set
            {
                this.selectedClass = value;
                this.OnPropertyChanged(nameof(this.SelectedClass));
                this.OnPropertyChanged(nameof(this.SelectedClassName));
                if (this.selectedClass != null)
                {
                    this.classData = this.selectedClass.GetClassData();
                    this.OnPropertyChanged(nameof(this.ClassData));
                    this.OnPropertyChanged(nameof(this.PresetsVisibility));
                    this.OnClassSelected?.Invoke(value);
                }              
            }
        }
        private IClassData classData;
        public IClassData ClassData
        {
            get
            {
                return this.classData;
            }
        }
        public Visibility PresetsVisibility
        {
            get
            {
                if (this.classData is null)
                {
                    return Visibility.Collapsed;
                }
                return this.ClassData.PresetsEnabled ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public string SelectedClassName 
        {
            get
            {
                return this.SelectedClass == null ? "(класс не выбран)" : this.SelectedClass.GetClassData().ListName;
            }
        }
    }
}
