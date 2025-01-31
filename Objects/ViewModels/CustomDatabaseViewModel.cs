using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Components;
using Incas.Objects.Engine;
using Incas.Objects.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Incas.Objects.ViewModels
{
    public class CustomDatabaseViewModel : BaseViewModel
    {
        private string selectedCategory;
        private Class selectedClass;
        public delegate void SelectedClassDelegate(Class selectedClass);
        public event SelectedClassDelegate OnClassSelected;
        public delegate void SelectedPresetDelegate(Class selectedClass, PresetReference preset);
        public event SelectedPresetDelegate OnPresetSelected;
        public CustomDatabaseViewModel()
        {
            ProgramState.DatabasePage = this;
        }
        public List<string> Categories
        {
            get
            {
                using Class cl = new();
                return cl.GetCategories();
            }
        }
        public string CategoryName => this.selectedCategory;
        public void UpdateAll()
        {
            this.OnPropertyChanged(nameof(this.Categories));
            this.OnPropertyChanged(nameof(this.SelectedCategory));
            this.OnPropertyChanged(nameof(this.Classes));
            this.OnPropertyChanged(nameof(this.SelectedClass));
        }
        public string SelectedCategory
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
                return cl.GetClassesByCategory(this.SelectedCategory);
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
                    this.OnPropertyChanged(nameof(this.Presets));
                    this.SelectedPreset = new();
                    this.OnClassSelected?.Invoke(value);
                }              
            }
        }
        private ClassData classData;
        public ClassData ClassData
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
        public List<PresetReference> Presets
        {
            get
            {
                if (this.classData is null)
                {
                    return new();
                }
                if (this.classData.PresetsEnabled)
                {
                    return Processor.GetPresetsReferences(this.selectedClass);
                }
                return new();
            }
        }
        private PresetReference selectedPreset;
        public PresetReference SelectedPreset
        {
            get
            {
                return this.selectedPreset;
            }
            set
            {
                this.selectedPreset = value;
                this.OnPropertyChanged(nameof(this.SelectedPreset));
                if (this.selectedPreset.Id != Guid.Empty)
                {
                    this.OnPresetSelected?.Invoke(this.selectedClass, this.selectedPreset);                   
                }
                this.OnPropertyChanged(nameof(this.SelectedClassName));
            }
        }
        public string SelectedClassName 
        {
            get
            {
                if (this.selectedPreset.Id == Guid.Empty)
                {
                    return this.SelectedClass == null ? "(класс не выбран)" : this.SelectedClass.Name;
                }
                else
                {
                    return this.SelectedClass == null ? "(класс не выбран)" : this.SelectedClass.Name + ": " + this.selectedPreset.Name;
                }
            }
        }
        public void UpdatePresets()
        {
            this.OnPropertyChanged(nameof(this.Presets));
        }
    }
}
