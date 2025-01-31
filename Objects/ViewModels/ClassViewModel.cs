using ICSharpCode.AvalonEdit.Document;
using Incas.Core.ViewModels;
using Incas.Objects.Components;
using Incas.Objects.Models;
using System.Collections.Generic;
using System.Windows;
using Field = Incas.Objects.Models.Field;

namespace Incas.Objects.ViewModels
{
    internal class ClassViewModel : BaseViewModel
    {
        public ClassData SourceData { get; set; }
        public Class Source;
        public ClassViewModel(Class source)
        {
            this.Source = source;
            this.SourceData = this.Source.GetClassData();
            this.textDocument = new()
            {
                Text = this.SourceData.Script ?? ""
            };
            this.textDocument.TextChanged += this.TextDocument_TextChanged;
        }

        private void TextDocument_TextChanged(object sender, System.EventArgs e)
        {
            this.SourceData.Script = this.textDocument.Text;
        }

        public ClassType Type
        {
            get => this.SourceData.ClassType;
            set
            {
                this.SourceData.ClassType = value;
                this.OnPropertyChanged(nameof(this.Type));
            }
        }
        public string NameOfClass
        {
            get => this.Source.Name;
            set
            {
                this.Source.Name = value
                    .Replace(":", "")
                    .Replace("#", "")
                    .Replace("$", "")
                    .Replace("\\", "")
                    .Replace("/", "")
                    .Replace("!", "");
                this.OnPropertyChanged(nameof(this.NameOfClass));
            }
        }
        public string CategoryOfClass
        {
            get => this.Source.Category;
            set
            {
                this.Source.Category = value;
                this.OnPropertyChanged(nameof(this.CategoryOfClass));
            }
        }
        public string NameTemplate
        {
            get => this.SourceData.NameTemplate;
            set
            {
                this.SourceData.NameTemplate = value;
                this.OnPropertyChanged(nameof(this.NameTemplate));
            }
        }
        public bool ShowCard
        {
            get => this.SourceData.ShowCard;
            set
            {
                this.SourceData.ShowCard = value;
                this.OnPropertyChanged(nameof(this.ShowCard));
            }
        }
        public bool PresetsEnabled
        {
            get => this.SourceData.PresetsEnabled;
            set
            {
                this.SourceData.PresetsEnabled = value;
                this.OnPropertyChanged(nameof(this.PresetsEnabled));
            }
        }
        public bool EditByAuthorOnly
        {
            get => this.SourceData.EditByAuthorOnly;
            set
            {
                this.SourceData.EditByAuthorOnly = value;
                this.OnPropertyChanged(nameof(this.EditByAuthorOnly));
            }
        }
        public bool AuthorOverrideEnabled
        {
            get => this.SourceData.AuthorOverrideEnabled;
            set
            {
                this.SourceData.AuthorOverrideEnabled = value;
                this.OnPropertyChanged(nameof(this.AuthorOverrideEnabled));
            }
        }
        public bool InsertTemplateName
        {
            get => this.SourceData.InsertTemplateName;
            set
            {
                this.SourceData.InsertTemplateName = value;
                this.OnPropertyChanged(nameof(this.InsertTemplateName));
            }
        }
        private TextDocument textDocument;
        public TextDocument CodeModule
        {
            get => this.textDocument;
            set
            {
                this.textDocument = value;
                this.SourceData.Script = value.Text;
                this.OnPropertyChanged(nameof(this.CodeModule));
            }
        }
        public Visibility DocumentClassVisibility
        {
            get
            {
                switch (this.SourceData.ClassType)
                {
                    case ClassType.Document:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }
        public Visibility ProcessClassVisibility
        {
            get
            {
                switch (this.SourceData.ClassType)
                {
                    case ClassType.Process:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            }
        }

        public void SetData(List<Field> fields)
        {
            this.SourceData.Fields = fields;
            this.Source.SetClassData(this.SourceData);
        }
    }
}
