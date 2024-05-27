
using Incas.Core.ViewModels;
using IncasEngine.TemplateManager;
using Models;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Incubator_2.ViewModels
{
    internal class VM_Template : BaseViewModel
    {
        private Template templateMain;
        private TemplateSettings templateSettings;
        public VM_Template(Template templ)
        {
            this.templateMain = templ;
            this.templateSettings = templ.GetTemplateSettings();
        }
        public void ApplyNewTemplate(Template templ)
        {
            this.templateMain = templ;
            this.templateSettings = templ.GetTemplateSettings();
            this.OnPropertyChanged(nameof(this.NameOfTemplate));
            this.OnPropertyChanged(nameof(this.Source));
            this.OnPropertyChanged(nameof(this.OnSavingScript));
            this.OnPropertyChanged(nameof(this.Category));
            this.OnPropertyChanged(nameof(this.ValidationScript));
            this.OnPropertyChanged(nameof(this.NumberPostfix));
        }
        public void SaveTemplate()
        {
            this.templateMain.SaveTemplateSettings(this.GetSettings());
            this.templateMain.UpdateTemplate();
        }
        public Template GetTemplate()
        {
            return this.templateMain;
        }
        public bool IsEdit
        {
            get
            {
                if (this.templateMain.id == 0)
                {
                    return false;
                }
                return true;
            }
        }
        public int Id
        {
            get
            {
                return this.templateMain.id;
            }
        }
        public TemplateType Type
        {
            get
            {
                return this.templateMain.type;
            }
            set
            {
                this.templateMain.type = value;
                this.OnPropertyChanged("Type");
            }
        }
        public string Parents
        {
            get
            {
                return this.templateMain.parent;
            }
            set
            {
                this.templateMain.parent = value;
                this.OnPropertyChanged("Parents");
            }
        }
        public string NameOfTemplate
        {
            get { return this.templateMain.name; }
            set
            {
                if (this.templateMain.name != value)
                {
                    this.templateMain.name = value;
                    this.OnPropertyChanged("NameOfTemplate");
                }
            }
        }
        public string Source
        {
            get { return this.templateMain.path; }
            set
            {
                if (this.templateMain.path != value)
                {
                    this.templateMain.path = value;
                    this.OnPropertyChanged("Source");
                }
            }
        }
        public string Category
        {
            get { return this.templateMain.suggestedPath; }
            set
            {
                if (this.templateMain.suggestedPath != value)
                {
                    this.templateMain.suggestedPath = value;
                    this.OnPropertyChanged("Category");
                }
            }
        }
        public bool CategoryEnabled
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.templateMain.parent);
            }
        }
        public string OnSavingScript
        {
            get
            {
                if (this.templateSettings.OnSaving != null)
                {
                    return this.templateSettings.OnSaving;
                }
                return "";
            }
            set
            {
                this.templateSettings.OnSaving = value;
                this.OnPropertyChanged(nameof(this.OnSavingScript));
            }
        }
        public string OnOpeningScript
        {
            get
            {
                if (this.templateSettings.OnOpening != null)
                {
                    return this.templateSettings.OnOpening;
                }
                return "";
            }
            set
            {
                this.templateSettings.OnOpening = value;
                this.OnPropertyChanged(nameof(this.OnOpeningScript));
            }
        }
        public string ValidationScript
        {
            get
            {
                if (this.templateSettings.Validation != null)
                {
                    return this.templateSettings.Validation;
                }
                return "";
            }
            set
            {
                this.templateSettings.Validation = value;
                this.OnPropertyChanged(nameof(this.ValidationScript));
            }
        }
        public string NumberPrefix
        {
            get
            {
                return this.templateSettings.NumberPrefix;
            }
            set
            {
                this.templateSettings.NumberPrefix = value;
                this.OnPropertyChanged(nameof(this.NumberPrefix));
            }
        }
        public string NumberPostfix
        {
            get
            {
                return this.templateSettings.NumberPostfix;
            }
            set
            {
                this.templateSettings.NumberPostfix = value;
                this.OnPropertyChanged(nameof(this.NumberPostfix));
            }
        }
        public string FileNameTemplate
        {
            get
            {
                return this.templateSettings.FileNameTemplate;
            }
            set
            {
                this.templateSettings.FileNameTemplate = value;
                this.OnPropertyChanged(nameof(this.FileNameTemplate));
            }
        }
        public bool RequiresSave
        {
            get
            {
                return this.templateSettings.RequiresSave;
            }
            set
            {
                this.templateSettings.RequiresSave = value;
                if (this.RequiresSave)
                {
                    this.NeverSave = false;
                }
                this.OnPropertyChanged(nameof(this.RequiresSave));
            }
        }
        public bool NeverSave
        {
            get
            {
                return this.templateSettings.PreventSave;
            }
            set
            {
                this.templateSettings.PreventSave = value;
                if (this.NeverSave)
                {
                    this.RequiresSave = false;
                }
                this.OnPropertyChanged(nameof(this.NeverSave));
            }
        }
        public TemplateSettings GetSettings()
        {
            return this.templateSettings;
        }
    }
}
