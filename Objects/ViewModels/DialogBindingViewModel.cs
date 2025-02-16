using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;

namespace Incas.Objects.ViewModels
{
    public class DialogBindingViewModel : BaseViewModel
    {        
        private Field selectedField;
        
        public DialogBindingViewModel(BindingData data)
        {
            using (Class cl = new())
            {
                this.classes = cl.GetAllClassItems();
            }
            foreach (ClassItem cl in this.Classes)
            {
                if (cl.Id == data.BindingClass)
                {
                    this.SelectedClass = cl;
                    break;
                }
            }
            this.OnPropertyChanged(nameof(this.Fields));           
            if (this.selectedClass.Id != Guid.Empty)
            {
                foreach (Field f in this.Fields)
                {
                    if (f.Id == data.BindingField)
                    {
                        this.BindingField = f;
                        break;
                    }
                }
            }
        }
        private List<ClassItem> classes;
        public List<ClassItem> Classes => this.classes;

        private List<Field> fields;
        public List<Field> Fields
        {
            get 
            {
                return this.fields;
            }
            set
            {
                this.fields = value;
                this.OnPropertyChanged(nameof(this.Fields));
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
                this.Fields = (new Class(this.SelectedClass.Id).GetClassData() as ClassDataBase).GetBindableFields();
                this.OnPropertyChanged(nameof(this.BindingField));
            }
        }
        public Field BindingField
        {
            get
            {
                return this.selectedField;
            }
            set
            {
                this.selectedField = value;
                this.OnPropertyChanged(nameof(this.BindingField));
            }
        }
    }
}
