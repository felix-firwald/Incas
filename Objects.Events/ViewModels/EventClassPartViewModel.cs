using Incas.Core.ViewModels;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Events.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class EventClassPartViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public IMembersContainerViewModel Source { get; set; }

        public EventClassPartViewModel(IMembersContainerViewModel source)
        {
            this.Source = source;
        }

        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>
        public bool Save()
        {
            return true;
        }
    }
}
