using Incas.Core.ViewModels;
using IncasEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Admin.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class GeneralizatorIncludingViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public GeneralizatorItem Source { get; set; }
        public delegate void GeneralizatorIncludingChanged(Guid item);
        public event GeneralizatorIncludingChanged OnIncluded;
        public event GeneralizatorIncludingChanged OnRemoved;

        public GeneralizatorIncludingViewModel(GeneralizatorItem source)
        {
            this.Source = source;
        }
        private bool isIncluding = false;
        public bool IsIncluding
        {
            get
            {
                return this.isIncluding;
            }
            set
            {
                this.isIncluding = value;
                if (value is true)
                {
                    this.OnIncluded?.Invoke(this.Source.Id);
                }
                else
                {
                    this.OnRemoved?.Invoke(this.Source.Id);
                }
                this.OnPropertyChanged(nameof(this.IsIncluding));
            }
        }
        public string Name
        {
            get
            {
                return this.Source.Name;
            }
        }
        public string Description
        {
            get
            {
                return this.Source.Description;
            }
        }
    }
}
