using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Incas.Objects.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class TableUserItemViewModel : BaseViewModel
    {
        public delegate void ActionRequest(Table tab);
        public event ActionRequest OnOpenTableRequested;
        /// <summary>
        /// Model
        /// </summary>
        public Table Source { get; set; }

        public TableUserItemViewModel(Table source)
        {
            this.Source = source;
            this.OpenTableView = new Command(this.DoOpenTableView);
        }

        private void DoOpenTableView(object obj)
        {
            this.OnOpenTableRequested?.Invoke(this.Source);
        }

        public string ConsolidatedName
        {
            get
            {
                return this.Source.ConsolidatedName;
            }
            set
            {
                this.Source.ConsolidatedName = value;
                this.OnPropertyChanged(nameof(this.ConsolidatedName));
            }
        }

        public ICommand OpenTableView { get; set; }
    }
}
