using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Incas.Objects.ViewModels
{
    public class BindingConstraintViewModel : BaseViewModel
    {
        public static List<ConstraintValue.ConstraintValueType> AvailableTypes
        {
            get
            {
                return new() { ConstraintValue.ConstraintValueType.ByFixedValue, ConstraintValue.ConstraintValueType.ByField };
            }
        }
        public BindingConstraintViewModel(KeyValuePair<Guid, ConstraintValue> pair, ObservableCollection<FieldViewModel> myClassFields, List<Field> targetClassFields)
        {
            this.MyFields = myClassFields;
            this.TargetClassFields = targetClassFields;
            this.ConstraintType = pair.Value.Type;
            if (pair.Key == Guid.Empty)
            {
                return;
            }
            foreach (Field f in targetClassFields)
            {
                if (pair.Key == f.Id)
                {
                    this.ToField = f;
                    break;
                }
            }
            switch (this.ConstraintType)
            {
                case ConstraintValue.ConstraintValueType.ByFixedValue:
                    this.FromValue = pair.Value.Value;
                    break;
                case ConstraintValue.ConstraintValueType.ByField:
                    foreach (FieldViewModel f in myClassFields)
                    {
                        if (pair.Value.TargetField == f.Source.Id)
                        {
                            this.FromMyField = f;
                            break;
                        }
                    }
                    break;
            }            
        }
        public ConstraintValue GetConstraint()
        {
            ConstraintValue cv = new();
            cv.Type = this.ConstraintType;
            switch (cv.Type)
            {
                case ConstraintValue.ConstraintValueType.ByFixedValue:
                    cv.Value = this.FromValue;
                    break;
                case ConstraintValue.ConstraintValueType.ByField:
                    cv.TargetField = this.FromMyField.Source.Id;
                    break;
            }
            return cv;
        }
        private ConstraintValue.ConstraintValueType constraintType;
        public ConstraintValue.ConstraintValueType ConstraintType
        {
            get
            {
                return this.constraintType;
            }
            set
            {
                this.constraintType = value;
                this.OnPropertyChanged(nameof(this.ConstraintType));
                this.OnPropertyChanged(nameof(this.FromValueVisibility));
                this.OnPropertyChanged(nameof(this.FromFieldVisibility));
            }
        }
        private List<Field> targetClassFields;
        public List<Field> TargetClassFields
        {
            get
            {
                return this.targetClassFields;
            }
            set
            {
                this.targetClassFields = value;
                this.OnPropertyChanged(nameof(this.TargetClassFields));
            }
        }
        private ObservableCollection<FieldViewModel> myFields;
        public ObservableCollection<FieldViewModel> MyFields
        {
            get
            {
                return this.myFields;
            }
            set
            {
                this.myFields = value;
                this.OnPropertyChanged(nameof(this.MyFields));
            }
        }
        private FieldViewModel fromField;
        public FieldViewModel FromMyField
        {
            get
            {
                return this.fromField;
            }
            set
            {
                this.fromField = value;
                this.OnPropertyChanged(nameof(this.FromMyField));
            }
        }
        private Field toField;
        

        public Field ToField
        {
            get
            {
                return this.toField;
            }
            set
            {
                this.toField = value;
                this.OnPropertyChanged(nameof(this.ToField));
            }
        }
        private string fromValue;
        public string FromValue
        {
            get
            {
                return this.fromValue;
            }
            set
            {
                this.fromValue = value;
                this.OnPropertyChanged(nameof(this.FromValue));
            }
        }
        public Visibility FromFieldVisibility
        {
            get
            {
                switch (this.ConstraintType)
                {
                    case ConstraintValue.ConstraintValueType.ByField:
                        return Visibility.Visible;
                } 
                return Visibility.Collapsed;
            }
        }
        public Visibility FromValueVisibility
        {
            get
            {
                switch (this.ConstraintType)
                {
                    case ConstraintValue.ConstraintValueType.ByFixedValue:
                        return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
    }
    public class DialogBindingViewModel : BaseViewModel
    {
        private ObservableCollection<FieldViewModel> myClassFields;
        private Field selectedField;
        
        public DialogBindingViewModel(ObservableCollection<FieldViewModel> fields, BindingData data)
        {
            this.myClassFields = fields;
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
            this.Constraints = new();
            if (data.Compliance is not null && data.Compliance.Count > 0)
            {                
                foreach (KeyValuePair<Guid,ConstraintValue> pair in data.Compliance)
                {
                    this.AddConstraint(pair, this.myClassFields);
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
        private ObservableCollection<BindingConstraintViewModel> constraints;
        public ObservableCollection<BindingConstraintViewModel> Constraints
        {
            get
            {
                return this.constraints;
            }
            set
            {
                this.constraints = value;
                this.OnPropertyChanged(nameof(this.Constraints));
            }
        }
        public void AddConstraint(KeyValuePair<Guid, ConstraintValue> pair, ObservableCollection<FieldViewModel> myClassFields)
        {
            BindingConstraintViewModel vm = new(pair, myClassFields, this.Fields);
            this.Constraints.Add(vm);
        }
        public void AddConstraint()
        {
            this.AddConstraint(new KeyValuePair<Guid, ConstraintValue>(), this.myClassFields);
        }
        public Dictionary<Guid, ConstraintValue> GetConstraintsForSave()
        {
            Dictionary<Guid, ConstraintValue> result = new();
            foreach (BindingConstraintViewModel vm in this.Constraints)
            {
                result.Add(vm.ToField.Id, vm.GetConstraint());
            }
            return result;
        }
    }
}
