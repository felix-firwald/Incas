using Incas.Core.Classes;
using Incas.Core.ViewModels;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

namespace Incas.Objects.ViewModels
{
    public class BindingConstraintViewModel : BaseViewModel
    {
        public static List<ConstraintValue.ConstraintValueType> AvailableTypes => [ConstraintValue.ConstraintValueType.ByFixedValue, ConstraintValue.ConstraintValueType.ByField];
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
            ConstraintValue cv = new()
            {
                Type = this.ConstraintType
            };
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
            get => this.constraintType;
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
            get => this.targetClassFields;
            set
            {
                this.targetClassFields = value;
                this.OnPropertyChanged(nameof(this.TargetClassFields));
            }
        }
        private ObservableCollection<FieldViewModel> myFields;
        public ObservableCollection<FieldViewModel> MyFields
        {
            get => this.myFields;
            set
            {
                this.myFields = value;
                this.OnPropertyChanged(nameof(this.MyFields));
            }
        }
        private FieldViewModel fromField;
        public FieldViewModel FromMyField
        {
            get => this.fromField;
            set
            {
                this.fromField = value;
                this.OnPropertyChanged(nameof(this.FromMyField));
            }
        }
        private Field toField;


        public Field ToField
        {
            get => this.toField;
            set
            {
                this.toField = value;
                this.OnPropertyChanged(nameof(this.ToField));
            }
        }
        private string fromValue;
        public string FromValue
        {
            get => this.fromValue;
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

        public DialogBindingViewModel(ClassViewModel @class, BindingData data)
        {
            if (@class is not null)
            {
                this.myClassFields = @class.Fields;
            }
            
            using (Class cl = new())
            {
                List<WorkspaceComponent> components = new();
                if (@class is null) // if gen
                {
                    components = ProgramState.CurrentWorkspace.GetDefinition().Components;
                }
                else
                {
                    components = ProgramState.CurrentWorkspace.GetDefinition().GetAvailableComponentsFor(@class.Source.Component);
                }
                
                this.classes = new(cl.GetGroupedItems(components));
            }
            foreach (ClassGroupedItem cl in this.Classes)
            {
                if (cl.Id == data.BindingClass.ToString())
                {
                    this.SelectedClass = cl;
                    break;
                }
            }
            this.OnPropertyChanged(nameof(this.Fields));
            if (!string.IsNullOrEmpty(this.selectedClass.Id) && this.selectedClass.Id != Guid.Empty.ToString())
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
            this.Cascade = data.OnDeleteCascade;
            this.Restrict = data.OnDeleteRestrict;
            this.Constraints = [];
            if (data.Compliance is not null && data.Compliance.Count > 0)
            {
                foreach (KeyValuePair<Guid, ConstraintValue> pair in data.Compliance)
                {
                    this.AddConstraint(pair, this.myClassFields);
                }
            }
        }
        private ObservableCollection<ClassGroupedItem> classes;
        public ObservableCollection<ClassGroupedItem> Classes => this.classes;

        private List<Field> fields;
        public List<Field> Fields
        {
            get => this.fields;
            set
            {
                this.fields = value;
                this.OnPropertyChanged(nameof(this.Fields));
            }
        }
        private ClassGroupedItem selectedClass;
        public ClassGroupedItem SelectedClass
        {
            get => this.selectedClass;
            set
            {
                this.selectedClass = value;
                this.OnPropertyChanged(nameof(this.SelectedClass));
                this.Fields = (EngineGlobals.GetClass(Guid.Parse(this.SelectedClass.Id)).GetClassData() as ClassDataBase).GetBindableFields();
                this.OnPropertyChanged(nameof(this.BindingField));
            }
        }
        public Field BindingField
        {
            get => this.selectedField;
            set
            {
                this.selectedField = value;
                this.OnPropertyChanged(nameof(this.BindingField));
            }
        }
        private bool cascade;
        public bool Cascade
        {
            get
            {
                return this.cascade;
            }
            set
            {
                this.cascade = value;
                if (value)
                {
                    this.Restrict = false;
                }
                this.OnPropertyChanged(nameof(this.Cascade));
            }
        }
        private bool restrict;
        public bool Restrict
        {
            get
            {
                return this.restrict;
            }
            set
            {
                this.restrict = value;
                if (value)
                {
                    this.Cascade = false;
                }
                this.OnPropertyChanged(nameof(this.Restrict));
            }
        }
        private ObservableCollection<BindingConstraintViewModel> constraints;
        public ObservableCollection<BindingConstraintViewModel> Constraints
        {
            get => this.constraints;
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
            Dictionary<Guid, ConstraintValue> result = [];
            foreach (BindingConstraintViewModel vm in this.Constraints)
            {
                result.Add(vm.ToField.Id, vm.GetConstraint());
            }
            return result;
        }
    }
}
