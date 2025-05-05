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
    public class GeneralizatorViewModel : BaseViewModel
    {
        public delegate void OpenAdditionalSettings(IClassDetailsSettings settings);
        public event OpenAdditionalSettings OnAdditionalSettingsOpenRequested;
        public Generalizator Source { get; set; }
        public GeneralizatorViewModel()
        {
            this.Source = new();
            this.Fields = new();
        }
        public GeneralizatorViewModel(Generalizator gen)
        {
            this.Source = gen;
            this.Fields = new();
            foreach (Field f in this.Source.Data.Fields)
            {
                this.AddField(f, true);
            }
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
        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
            {
                DialogsManager.ShowExclamationDialog("Обобщению не присвоено имя!", "Сохранение прервано");
                return false;
            }
            try
            {
                this.Source.Data = new();
                this.Source.Data.Fields = new();
                foreach (FieldViewModel field in this.Fields)
                {
                    field.Source.Check();
                    field.Source.SetId();
                    this.Source.Data.Fields.Add(field.Source);
                }
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Сохранение прервано");
                return false;
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
                return false;
            }
            this.Source.WriteGeneralizator();
            return true;
        }
    }
}
