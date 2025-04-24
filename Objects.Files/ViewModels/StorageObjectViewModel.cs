using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Files.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class StorageObjectViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public StorageObject Source { get; set; }

        public StorageObjectViewModel(StorageObject source)
        {
            this.Source = source;
        }
    }
}
