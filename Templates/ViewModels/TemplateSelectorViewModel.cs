using Incas.Core.ViewModels;
using Incas.Templates.Components;
using Incas.Templates.Models;
using System.Collections.Generic;

namespace Incas.Templates.ViewModels
{

    public class TemplateSelectorViewModel : BaseViewModel
    {
        private STemplate selectedTemplate;
        private string helptext = "Выбранному пользователю будет послан процесс от вашего имени.";
        public TemplateSelectorViewModel()
        {

        }
        private TemplateType _templateType;
        public TemplateType TemplateType
        {
            get => this._templateType;
            set
            {
                this._templateType = value;
                this.OnPropertyChanged(nameof(this.TemplateType));
            }
        }

        public string HelpTextTitle
        {
            get => this.helptext;
            set
            {
                this.helptext = value;
                this.OnPropertyChanged(nameof(this.HelpTextTitle));
            }
        }
        public List<STemplate> Templates
        {
            get
            {
                using Template t = new();
                switch (this.TemplateType)
                {
                    case TemplateType.Word:
                    default:
                        return t.GetAllWordTemplates();
                    case TemplateType.Text:
                        return t.GetAllTextTemplates();
                }
            }
            set => this.OnPropertyChanged(nameof(this.Templates));
        }

        public STemplate SelectedTemplate
        {
            get => this.selectedTemplate;
            set
            {
                this.selectedTemplate = value;
                this.OnPropertyChanged(nameof(this.SelectedTemplate));
            }
        }
    }
}
