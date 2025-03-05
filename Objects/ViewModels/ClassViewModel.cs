using ICSharpCode.AvalonEdit.Document;
using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Views.Windows;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Models;
using IncasEngine.Workspace;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Incas.Objects.ViewModels
{
    public class ClassViewModel : BaseViewModel
    {
        public enum ClassMode
        {
            Usual,
            Service
        }
        public ClassMode Mode { get; set; }
        public IClassData SourceData { get; set; }
        public IClass Source;
        public delegate void DrawCall();
        public event DrawCall OnDrawCalling;
        public ClassViewModel(Class source)
        {
            this.Mode = ClassMode.Usual;
            this.Source = source;
            this.SourceData = this.Source.GetClassData();
            this.Fields = new();
            foreach (Field f in this.SourceData.Fields)
            {
                this.Fields.Add(new(f, this));
            }
            this.Fields.CollectionChanged += this.Fields_CollectionChanged;
            this.SetCommands();
            this.ViewControls = new();
            this.AvailableComponents = new(ProgramState.CurrentWorkspace.CurrentGroup.GetAvailableComponents());
            this.SelectedComponent = this.Source.Component;
            this.Methods = new();
            if (this.SourceData.Methods is not null)
            {
                foreach (Method method in this.SourceData.Methods)
                {
                    MethodViewModel vm = new(method);
                    this.Methods.Add(vm);
                }
            }
            if (this.SourceData.EditorView is not null)
            {
                foreach (ViewControl vc in this.SourceData.EditorView.Controls)
                {
                    this.AddNewControlToCustomForm(vc);
                }
            }
        }
#if !E_FREE
        public ClassViewModel(ServiceClass source)
        {
            this.Mode = ClassMode.Service;
            this.Source = source;
            this.SourceData = this.Source.GetClassData();
            this.Fields = new();
            foreach (Field f in this.SourceData.Fields)
            {
                this.Fields.Add(new(f, this));
            }
            this.Fields.CollectionChanged += this.Fields_CollectionChanged;
            this.SetCommands();
            this.AvailableComponents = new([this.Source.Component]);
            this.SelectedComponent = this.Source.Component;
        }
#endif
        #region Commands
        private void SetCommands()
        {
            this.RemoveField = new Command(this.DoRemoveField);
            this.MoveUpField = new Command(this.DoMoveUpField);
            this.MoveDownField = new Command(this.DoMoveDownField);
            this.OpenFieldSettings = new Command(this.DoOpenFieldSettings);
        }

        public ICommand RemoveField { get; private set; }
        public ICommand MoveUpField { get; private set; }
        public ICommand MoveDownField { get; private set; }
        public ICommand OpenFieldSettings { get; private set; }
        private void DoRemoveField(object obj)
        {
            FieldViewModel field = obj as FieldViewModel;
            if (DialogsManager.ShowQuestionDialog($"Вы действительно хотите удалить поле [{field.Name}] из этого класса? После сохранения класса это действие отменить нельзя: это поле будет безвозвратно удалено во всех объектах.", "Удалить поле?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {
                BindingData data = new()
                {
                    BindingClass = this.Source.Id,
                    BindingField = field.Source.Id
                };
                if (data.BindingClass == Guid.Empty || data.BindingField == Guid.Empty)
                {
                    this.Fields.Remove(field);
                }
                using Class cl = new();
                List<string> list = cl.FindBackReferencesNames(data);
                if (list.Count > 0)
                {
                    DialogsManager.ShowExclamationDialog($"Поле невозможно удалить, поскольку на него ссылаются следующие классы:\n{string.Join(",\n", list)}", "Удаление невозможно");
                }
                else
                {
                    this.Fields.Remove(field);
                }
            }           
        }
        private void DoMoveUpField(object obj)
        {
            FieldViewModel field = obj as FieldViewModel;
            int position = this.Fields.IndexOf(field);
            if (position > 0)
            {
                position -= 1;
            }
            this.Fields.Remove(field);
            this.Fields.Insert(position, field);
        }
        private void DoMoveDownField(object obj)
        {
            FieldViewModel field = obj as FieldViewModel;
            int position = this.Fields.IndexOf(field);
            if (position < this.Fields.Count - 1)
            {
                position += 1;
            }
            this.Fields.Remove(field);
            this.Fields.Insert(position, field);
        }
        private void DoOpenFieldSettings(object obj)
        {
            FieldViewModel field = obj as FieldViewModel;
            string name = $"Настройки поля [{field.Name}]";
            switch (field.Type)
            {
                case FieldType.String:
                    TextFieldSettings tf = new(field.Source);
                    tf.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Green);
                    break;
                case FieldType.Text:
                    TextBigFieldSettings tb = new(field.Source);
                    tb.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Green);
                    break;
                case FieldType.Integer:
                    NumberFieldSettings n = new(field.Source);
                    n.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case FieldType.Object:
                    BindingData db = new();
                    DialogBinding dialog = new(this, field.Source);
                    dialog.ShowDialog();
                    if (dialog.Result == true)
                    {
                        db.BindingClass = dialog.SelectedClass;
                        db.BindingField = dialog.SelectedField;
                        db.Compliance = dialog.vm.GetConstraintsForSave();
                        db.OnDeleteCascade = dialog.vm.Cascade;
                        db.OnDeleteRestrict = dialog.vm.Restrict;
                        //field.Source.Value = JsonConvert.SerializeObject(db);
                        field.Source.SetBindingData(db);
                    }
                    break;
                case FieldType.LocalEnumeration:
                    LocalEnumerationFieldSettings le = new(field.Source);
                    le.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case FieldType.GlobalEnumeration:
                    GlobalEnumerationFieldSettings ge = new(field.Source);
                    ge.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case FieldType.LocalConstant:
                    ConstantFieldSettings cf = new(field.Source);
                    cf.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Red);
                    break;
                case FieldType.GlobalConstant:
                    GlobalConstantFieldSettings gc = new(field.Source);
                    gc.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Red);
                    break;
                case FieldType.HiddenField:
                    ConstantFieldSettings hf = new(field.Source);
                    hf.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Red);
                    break;
                case FieldType.Date:
                    DateFieldSettings dt = new(field.Source);
                    dt.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case FieldType.Table:
                    TableSettings ts = new(field.Source.Value, field.Source.Name);
                    if (ts.ShowDialog() == true)
                    {
                        field.Source.Value = JsonConvert.SerializeObject(ts.Data);
                    }
                    break;
            }
        }
        #endregion

        private void TextDocument_TextChanged(object sender, System.EventArgs e)
        {
            //this.SourceData.Script = this.textDocument.Text;
        }

        public ClassType Type
        {
            get => this.Source.Type;
            set
            {
                this.Source.Type = value;
                this.OnPropertyChanged(nameof(this.Type));
            }
        }
        public Visibility InheritanceVisibility
        {
            get
            {
                if (this.Source.Parents is null || this.Source.Parents.Count == 0)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }
        public string Inheritance
        {
            get
            {
                if (this.Source.Parents is null || this.Source.Parents.Count == 0)
                {
                    return "";
                }
                return string.Join(", ", this.Source.GetParentClassesNames());
            }
        }
        public string NameOfClass
        {
            get => this.Source.Name;
            set
            {
                this.Source.Name = value
                    .Replace(":", "")
                    .Replace("#", "")
                    .Replace("$", "")
                    .Replace("\\", "")
                    .Replace("/", "")
                    .Replace("!", "");
                this.OnPropertyChanged(nameof(this.NameOfClass));
            }
        }
        private ObservableCollection<WorkspaceComponent> components;
        public ObservableCollection<WorkspaceComponent> AvailableComponents
        {
            get
            {
                return this.components;
            }
            set
            {
                this.components = value;
                this.OnPropertyChanged(nameof(this.AvailableComponents));
            }
        }
        public WorkspaceComponent SelectedComponent
        {
            get
            {
                foreach (WorkspaceComponent wc in this.AvailableComponents)
                {
                    if (wc == this.Source.Component)
                    {
                        return wc;
                    }
                }
                return null;
            }
            set
            {
                foreach (WorkspaceComponent wc in this.AvailableComponents)
                {
                    if (wc.Id == value.Id)
                    {
                        this.Source.Component = value;
                        break;
                    }
                }
                this.OnPropertyChanged(nameof(this.SelectedComponent));
            }
        }
        
        public string ListName
        {
            get => this.SourceData.ListName;
            set
            {
                this.SourceData.ListName = value;
                this.OnPropertyChanged(nameof(this.ListName));
            }
        }
        public string NameTemplate
        {
            get => this.SourceData.NameTemplate;
            set
            {
                this.SourceData.NameTemplate = value;
                this.OnPropertyChanged(nameof(this.NameTemplate));
            }
        }
        public bool ShowCard
        {
            get => this.SourceData.ShowCard;
            set
            {
                this.SourceData.ShowCard = value;
                this.OnPropertyChanged(nameof(this.ShowCard));
            }
        }
        public bool EditByAuthorOnly
        {
            get => this.SourceData.EditByAuthorOnly;
            set
            {
                this.SourceData.EditByAuthorOnly = value;
                this.OnPropertyChanged(nameof(this.EditByAuthorOnly));
            }
        }
        public bool AuthorOverrideEnabled
        {
            get => this.SourceData.AuthorOverrideEnabled;
            set
            {
                this.SourceData.AuthorOverrideEnabled = value;
                this.OnPropertyChanged(nameof(this.AuthorOverrideEnabled));
            }
        }
        private TextDocument textDocument;
        public TextDocument CodeModule
        {
            get => this.textDocument;
            set
            {
                this.textDocument = value;
                this.OnPropertyChanged(nameof(this.CodeModule));
            }
        }
        public Visibility DocumentClassVisibility
        {
            get
            {
                switch (this.Source.Type)
                {
                    case ClassType.Document:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }
        public Visibility ProcessClassVisibility
        {
            get
            {
                switch (this.Source.Type)
                {
                    case ClassType.Process:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }
        private ObservableCollection<FieldViewModel> fields;
        public ObservableCollection<FieldViewModel> Fields
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
        private ObservableCollection<MethodViewModel> methods;
        public ObservableCollection<MethodViewModel> Methods
        {
            get
            {
                return this.methods;
            }
            set
            {
                this.methods = value;
                this.OnPropertyChanged(nameof(this.Methods));
            }
        }
        private MethodViewModel selectedMethod;
        public MethodViewModel SelectedMethod
        {
            get
            {
                return this.selectedMethod;
            }
            set
            {
                this.selectedMethod = value;
                this.OnPropertyChanged(nameof(this.SelectedMethod));
            }
        }
        private void Fields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.UpdateFieldCollections();
        }
        public void UpdateFieldCollections()
        {
            this.OnPropertyChanged(nameof(this.MapFieldsList));
        }
        public List<ControlType> AvailableControlTypes
        {
            get
            {
                return [
                    ControlType.VerticalStack, 
                    ControlType.HorizontalStack, 
                    ControlType.Tab,
                    ControlType.TabItem,
                    ControlType.Grid, 
                    ControlType.Group
                    ];
            }
        }
        private ObservableCollection<ViewControlViewModel> controls;
        public ObservableCollection<ViewControlViewModel> ViewControls
        {
            get
            {
                return this.controls;
            }
            set
            {
                this.controls = value;
                this.OnPropertyChanged(nameof(this.ViewControls));
            }
        }
        private ViewControlViewModel selectedViewControl;
        public ViewControlViewModel SelectedViewControl
        {
            get
            {
                return this.selectedViewControl;
            }
            set
            {
                this.selectedViewControl = value;
                this.OnPropertyChanged(nameof(this.SelectedViewControl));
            }
        }

        public void AddNewControlToCustomForm(ViewControl vc)
        {
            if (this.ViewControls is null)
            {
                this.ViewControls = new();
            }
            ViewControlViewModel vm = new(vc);
            vm.OnDrawCalling += this.Vm_OnDrawCalling;
            vm.OnRemoveRequested += this.Vm_OnRemoveRequested;
            this.ViewControls.Add(vm);
        }

        private void Vm_OnRemoveRequested(ViewControlViewModel vm)
        {
            this.ViewControls.Remove(vm);
            this.OnDrawCalling?.Invoke();
        }

        private void Vm_OnDrawCalling()
        {
            this.OnDrawCalling?.Invoke();
        }

        public ObservableCollection<FieldViewModel> MapFieldsList
        {
            get
            {
                ObservableCollection<FieldViewModel> result = new();
                foreach (FieldViewModel field in this.Fields)
                {
                    switch (field.Type)
                    {
                        case FieldType.String:
                        case FieldType.Text:
                        case FieldType.Integer:
                        case FieldType.Float:
                        case FieldType.Object:
                        case FieldType.Date:
                        case FieldType.Boolean:
                        case FieldType.GlobalEnumeration:
                        case FieldType.LocalEnumeration:
                            result.Add(field);
                            break;
                        default:
                            field.ListVisibility = false;
                            break;
                    }
                }
                return result;
            }
        }
        public bool InCustomClassEnabledOnly
        {
            get
            {
                switch (this.Mode)
                {
                    case ClassMode.Usual:
                        return true;
                }
                return false;
            }
        }
        private static List<FieldType> fieldTypes = new()
        {
            FieldType.String,
            FieldType.Text,
            FieldType.Boolean,
            FieldType.Integer,
            FieldType.Float,
            FieldType.Date,
            FieldType.LocalEnumeration, 
            FieldType.GlobalEnumeration,
            FieldType.Object,
#if E_BUSINESS
            FieldType.Structure,
#endif
            FieldType.Table,
            FieldType.LocalConstant,
            FieldType.GlobalConstant, 
            FieldType.HiddenField,           
        };
        public List<FieldType> FieldTypes
        {
            get
            {
                return fieldTypes;
            }
        }

        public void SetData()
        {
            this.SourceData.Fields = new();
            foreach (FieldViewModel field in this.Fields)
            {
                if (field.BelongsThisClass)
                {
                    this.SourceData.Fields.Add(field.Source);
                }              
            }
            this.Source.SetClassData(this.SourceData);
        }
        public List<Field> GetFields()
        {
            List<Field> result = new();
            foreach (FieldViewModel field in this.Fields)
            {
                result.Add(field.Source);
            }
            return result;
        }
        private void CheckField(Field f)
        {
            if (string.IsNullOrWhiteSpace(f.Name))
            {
                throw new FieldDataFailed($"Одному из полей не присвоено имя. Настройте поле, а затем попробуйте снова.");
            }
            try
            {
                switch (f.Type)
                {
                    case FieldType.Text:
                    case FieldType.String:
                        break;
                    case FieldType.LocalEnumeration:
                        if (f.GetLocalEnumeration().Count == 0)
                        {
                            throw new FieldDataFailed("");
                        }
                        break;
                    case FieldType.GlobalEnumeration:
                        if (f.GetGlobalEnumeration().TargetId == Guid.Empty)
                        {
                            throw new FieldDataFailed("");
                        }
                        break;
                    case FieldType.Object:
                        BindingData bd = f.GetBindingData();
                        if (bd.BindingClass == Guid.Empty || bd.BindingField == Guid.Empty)
                        {
                            throw new FieldDataFailed($"Не определена привязка у поля [{f.Name}] (\"{f.VisibleName}\"). Настройте поле, а затем попробуйте снова.");
                        }
                        break;
                    case FieldType.GlobalConstant:
                        //Guid.Parse(f.Value);
                        break;
                    case FieldType.HiddenField:
                        break;
                    case FieldType.Integer:
                        if (f.GetNumberFieldData() == null)
                        {
                            throw new FieldDataFailed("");
                        }
                        break;
                    case FieldType.Date:
                        f.GetDateFieldData();
                        break;
                    case FieldType.Table:
                        JsonConvert.DeserializeObject<TableFieldData>(f.Value);
                        break;
                }
            }
            catch
            {
                throw new FieldDataFailed($"Поле [{f.Name}] (\"{f.VisibleName}\") не настроено. Настройте поле, а затем попробуйте снова.");
            }
        }
        public bool Validate()
        {
            try
            {
                this.Source.Component = this.SelectedComponent;
                if (string.IsNullOrWhiteSpace(this.NameOfClass))
                {
                    DialogsManager.ShowExclamationDialog("Классу не присвоено наименование!", "Сохранение прервано");
                    return false;
                }
                if (this.Fields.Count == 0)
                {
                    DialogsManager.ShowExclamationDialog("Класс не может не содержать полей.", "Сохранение прервано");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(this.NameTemplate))
                {
                    FieldNameInsertor fn = new(new(this.Fields));
                    if (fn.ShowDialog("Поле для наименования объектов", Core.Classes.Icon.Subscript))
                    {
                        this.NameTemplate = $"{this.NameOfClass} {fn.GetSelectedField()}";
                    }
                    else
                    {
                        this.NameTemplate = $"{this.NameOfClass} [{this.fields[0].Name}]";
                    }
                }
                foreach (FieldViewModel field in this.Fields)
                {
                    
                    if (field.BelongsThisClass)
                    {
                        this.CheckField(field.Source);
                        field.Source.SetId();
                    }                  
                }
                this.SourceData.EditorView = new();
                foreach (ViewControlViewModel vm in this.ViewControls)
                {
                    vm.Save();
                    this.SourceData.EditorView.Controls.Add(vm.Source);
                }
                //DialogsManager.ShowInfoDialog(this.SourceData.EditorView);
                this.SetData();
                
                return true;
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Сохранение прервано");
                return false;
            }
        }
        public bool Save()
        {
            bool result = this.Validate();
            this.Source.Save();
            return result;
        }
    }
}
