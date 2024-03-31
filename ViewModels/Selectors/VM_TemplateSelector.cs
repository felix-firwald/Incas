using Models;
using System.Collections.Generic;

namespace Incubator_2.ViewModels.Selectors
{

    public class VM_TemplateSelector : VM_Base
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
                return _templateType;
            }
            set
            {
                _templateType = value;
                OnPropertyChanged(nameof(TemplateType));
            }
        }

        public string HelpTextTitle
        {
            get
            {
                return helptext;
            }
            set
            {
                helptext = value;
                OnPropertyChanged(nameof(HelpTextTitle));
            }
        }
        public List<STemplate> Templates
        {
            get
            {
                using (Template t = new Template())
                {
                    switch (TemplateType)
                    {
                        case TemplateType.Word:
                        default:
                            return t.GetAllWordTemplates();
                        case TemplateType.Text:
                            return t.GetAllTextTemplates();
                    }
                }
            }
            set
            {
                OnPropertyChanged(nameof(Templates));

            }

        }

        public STemplate SelectedTemplate
        {
            get { return selectedTemplate; }
            set
            {
                selectedTemplate = value;
                OnPropertyChanged(nameof(SelectedTemplate));
            }
        }
    }
}
