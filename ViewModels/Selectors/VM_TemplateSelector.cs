using Incas.Core.ViewModels;
using IncasEngine.TemplateManager;
using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels.Selectors
{

    public class VM_TemplateSelector : BaseViewModel
    {
        private STemplate selectedTemplate;
        private string helptext = "Выбранному пользователю будет послан процесс от вашего имени.";
        public VM_TemplateSelector()
        {

        }
        private TemplateType _templateType;
        public TemplateType TemplateType
        {
            get
            {
                return this._templateType;
            }
            set
            {
                this._templateType = value;
                this.OnPropertyChanged(nameof(this.TemplateType));
            }
        }

        public string HelpTextTitle
        {
            get
            {
                return this.helptext;
            }
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
            set
            {
                this.OnPropertyChanged(nameof(this.Templates));

            }
        }

        public STemplate SelectedTemplate
        {
            get { return this.selectedTemplate; }
            set
            {
                this.selectedTemplate = value;
                this.OnPropertyChanged(nameof(this.SelectedTemplate));
            }
        }
    }
}
