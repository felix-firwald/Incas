using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Interfaces;
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
    public class ProcessViewModel : BaseViewModel, IViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public Process Source { get; set; }

        public ProcessViewModel(Process source)
        {
            this.Source = source;
            if (this.Source.Data is null)
            {
                this.Source.Data = new();
            }
        }

        public DateTime OpenDate
        {
            get
            {
                return this.Source.OpenDate;
            }
            set
            {
                this.Source.OpenDate = value;
                this.OnPropertyChanged(nameof(this.OpenDate));
            }
        }
        public DateTime CloseDate
        {
            get
            {
                return this.Source.CloseDate;
            }
            set
            {
                this.Source.CloseDate = value;
                this.OnPropertyChanged(nameof(this.CloseDate));
            }
        }
        public string Description
        {
            get
            {
                return this.Source.Data.Description;
            }
            set
            {
                this.Source.Data.Description = value;
                this.OnPropertyChanged(nameof(this.Description));
            }
        }

        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>
        public void Save()
        {

        }
    }
}
