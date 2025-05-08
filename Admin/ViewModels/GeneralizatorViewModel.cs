using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Incas.Admin.ViewModels
{
    public class GeneralizatorViewModel : BaseViewModel, IMembersContainerViewModel
    {
        public delegate void OpenAdditionalSettings(IClassDetailsSettings settings);
        public event OpenAdditionalSettings OnAdditionalSettingsOpenRequested;
        public Generalizator Source { get; set; }
        public GeneralizatorViewModel()
        {
            this.Source = new();
            this.Init();
        }
        public GeneralizatorViewModel(Generalizator gen)
        {
            this.Source = gen;
            this.Init();
            if (this.Source.Data.Fields is not null)
            {
                foreach (Field f in this.Source.Data.Fields)
                {
                    this.AddField(f, true);
                }
            }
            if (this.Source.Data.Methods is not null)
            {
                foreach (Method f in this.Source.Data.Methods)
                {
                    this.AddMethod(f);
                }
            }
            if (this.Source.Data.StaticMethods is not null)
            {
                foreach (Method f in this.Source.Data.StaticMethods)
                {
                    this.AddStaticMethod(f);
                }
            }
            if (this.Source.Data.Tables is not null)
            {
                foreach (Table f in this.Source.Data.Tables)
                {
                    this.AddTable(f);
                }
            }          
        }
        private void Init()
        {
            this.Fields = new();
            this.Fields.CollectionChanged += this.Members_CollectionChanged;
            this.Methods = new();
            this.Methods.CollectionChanged += this.Members_CollectionChanged;
            this.StaticMethods = new();
            this.Tables = new();
            this.Tables.CollectionChanged += this.Members_CollectionChanged;
        }
        public string Name
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        private void Members_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged(nameof(this.Members));
        }
        public void AddField(Field f, bool isExists = false)
        {
            FieldViewModel field = new(f, isExists);
            field.OnMoveDownRequested += this.Field_OnMoveDownRequested;
            field.OnMoveUpRequested += this.Field_OnMoveUpRequested;
            field.OnRemoveRequested += this.Field_OnRemoveRequested;
            this.Fields.Add(field);
        }
        public void AddMethod(Method m)
        {
            MethodViewModel vm = new(m);
            vm.OnOpenMethodRequested += this.DoOpenDetails;
            vm.OnRemoveRequested += this.DoRemoveMethod;
            this.Methods.Add(vm);
        }
        public void AddStaticMethod(Method m)
        {
            MethodViewModel vm = new(m);
            vm.OnOpenMethodRequested += this.DoOpenDetails;
            vm.OnRemoveRequested += this.DoRemoveMethod;
            this.StaticMethods.Add(vm);
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

        private void DoRemoveMethod(MethodViewModel field)
        {
            this.Methods.Remove(field);
        }

        private void DoOpenDetails(IClassDetailsSettings settings)
        {
            settings.SetUpContext(this);
            this.OnAdditionalSettingsOpenRequested?.Invoke(settings);
        }

        private void Field_OnRemoveRequested(FieldViewModel field)
        {
            this.Fields.Remove(field);
        }

        private void Field_OnMoveUpRequested(FieldViewModel field)
        {
            this.Fields.MoveUp(field);
        }

        private void Field_OnMoveDownRequested(FieldViewModel field)
        {
            this.Fields.MoveDown(field);
        }
        public ObservableCollection<IClassMemberViewModel> Members
        {
            get
            {
                ObservableCollection<IClassMemberViewModel> result = new();
                foreach (IClassMemberViewModel member in this.Fields)
                {
                    result.Add(member);
                }
                foreach (IClassMemberViewModel member in this.Tables)
                {
                    result.Add(member);
                }
                foreach (IClassMemberViewModel member in this.Methods)
                {
                    result.Add(member);
                }
                return result;
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
        private ObservableCollection<MethodViewModel> staticMethods;
        public ObservableCollection<MethodViewModel> StaticMethods
        {
            get
            {
                return this.staticMethods;
            }
            set
            {
                this.staticMethods = value;
                this.OnPropertyChanged(nameof(this.StaticMethods));
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
#endif
        };
        public List<FieldType> FieldTypes
        {
            get
            {
                return fieldTypes;
            }
        }
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                throw new FieldDataFailed("Обобщению не присвоено имя!");
            }
            int memberCount = this.fields.Count + this.methods.Count;
            if (memberCount == 0)
            {
                throw new FieldDataFailed("Обобщение не содержит ни одного элемента!");
            }
            HashSet<string> names = new();
            foreach (FieldViewModel field in this.Fields)
            {
                if (names.Contains(field.Name))
                {
                    throw new FieldDataFailed($"Внутреннее имя [{field.Name}] уже существует в обобщении. Пожалуйста, проверьте уникальность всех его элементов.");
                }
                else
                {
                    names.Add(field.Name);
                }
                field.Source.Check();
                field.Source.SetId();
            }
            foreach (TableViewModel table in this.Tables)
            {
                if (names.Contains(table.Name))
                {
                    throw new FieldDataFailed($"Внутреннее имя [{table.Name}] уже существует в обобщении. Пожалуйста, проверьте уникальность всех его элементов.");
                }
                else
                {
                    names.Add(table.Name);
                }
                table.Validate();
            }
            foreach (MethodViewModel method in this.Methods)
            {
                if (names.Contains(method.Name))
                {
                    throw new FieldDataFailed($"Внутреннее имя [{method.Name}] уже существует в обобщении. Пожалуйста, проверьте уникальность всех его элементов.");
                }
                else
                {
                    names.Add(method.Name);
                }
                method.Source.SetId();
            }
        }
        public void Save()
        {
            this.Source.Data = new();
            this.Source.Data.Fields = new();
            foreach (FieldViewModel field in this.Fields)
            {
                field.Source.SetId();
                this.Source.Data.Fields.Add(field.Source);
            }
            this.Source.Data.Methods = new();
            foreach (MethodViewModel method in this.Methods)
            {
                if (method.BelongsThisClass)
                {
                    this.Source.Data.Methods.Add(method.Source);
                }
            }
            this.Source.Data.StaticMethods = new();
            foreach (MethodViewModel method in this.StaticMethods)
            {
                this.Source.Data.StaticMethods.Add(method.Source);
            }
            this.Source.Data.Tables = new();
            foreach (TableViewModel table in this.Tables)
            {
                if (table.BelongsThisClass)
                {
                    table.Save();
                    this.Source.Data.Tables.Add(table.Source);
                }
            }
            this.Source.WriteGeneralizator();
            Class cl = new();
            List<Class> classes = cl.GetClassesByGeneralizator(this.Source);
            foreach (Class c in classes)
            {
                c.ApplyGeneralizator(this.Source);
            }
            cl.Update(classes);
        }
    }
}
