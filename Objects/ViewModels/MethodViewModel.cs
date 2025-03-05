using AvaloniaEdit.Document;
using Incas.Core.ViewModels;
using IncasEngine.ObjectiveEngine.Models;

namespace Incas.Objects.ViewModels
{
    public class MethodViewModel : BaseViewModel
    {
        public Method Source { get; set; }
        public MethodViewModel(Method method)
        {
            this.Source = method;
            this.textDocument = new()
            {
                Text = this.Source.Code ?? "\n\n\n\n\n"
            };
            this.textDocument.TextChanged += this.TextDocument_TextChanged;
        }

        private void TextDocument_TextChanged(object sender, System.EventArgs e)
        {
            this.Source.Code = this.textDocument.Text;
        }

        public string Name
        {
            get
            {
                return this.Source.Name;
            }
            set
            {
                this.Source.Name = value;
                this.OnPropertyChanged(nameof(this.Name));
            }
        }
        public string Code
        {
            get
            {
                return this.Source.Code;
            }
            set
            {
                this.Source.Code = value;
                this.OnPropertyChanged(nameof(this.Code));
            }
        }
        private TextDocument textDocument;
        public TextDocument CodeModule
        {
            get => this.textDocument;
            set
            {
                this.textDocument = value;
                this.Source.Code = value.Text;
                this.OnPropertyChanged(nameof(this.CodeModule));
            }
        }
    }
}