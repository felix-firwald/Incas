using Incas.Core.Attributes;
using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using Incas.DialogSimpleForm.Exceptions;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using System.ComponentModel;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для ViewControlGridSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Validate вызывается перед сохранением формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class ViewControlGridSettings : StaticAutoUIBase
    {
        #region Data
        private ViewControl.ViewControlGridSettings source { get; set; }
        [Description("Количество колонок")]
        public int ColumnsCount { get; set; }

        [Description("Количество рядов")]
        public int RowsCount { get; set; }
        #endregion

        public ViewControlGridSettings(ViewControl.ViewControlGridSettings settings)
        {
            this.source = settings;
            if (this.source is null)
            {
                this.source = new();
            }
            else
            {
                this.ColumnsCount = this.source.ColumnsCount;
                this.RowsCount = this.source.RowsCount;
            }
        }

        #region Functionality
        public override void Load()
        {
            
        }

        public override void Validate()
        {
            if (this.ColumnsCount == 0)
            {
                throw new SimpleFormFailed("Количество колонок не может быть 0!");
            }
            if (this.RowsCount == 0)
            {
                throw new SimpleFormFailed("Количество рядов не может быть 0!");
            }
        }

        public override void Save()
        {
            this.source.RowsCount = this.RowsCount;
            this.source.ColumnsCount = this.ColumnsCount;
        }
        public ViewControl.ViewControlGridSettings GetResult()
        {
            return this.source;
        }
        #endregion
    }
}
