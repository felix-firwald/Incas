using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.Objects.Components;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static IncasEngine.ObjectiveEngine.FieldComponents.TableFieldData;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ColumnSelector.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ColumnSelector : StaticAutoUIBase
    {
        protected override string FinishButtonText { get => "Скопировать"; }
        #region Data
        [Description("Целевая колонка")]
        public Selector Target { get; set; }

        [Description("Только для пустых ячеек")]
        public bool OnlyEmptyOnes { get; set; }
        #endregion

        public ColumnSelector(List<Field> columns, Dictionary<Guid, ColumnConfiguration> conf)
        {
            this.OnlyEmptyOnes = true;
            this.Target = new(new());
            foreach (Field dc in columns)
            {
                ColumnConfiguration cc = conf[dc.Id];
                if (!cc.IsReadOnly || cc.Visibility == System.Windows.Visibility.Visible)
                {
                    this.Target.Pairs.Add(dc, dc.VisibleName);
                }               
            }
        }

        #region Functionality
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
