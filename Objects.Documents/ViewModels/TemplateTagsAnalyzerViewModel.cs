using Incas.Core.ViewModels;
using Incas.Objects.Documents.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incas.Objects.Documents.ViewModels
{
    /// <summary>
    /// Класс ViewModel.
    /// Метод Save вызывается после применения изменений на форме и подтверждает сохранение изменений из ViewModel в Model.
    /// </summary>
    public class TemplateTagsAnalyzerViewModel : BaseViewModel
    {
        /// <summary>
        /// Model
        /// </summary>
        public object Source { get; set; }

        public TemplateTagsAnalyzerViewModel(List<string> source)
        {
            this.Source = source;
            this.FoundItems = new();
            foreach (string item in source)
            {
                FoundTemplateTagViewModel vmItem = new(item);
                this.FoundItems.Add(vmItem);
            }
        }
        private ObservableCollection<FoundTemplateTagViewModel> templateTags;
        public ObservableCollection<FoundTemplateTagViewModel> FoundItems
        {
            get
            {
                return this.templateTags;
            }
            set
            {
                this.templateTags = value;
                this.OnPropertyChanged(nameof(this.FoundItems));
            }
        }

    }
}
