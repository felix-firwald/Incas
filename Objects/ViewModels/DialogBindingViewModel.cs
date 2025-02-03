using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Models;
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

        public List<Class> Classes => this.classes;
        public List<Field> Fields => this.SelectedClass == null ? ([]) : this.SelectedClass.GetClassData().GetBindableFields();
        public Class SelectedClass
        {
            get
            {
                foreach (Class cl in this.classes)
                {
                    if (cl.Id == this.selectedClass)
                    {
                        return cl;
                    }
                }
                return null;
            }
            set
            {
                this.selectedClass = value.Id;
                this.OnPropertyChanged(nameof(this.SelectedClass));
                this.OnPropertyChanged(nameof(this.Fields));
                this.OnPropertyChanged(nameof(this.SelectedField));
            }
        }
        public Field SelectedField
        {
            get
            {
                if (this.SelectedClass is null)
                {
                    return null;
                }
                foreach (Field f in this.Fields)
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
