using Incas.Core.ViewModels;
using Incas.Objects.Models;
using System.Collections.Generic;

namespace Incas.Objects.ViewModels
{
    public class CustomDatabaseViewModel : BaseViewModel
    {
        private string selectedCategory;
        private Class selectedClass;
        public delegate void SelectedClassDelegate(Class selectedClass);
        public event SelectedClassDelegate OnClassSelected;
        public CustomDatabaseViewModel() { }
        public List<string> Categories
        {
            get
            {
                using Class cl = new();
                return cl.GetCategories();
            }
        }
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
                using (Class cl = new())
                {
                    return cl.GetClassesByCategory(this.SelectedCategory);
                }
            }
        }
        public Class SelectedClass
        {
            get
            {
                return this.selectedClass;
            }
            set
            {
                this.selectedClass = value;                
                this.OnPropertyChanged(nameof(this.SelectedClass));
                this.OnPropertyChanged(nameof(this.SelectedClassName));
                this.OnClassSelected?.Invoke(value);
            }
        }
        public string SelectedClassName
        {
            get
            {
                if (this.SelectedClass == null)
                {
                    return "(класс не выбран)";
                }
                return this.SelectedClass.name;
            }
        }
    }
}
