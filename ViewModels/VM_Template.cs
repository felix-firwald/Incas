
using Models;

namespace Incubator_2.ViewModels
{
    class VM_Template : VM_Base
    {
        private Template template_main;
        private TemplateSettings templateSettings;
        public VM_Template(Template templ)
        {
            this.template_main = templ;
            this.templateSettings = templ.GetTemplateSettings();
        }
        public bool IsEdit
        {
            get
            {
                if (template_main.id == 0)
                {
                    return false;
                }
                return true;
            }
        }
        public string NameOfTemplate
        {
            get { return this.template_main.name; }
            set
            {
                if (this.template_main.name != value)
                {
                    this.template_main.name = value;
                    OnPropertyChanged("NameOfTemplate");
                }
            }
        }
        public string Source
        {
            get { return this.template_main.path; }
            set
            {
                if (this.template_main.path != value)
                {
                    this.template_main.path = value;
                    OnPropertyChanged("Source");
                }
            }
        }
        public string Category
        {
            get { return this.template_main.suggestedPath; }
            set
            {
                if (this.template_main.suggestedPath != value)
                {
                    this.template_main.suggestedPath = value;
                    OnPropertyChanged("Category");
                }
            }
        }
        public string OnSavingScript
        {
            get
            {
                if (templateSettings.OnSaving != null)
                {
                    return templateSettings.OnSaving;
                }
                return "";
            }
            set
            {
                templateSettings.OnSaving = value;
                OnPropertyChanged(nameof(OnSavingScript));
            }
        }
        public string ValidationScript
        {
            get
            {
                if (templateSettings.Validation != null)
                {
                    return templateSettings.Validation;
                }
                return "";
            }
            set
            {
                templateSettings.Validation = value;
                OnPropertyChanged(nameof(ValidationScript));
            }
        }
        public string NumberPrefix
        {
            get
            {
                return templateSettings.NumberPrefix;
            }
            set
            {
                templateSettings.NumberPrefix = value;
                OnPropertyChanged(nameof(NumberPrefix));
            }
        }
        public string NumberPostfix
        {
            get
            {
                return templateSettings.NumberPostfix;
            }
            set
            {
                templateSettings.NumberPostfix = value;
                OnPropertyChanged(nameof(NumberPostfix));
            }
        }
        public TemplateSettings GetSettings()
        {
            return templateSettings;
        }
    }
}
