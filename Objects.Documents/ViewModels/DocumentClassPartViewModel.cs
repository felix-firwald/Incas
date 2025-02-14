using Incas.Core.ViewModels;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System.Collections.ObjectModel;
using System.Windows;

namespace Incas.Objects.Documents.ViewModels
{
    public class DocumentClassPartViewModel : BaseViewModel
    {
        public DocumentClassData SourceData { get; set; }
        public DocumentClassPartViewModel(ClassViewModel vm)
        {
            this.SourceData = (DocumentClassData)vm.SourceData;
            if (this.SourceData.Documents is null)
            {
                this.Templates = new();
            }
            else
            {
                this.Templates = new();
                foreach (Template doc in this.SourceData.Documents)
                {
                    this.Templates.Add(new(doc));
                }
            }
        }
        private ObservableCollection<TemplateViewModel> templates;
        public ObservableCollection<TemplateViewModel> Templates
        {
            get
            {
                return this.templates;
            }
            set
            {
                this.templates = new(value);
                this.OnPropertyChanged(nameof(this.Templates));
            }
        }
        public void UpdateAll()
        {
            this.OnPropertyChanged(nameof(this.Templates));
        }
        private TemplateViewModel selectedTemplate;
        public TemplateViewModel SelectedTemplate
        {
            get
            {
                return this.selectedTemplate;
            }
            set
            {
                this.selectedTemplate = value;
                this.OnPropertyChanged(nameof(this.SelectedTemplate));
                this.OnPropertyChanged(nameof(this.DetailsVisibility));
                this.OnPropertyChanged(nameof(this.TemplatePathSelected));
            }
        }
        public Visibility DetailsVisibility
        {
            get
            {
                bool visible = this.SelectedTemplate != null;
                return this.FromBool(visible);
            }
        }
        public bool TemplatePathSelected
        {
            get
            {
                return !string.IsNullOrEmpty(this.SelectedTemplate?.Source.File);
            }
        }
        public bool InsertTemplateName
        {
            get
            {
                return this.SourceData.InsertTemplateName;
            }
            set
            {
                this.SourceData.InsertTemplateName = value;
                this.OnPropertyChanged(nameof(this.InsertTemplateName));
            }
        }
        public void MinimizeAll()
        {

        }
        public void MaximizeAll()
        {

        }
        public void Save()
        {
            this.SourceData.Documents = new();
            foreach (TemplateViewModel template in this.Templates)
            {
                template.Save();
                this.SourceData.Documents.Add(template.Source);
            }
        }
    }
}
