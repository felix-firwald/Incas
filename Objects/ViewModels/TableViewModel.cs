using Incas.Core.ViewModels;
using Incas.Objects.Interfaces;
using Incas.Objects.Views.Pages;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Incas.Objects.ViewModels.MethodViewModel;

namespace Incas.Objects.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class TableViewModel : BaseViewModel
    {
        public delegate void OpenMethod(IClassDetailsSettings settings);
        public event OpenMethod OnOpenTableRequested;
        public delegate void Action(TableViewModel table);
        public event Action OnRemoveRequested;

        /// <summary>
        /// Model
        /// </summary>
        public Table Source { get; set; }
        
        public TableViewModel(Table source)
        {
            this.Source = source;
            this.OpenTableSettings = new Command(this.DoOpenTableSettings);
            this.RemoveTable = new Command(this.DoRemoveTable);
        }

        private void DoRemoveTable(object obj)
        {
            this.OnRemoveRequested?.Invoke(this);
        }

        private void DoOpenTableSettings(object obj)
        {
            TableEditor editor = new(this);
            this.OnOpenTableRequested?.Invoke(editor);
        }

        public ICommand OpenTableSettings { get; set; }
        public ICommand RemoveTable { get; set; }

        public string Name
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value.Replace(' ', '_');
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        /// <summary>
        /// Сохраняет изменения в Model
        /// </summary>
        //public bool Save()
        //{

        //}
    }
}
