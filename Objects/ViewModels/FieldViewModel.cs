using DocumentFormat.OpenXml.Bibliography;
using Incas.Core.Classes;
using Incas.Core.ViewModels;
using Incas.Objects.AutoUI;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Windows;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Incas.Objects.ViewModels
{
    public class FieldViewModel : BaseViewModel, IClassMemberViewModel
    {
        public Field Source;
        public ClassViewModel Owner;
        public delegate void Action(FieldViewModel field);
        public event Action OnRemoveRequested;
        public event Action OnMoveUpRequested;
        public event Action OnMoveDownRequested;

        public FieldViewModel(Field field, ClassViewModel owner, bool isExists = false) // new
        {
            this.Source = field;
            this.SetCommands();
            this.Owner = owner;
            if (isExists)
            {
                this.IsExists = true;
                this.ExistedType = this.Source.Type;
            }
        }
        public FieldViewModel(Field field, bool isExists = false) // new
        {
            this.Source = field;
            this.SetCommands();
            if (isExists)
            {
                this.IsExists = true;
                this.ExistedType = this.Source.Type;
            }
        }
        public FieldViewModel(ClassViewModel owner)
        {
            this.Source = new();
            if (this.Source.Id == Guid.Empty)
            {
                this.Source.Id = Guid.NewGuid();
            }
            this.SetCommands();
            this.Owner = owner;
        }
        #region Commands
        
        private void SetCommands()
        {
            this.AssignToContainer = new Command(this.DoAssignToContainer);
            this.RemoveField = new Command(this.DoRemoveField);
            this.MoveUpField = new Command(this.DoMoveUpField);
            this.MoveDownField = new Command(this.DoMoveDownField);
            this.OpenFieldSettings = new Command(this.DoOpenFieldSettings);
        }
        public ICommand AssignToContainer { get; private set; }
        public ICommand RemoveField { get; private set; }
        public ICommand MoveUpField { get; private set; }
        public ICommand MoveDownField { get; private set; }
        public ICommand OpenFieldSettings { get; private set; }
        public void DoAssignToContainer(object param)
        {
            if (this.Owner.SelectedViewControl is not null)
            {
                ViewControlViewModel vm = new(
                    new() { 
                        Name = this.Name, 
                        Type = IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms.ControlType.FieldFiller, 
                        Field = this.Source.Id 
                    }
                );
                this.Owner.RemoveFieldControl(this.Source.Id);
                this.Owner.SelectedViewControl.AddChild(vm);
            }
        }
        public void DoRemoveField(object param)
        {
            if (DialogsManager.ShowQuestionDialog($"Вы действительно хотите удалить поле [{this.Name}]? После сохранения это действие отменить нельзя: это поле будет безвозвратно удалено.", "Удалить поле?", "Удалить", "Не удалять") == Core.Views.Windows.DialogStatus.Yes)
            {
                this.OnRemoveRequested?.Invoke(this);
            }
        }
        public void DoMoveUpField(object param)
        {
            this.OnMoveUpRequested?.Invoke(this);
        }
        public void DoMoveDownField(object param)
        {
            this.OnMoveDownRequested?.Invoke(this);
        }
        private void DoOpenFieldSettings(object obj)
        {
            string name = $"Настройки поля [{this.Name}]";
            switch (this.Type)
            {
                case FieldType.String:
                    TextFieldSettings tf = new(this.Source);
                    tf.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Green);
                    break;
                case FieldType.Text:
                    TextBigFieldSettings tb = new(this.Source);
                    tb.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Green);
                    break;
                case FieldType.Integer:
                    NumberFieldSettings n = new(this.Source);
                    n.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case FieldType.Object:
                    BindingData db = new();
                    DialogBinding dialog = new(this.Owner, this.Source);
                    dialog.ShowDialog();
                    if (dialog.Result == true)
                    {
                        db.BindingClass = dialog.SelectedClass;
                        db.BindingField = dialog.SelectedField;
                        db.Compliance = dialog.vm.GetConstraintsForSave();
                        db.OnDeleteCascade = dialog.vm.Cascade;
                        db.OnDeleteRestrict = dialog.vm.Restrict;
                        this.Source.SetBindingData(db);
                    }
                    break;
                case FieldType.LocalEnumeration:
                    LocalEnumerationFieldSettings le = new(this.Source);
                    le.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case FieldType.GlobalEnumeration:
                    GlobalEnumerationFieldSettings ge = new(this.Source);
                    ge.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
                case FieldType.Date:
                    DateFieldSettings dt = new(this.Source);
                    dt.ShowDialog(name, Icon.Sliders, DialogSimpleForm.Components.IconColor.Yellow);
                    break;
            }
        }

        #endregion
        public string VisibleName
        {
            get => this.Source.VisibleName;
            set
            {
                this.Source.VisibleName = value;
                this.OnPropertyChanged(nameof(this.VisibleName));
            }
        }
        public bool BelongsThisClass
        {
            get
            {
                return this.Source.Owner is null;
            }
        }
        public Visibility BelongsVisibility
        {
            get
            {
                return this.FromBool(this.BelongsThisClass);
            }
        }

        public string SelectedValue
        {
            get => this.Source.Value;
            set
            {
                this.Source.Value = value;
                this.OnPropertyChanged(nameof(this.SelectedValue));
            }
        }

        public string Name
        {
            get => this.Source.Name;
            set
            {
                if (value != this.Source.Name)
                {
                    this.Source.Name = value
                        .Replace(" ", "_")
                        .Replace(".", "_")
                        .Replace(":", "_")
                        .Replace("$", "_");
                    this.OnPropertyChanged(nameof(this.Name));
                }
            }
        }
        private bool expanded = false;
        public bool IsExpanded
        {
            get
            {
                return this.expanded;
            }
            set
            {
                this.expanded = value;
                this.OnPropertyChanged(nameof(this.IsExpanded));
            }
        }
        private bool exists = false;
        public bool IsExists
        {
            get
            {
                return this.exists;
            }
            set
            {
                this.exists = value;
                this.OnPropertyChanged(nameof(this.IsExists));
            }
        }
        private FieldType existedType;
        public FieldType ExistedType
        {
            get
            {
                return this.existedType;
            }
            set
            {
                this.existedType = value;
                this.OnPropertyChanged(nameof(this.ExistedType));
            }
        }
        public FieldType Type
        {
            get
            {
                return this.Source.Type;
            }
            set
            {
                if (this.IsExists)
                {
                    if (!Field.CanChangeTypeTo(this.ExistedType, value))
                    {
                        //if (DialogsManager.ShowQuestionDialog(
                        //    "Изменение типа данных этого поля на выбранный тип не поддерживается INCAS для автоматического преобразования в существующих объектах. Вы уверены, что хотите продолжить?",
                        //    "Поменять тип?",
                        //    "Да, поменять",
                        //    "Оставить текущий"
                        //    ) == Core.Views.Windows.DialogStatus.Yes)
                        //{
                            this.Source.Type = value;
                            this.OnPropertyChanged(nameof(this.Type));
                        //}
                        //else
                        //{
                        //    this.Source.Type = this.ExistedType;
                        //    this.OnPropertyChanged(nameof(this.Type));
                        //    return;
                        //}                      
                    }
                    else
                    {
                        this.Source.Type = value;
                        this.OnPropertyChanged(nameof(this.Type));
                    }
                }
                else
                {
                    this.Source.Type = value;
                    this.OnPropertyChanged(nameof(this.Type));
                }                
            }
        }
        public bool ListVisibility
        {
            get
            {
                return this.Source.ListVisibility;
            }
            set
            {
                this.Source.ListVisibility = value;
                this.OnPropertyChanged(nameof(this.ListVisibility));
            }
        }
        public bool IsUnique
        {
            get
            {
                return this.Source.IsUnique;
            }
            set
            {
                this.Source.IsUnique = value;
                this.OnPropertyChanged(nameof(this.IsUnique));
            }
        }
        public bool ActionBindingEnabled
        {
            get
            {
                return true;
            }
        }
        public bool EventBindingEnabled
        {
            get
            {
                return true;
            }
        }
        public Guid Id
        {
            get
            {
                return this.Source.Id;
            }
        }

        public IClassMemberViewModel.MemberType ClassMemberType => IClassMemberViewModel.MemberType.Field;
    }
}