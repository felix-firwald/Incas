using Incas.Core.ViewModels;
using Incas.Objects.Components;
using Incas.Objects.Models;
using System;
using System.Collections.Generic;

namespace Incas.Objects.ViewModels
{
    public class DialogBindingViewModel : BaseViewModel
    {
        private Guid selectedClass;
        private Guid selectedField;
        private List<Class> classes;
        public DialogBindingViewModel(BindingData data)
        {
            this.selectedClass = data.Class;
            this.selectedField = data.Field;
            using (Class cl = new())
            {
                this.classes = cl.GetAllClasses();
            }
            this.OnPropertyChanged(nameof(this.SelectedClass));
            this.OnPropertyChanged(nameof(this.Fields));
            this.OnPropertyChanged(nameof(this.SelectedField));
        }
        
        public List<Class> Classes
        {
            get
            {
                return this.classes;
            }
        }
        public List<Objects.Models.Field> Fields
        {
            get
            {
                if (this.SelectedClass == null)
                {
                    return new();
                }
                return this.SelectedClass.GetClassData().Fields;
            }
        }
        public Class SelectedClass
        {
            get
            {
                foreach (Class cl in this.classes)
                {
                    if (cl.identifier == this.selectedClass)
                    {
                        return cl;
                    }
                }
                return null;
            }
            set
            {
                this.selectedClass = value.identifier;
                this.OnPropertyChanged(nameof(this.SelectedClass));
                this.OnPropertyChanged(nameof(this.Fields));
                this.OnPropertyChanged(nameof(this.SelectedField));
            }
        }
        public Objects.Models.Field SelectedField
        {
            get
            {
                if (this.SelectedClass is null)
                {
                    return null;
                }
                foreach (Objects.Models.Field f in this.Fields)
                {
                    if (f.Id == this.selectedField)
                    {
                        return f;
                    }
                }
                return null;
            }
            set
            {
                if (value is not null)
                {
                    this.selectedField = value.Id;
                }             
                this.OnPropertyChanged(nameof(this.SelectedField));
            }
        }
    }
}
