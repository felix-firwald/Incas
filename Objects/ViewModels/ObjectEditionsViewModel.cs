using Incas.Core.Classes;
using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.Editions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;

namespace Incas.Objects.ViewModels
{
    public class ObjectEditionsViewModel : BaseViewModel
    {
        public IClass TargetClass { get; set; }
        public IObject TargetObject { get; set; }
        public delegate void SelectionChanged(Guid version);
        public event SelectionChanged OnSelectedVersionChanged;
        public ObjectEditionsViewModel(IClass @class, IObject @object)
        {
            this.TargetClass = @class;
            this.TargetObject = @object;
        }
        public List<Field> Fields
        {
            get
            {
                return this.TargetClass.GetClassData().GetSavebleFields();
            }
        }
        private Field selectedField { get; set; }
        public Field SelectedField
        {
            get
            {
                return this.selectedField;
            }
            set
            {
                this.selectedField = value;
                this.OnPropertyChanged(nameof(this.SelectedField));               
                this.OnPropertyChanged(nameof(this.Editions));
                this.SelectedEditionField = new();
            }
        }
        public List<FieldEdition> Editions
        {
            get
            {
                if (this.selectedField is null)
                {
                    return new();
                }
                return Processor.GetEditionsOfField(this.TargetClass, this.TargetObject, this.SelectedField);
            }
        }
        private FieldEdition selectedEditionField { get; set; }
        public FieldEdition SelectedEditionField
        {
            get
            {
                return this.selectedEditionField;
            }
            set
            {
                this.selectedEditionField = value;
                
                this.OnPropertyChanged(nameof(this.selectedEditionField));
                if (this.selectedEditionField.EditionId != Guid.Empty)
                {
                    this.OnSelectedVersionChanged?.Invoke(this.selectedEditionField.EditionId);
                }               
            }
        }
    }
}
