using Incas.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Core.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class StartPageViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public object Source { get; set; }

        public StartPageViewModel()
        {
            
        }
         
    }
}
