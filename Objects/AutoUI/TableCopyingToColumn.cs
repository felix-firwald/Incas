﻿using Incas.DialogSimpleForm.Components;
using IncasEngine.ObjectiveEngine.Models;
using System.Collections.Generic;
using System.ComponentModel;
using static IncasEngine.ObjectiveEngine.FieldComponents.TableFieldData;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для TableCopyingToColumn.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class TableCopyingToColumn : StaticAutoUIBase
    {
        protected override string FinishButtonText { get => "Скопировать"; }
        #region Data
        [Description("Источник копирования")]
        public Selector Source { get; set; }
        [Description("Целевая колонка")]
        public Selector Target { get; set; }
        #endregion

        public TableCopyingToColumn(List<Field> columns)
        {
            this.Source = new(new());
            this.Target = new(new());
            foreach (Field dc in columns)
            {
                this.Source.Pairs.Add(dc, dc.VisibleName);
                this.Target.Pairs.Add(dc, dc.VisibleName);
            }
        }

        #region Functionality
        public string GetSourceColumnName()
        {
            return ((Field)this.Source.SelectedObject).Name;
        }
        public string GetTargetColumnName()
        {
            return ((Field)this.Target.SelectedObject).Name;
        }
        public override void Load()
        {
            
        }

        public override void Validate()
        {

        }

        public override void Save()
        {
            
        }
        #endregion
    }
}
