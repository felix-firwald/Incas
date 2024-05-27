
using Incas.Core.ViewModels;
using Incas.Templates.Components;
using Incas.Templates.Models;
using IncasEngine.TemplateManager;

namespace Incas.Templates.ViewModels
{
    internal class TemplateViewModel : BaseViewModel
    {
        private Template templateMain;
        private TemplateSettings templateSettings;
        public TemplateViewModel(Template templ)
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
        public bool IsEdit => this.templateMain.id != 0;
        public int Id => this.templateMain.id;
        public TemplateType Type
        {
            get => this.templateMain.type;
            set
            {
                this.templateMain.type = value;
                this.OnPropertyChanged("Type");
            }
        }
        public string Parents
        {
            get => this.templateMain.parent;
            set
            {
                this.templateMain.parent = value;
                this.OnPropertyChanged("Parents");
            }
        }
        public string NameOfTemplate
        {
            get => this.templateMain.name;
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
            get => this.templateMain.path;
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
            get => this.templateMain.suggestedPath;
            set
            {
                if (this.templateMain.suggestedPath != value)
                {
                    this.templateMain.suggestedPath = value;
                    this.OnPropertyChanged("Category");
                }
            }
        }
        public bool CategoryEnabled => string.IsNullOrWhiteSpace(this.templateMain.parent);
        public string OnSavingScript
        {
            get => this.templateSettings.OnSaving != null ? this.templateSettings.OnSaving : "";
            set
            {
                this.templateSettings.OnSaving = value;
                this.OnPropertyChanged(nameof(this.OnSavingScript));
            }
        }
        public string OnOpeningScript
        {
            get => this.templateSettings.OnOpening != null ? this.templateSettings.OnOpening : "";
            set
            {
                this.templateSettings.OnOpening = value;
                this.OnPropertyChanged(nameof(this.OnOpeningScript));
            }
        }
        public string ValidationScript
        {
            get => this.templateSettings.Validation != null ? this.templateSettings.Validation : "";
            set
            {
                this.templateSettings.Validation = value;
                this.OnPropertyChanged(nameof(this.ValidationScript));
            }
        }
        public string NumberPrefix
        {
            get => this.templateSettings.NumberPrefix;
            set
            {
                this.templateSettings.NumberPrefix = value;
                this.OnPropertyChanged(nameof(this.NumberPrefix));
            }
        }
        public string NumberPostfix
        {
            get => this.templateSettings.NumberPostfix;
            set
            {
                this.templateSettings.NumberPostfix = value;
                this.OnPropertyChanged(nameof(this.NumberPostfix));
            }
        }
        public string FileNameTemplate
        {
            get => this.templateSettings.FileNameTemplate;
            set
            {
                this.templateSettings.FileNameTemplate = value;
                this.OnPropertyChanged(nameof(this.FileNameTemplate));
            }
        }
        public bool RequiresSave
        {
            get => this.templateSettings.RequiresSave;
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
            get => this.templateSettings.PreventSave;
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
