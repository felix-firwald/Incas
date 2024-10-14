using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Incas.Core.ViewModels
{
    class LoadingBoxViewModel : BaseViewModel
    {
        private string text = "Выполняется процесс...";
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                {
                    this.OnPropertyChanged(nameof(this.Text));
                }));                
            }
        }
        private bool isIndeterminate = true;
        public bool IsIndeterminate
        {
            get
            {
                return this.isIndeterminate;
            }
            set
            {
                this.isIndeterminate = value;
                this.OnPropertyChanged(nameof(this.IsIndeterminate));
            }
        }
    }
}
