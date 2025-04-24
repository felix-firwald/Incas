using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Processes.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class ProcessViewerViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public Process Source { get; set; }
        public Class Class { get; set; }

        public ProcessViewerViewModel(Class cl, Process source)
        {
            this.Source = source;
            this.Class = cl;
        }
        public string Description
        {
            get
            {
                return this.Source.Data.Description;
            }
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
