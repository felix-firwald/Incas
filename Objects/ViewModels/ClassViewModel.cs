using ICSharpCode.AvalonEdit.Document;
using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.AutoUI;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Pages;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Models;
using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

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
        public delegate void OpenAdditionalSettings(IClassDetailsSettings settings);
        public event OpenAdditionalSettings OnAdditionalSettingsOpenRequested;
        public ClassViewModel(Class source)
        {
            this.Mode = ClassMode.Usual;
            this.Source = source;
            this.SourceData = this.Source.GetClassData();
            this.Fields = new();
            foreach (Field f in this.SourceData.Fields)
            {
                this.AddField(f);
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
                    this.AddMethod(method);
                }
            }
            this.Tables = new();
            if (this.SourceData.Tables is not null)
            {
                foreach (Table table in this.SourceData.Tables)
                {
                    this.AddTable(table);
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

        public void AddField(Field f)
        {
            FieldViewModel vm = new(f, this, true);
            vm.OnRemoveRequested += this.DoRemoveField;
            vm.OnMoveUpRequested += this.DoMoveUpField;
            vm.OnMoveDownRequested += this.DoMoveDownField;
            this.Fields.Add(vm);
        }
        public void AddTable(Table f)
        {
            TableViewModel vm = new(f);
            vm.OnOpenTableRequested += this.DoOpenDetails;
            vm.OnRemoveRequested += this.DoRemoveTable;
            this.Tables.Add(vm);
        }

        private void DoRemoveTable(TableViewModel table)
        {
            this.Tables.Remove(table);
        }

        public void AddMethod(Method m)
        {
            MethodViewModel vm = new(m, this);
            vm.OnOpenMethodRequested += this.DoOpenDetails;
            vm.OnRemoveRequested += this.DoRemoveMethod;
            this.Methods.Add(vm);
        }

        private void DoRemoveMethod(MethodViewModel field)
        {
            this.Methods.Remove(field);
        }

        private void DoOpenDetails(IClassDetailsSettings settings)
        {
            settings.SetUpContext(this);
            this.OnAdditionalSettingsOpenRequested?.Invoke(settings);
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
            this.ViewControls = new();
            this.Fields.CollectionChanged += this.Fields_CollectionChanged;
            this.SetCommands();
            this.AvailableComponents = new([this.Source.Component]);
            this.SelectedComponent = this.Source.Component;
        }
#endif
        #region Commands
        private void SetCommands()
        {
            
        }

        private void DoRemoveField(FieldViewModel field)
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
                this.RemoveFieldControl(field.Source.Id);
            }          
        }
        private void DoMoveUpField(FieldViewModel field)
        {
            this.Fields.MoveUp(field);
        }
        private void DoMoveDownField(FieldViewModel field)
        {
            this.Fields.MoveDown(field);
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
        public string InternalName
        {
            get
            {
                return this.Source.InternalName;
            }
            set
            {
                this.Source.InternalName = value.Replace(' ', '_');
                this.OnPropertyChanged(nameof(this.InternalName));
            }
        }
        public string Description
        {
            get
            {
                return this.Source.Description;
            }
            set
            {
                this.Source.Description = value;
                this.OnPropertyChanged(nameof(this.Description));
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
        private ObservableCollection<TableViewModel> tables;
        public ObservableCollection<TableViewModel> Tables
        {
            get
            {
                return this.tables;
            }
            set
            {
                this.tables = value;
                this.OnPropertyChanged(nameof(this.Tables));
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
            vm.OnMoveUpRequested += this.Vm_OnMoveUpRequested;
            vm.OnMoveDownRequested += this.Vm_OnMoveDownRequested;
            this.ViewControls.Add(vm);
        }
        public void RemoveFieldControl(Guid field)
        {
            foreach (ViewControlViewModel control in this.ViewControls)
            {
                if (control.Source.Field == field)
                {
                    this.ViewControls.Remove(control);
                    return;
                }
                control.RemoveField(field);
            }
        }
        private void Vm_OnMoveDownRequested(ViewControlViewModel vm)
        {
            this.ViewControls.MoveDown(vm);
        }

        private void Vm_OnMoveUpRequested(ViewControlViewModel vm)
        {
            this.ViewControls.MoveUp(vm);
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
        private double previewZoom = 100;
        public double PreviewFormZoom
        {
            get
            {
                return this.previewZoom;
            }
            set
            {
                this.previewZoom = value;
                this.OnPropertyChanged(nameof(this.PreviewFormZoom));
            }
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
            this.SourceData.Methods = new();
            foreach (MethodViewModel method in this.Methods)
            {
                if (method.BelongsThisClass)
                {
                    this.SourceData.Methods.Add(method.Source);
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

        public bool Validate()
        {
            try
            {
                this.Source.Component = this.SelectedComponent;
                if (string.IsNullOrEmpty(this.InternalName))
                {
                    DialogsManager.ShowExclamationDialog("Классу не присвоено уникальное имя!", "Сохранение прервано");
                    return false;
                }
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
                        field.Source.Check();
                        field.Source.SetId();
                    }                  
                }
                foreach (MethodViewModel method in this.Methods)
                {
                    if (method.BelongsThisClass)
                    {
                        method.Source.SetId();
                    }
                }
                this.SourceData.EditorView = new();
                foreach (ViewControlViewModel vm in this.ViewControls)
                {
                    vm.Save();
                    this.SourceData.EditorView.Controls.Add(vm.Source);
                }
                this.SetData();
                
                return true;
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Сохранение прервано");
                return false;
            }
        }
        /// <summary>
        /// походу SetData выполняется ДВАЖДЫ!
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            bool result = this.Validate();
            this.SetData();
            this.Source.Save();
            return result;
        }
    }
}
