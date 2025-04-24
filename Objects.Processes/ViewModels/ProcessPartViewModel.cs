using Incas.Core.ViewModels;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Processes.ClassComponents;
using System.Collections.ObjectModel;

namespace Incas.Objects.Processes.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class ProcessPartViewModel : BaseViewModel
    {

        public ClassViewModel BaseViewModel { get; set; }

        public ProcessClassData SourceData { get; set; }

        public ProcessPartViewModel(ClassViewModel vm)
        {
            this.BaseViewModel = vm;
            this.SourceData = (ProcessClassData)vm.SourceData;
            this.RequiredDocuments = new();
        }
        public void AddDocument(ClassItem ci)
        {
            this.RequiredDocuments.Add(ci);
        }
        private ObservableCollection<ClassItem> requiredDocs;
        public ObservableCollection<ClassItem> RequiredDocuments
        {
            get
            {
                return this.requiredDocs;
            }
            set
            {
                this.requiredDocs = value;
                this.OnPropertyChanged(nameof(this.RequiredDocuments));
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
