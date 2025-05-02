using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Pages;
using IncasEngine.Core.ExtensionMethods;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Incas.Objects.ViewModels.MethodViewModel;

namespace Incas.Objects.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class TableViewModel : BaseViewModel, IClassMemberViewModel
    {
        public delegate void OpenMethod(IClassDetailsSettings settings);
        public event OpenMethod OnOpenTableRequested;
        public delegate void Action(TableViewModel table);
        public event Action OnRemoveRequested;

        /// <summary>
        /// Model
        /// </summary>
        public Table Source { get; set; }
        public ClassViewModel Owner;

        public TableViewModel(ClassViewModel owner, Table source)
        {
            this.Owner = owner;
            this.Source = source;
            this.OpenTableSettings = new Command(this.DoOpenTableSettings);
            this.RemoveTable = new Command(this.DoRemoveTable);
            this.AssignToContainer = new Command(this.DoAssignToContainer);
            this.Fields = new();
            if (this.Source.Fields is not null)
            {
                foreach (Field f in source.Fields)
                {
                    this.AddField(f);
                }
            }          
        }

        private void DoAssignToContainer(object obj)
        {
            if (this.Owner.SelectedViewControl is not null)
            {
                ViewControlViewModel vm = new(
                    new()
                    {
                        Name = this.Name,
                        Type = IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms.ControlType.Table,
                        Table = this.Source.Id
                    }
                );
                this.Owner.RemoveTableControl(this.Source.Id);
                this.Owner.SelectedViewControl.AddChild(vm);
            }
        }

        public void AddField(Field f)
        {
            FieldViewModel vm = new(f, this.Owner, true);
            vm.OnRemoveRequested += this.DoRemoveField;
            vm.OnMoveUpRequested += this.DoMoveUpField;
            vm.OnMoveDownRequested += this.DoMoveDownField;
            this.Fields.Add(vm);
        }

        private void DoMoveDownField(FieldViewModel field)
        {
            this.Fields.MoveDown(field);
        }

        private void DoMoveUpField(FieldViewModel field)
        {
            this.Fields.MoveUp(field);
        }

        private void DoRemoveField(FieldViewModel field)
        {
            this.Fields.Remove(field);
        }

        private void DoRemoveTable(object obj)
        {
            if (DialogsManager.ShowQuestionDialog($"Вы действительно хотите удалить таблицу [{this.Name}]? После сохранения это действие отменить нельзя: эта таблица будет безвозвратно удалена.", "Удалить таблицу?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {
                this.OnRemoveRequested?.Invoke(this);
            }
        }

        private void DoOpenTableSettings(object obj)
        {
            TableEditor editor = new(this);
            this.OnOpenTableRequested?.Invoke(editor);
        }

        public ICommand OpenTableSettings { get; set; }
        public ICommand RemoveTable { get; set; }
        public ICommand AssignToContainer { get; private set; }

        public string Name
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value.Replace(' ', '_');
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public string VisibleName
        {
            get
            {
                return this.Source.VisibleName;
            }
            set
            {
                this.Source.VisibleName = value.Replace('_', ' ');
                this.OnPropertyChanged(nameof(this.VisibleName));
            }
        }
        public string ConsolidatedName
        {
            get
            {
                return this.Source.ConsolidatedName;
            }
            set
            {
                this.Source.ConsolidatedName = value;
                this.OnPropertyChanged(nameof(this.ConsolidatedName));
            }
        }
        public bool BelongsThisClass
        {
            get
            {
                return this.Source.Owner is null;
            }
        }

        private static List<FieldType> fieldTypes = new()
        {
            FieldType.String,
            FieldType.Boolean,
            FieldType.Integer,
            FieldType.Float,
            FieldType.Date,
            FieldType.Time,
            FieldType.LocalEnumeration,
            FieldType.GlobalEnumeration,
            //FieldType.Object,
        };
        public List<FieldType> FieldTypes
        {
            get
            {
                return fieldTypes;
            }
        }

        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>
        public bool Save()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                throw new FieldDataFailed("Одной из таблиц не присвоено имя!");
            }
            if (this.Fields.Count == 0)
            {
                throw new FieldDataFailed($"Таблица [{this.Name}] не содержит ни одного поля.");
            }
            this.Source.Fields = new();
            foreach (FieldViewModel field in this.Fields)
            {
                field.Source.Check();
                if (string.IsNullOrEmpty(field.VisibleName))
                {
                    field.VisibleName = field.Name;
                }
                this.Source.Fields.Add(field.Source);
            }
            return true;
        }
        public Guid Id
        {
            get
            {
                return this.Source.Id;
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
        public IClassMemberViewModel.MemberType ClassMemberType => IClassMemberViewModel.MemberType.Table;
    }
}
