using Incas.Core.AutoUI;
using Incas.Objects.Components;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;

namespace Incas.Objects.AutoUI
{
    /// <summary>
    /// Класс автоматической генерации формы и сбора данных для StatusSettings.
    /// Метод Load вызывается перед генерацией формы.
    /// Метод Save вызывается после применения изменений на форме.
    /// </summary>
    public class StatusSettings : AutoUIBase
    {
        #region Data
        [StringLength(25)]
        [Description("Наименование статуса")]
        public string Name { get; set; }

        [StringLength(300)]
        [Description("Описание статуса")]
        public string Description { get; set; }

        [Description("Цвет статуса")]
        public Color Color { get; set; }
        #endregion

        public StatusSettings(StatusData data)
        {
            this.Name = data.Name;
            this.Description = data.Description;
            this.Color = data.Color;
        }
        public StatusSettings()
        {

        }

        #region Functionality
        public StatusData GetData()
        {
            StatusData data = new()
            {
                Name = this.Name,
                Description = this.Description,
                Color = this.Color
            };
            return data;
        }
        #endregion
    }
}
