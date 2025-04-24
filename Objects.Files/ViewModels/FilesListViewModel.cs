using Incas.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Files.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class FilesListViewModel : BaseViewModel
    {
        
        public FilesListViewModel()
        {
            
        }
        public ObservableCollection<StorageObjectViewModel> Paths { get; set; }

        private StorageObjectViewModel selectedPath;
        public StorageObjectViewModel SelectedPath
        {
            get
            {
                return this.selectedPath;
            }
            set
            {
                this.selectedPath = value;
                this.OnPropertyChanged(nameof(this.SelectedPath));
            }
        }
        private StorageObjectViewModel selectedObject;
        public StorageObjectViewModel SelectedObject
        {
            get
            {
                return this.selectedObject;
            }
            set
            {
                this.selectedObject = value;
                this.OnPropertyChanged(nameof(this.SelectedObject));
            }
        }
    }
}
