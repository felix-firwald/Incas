using Incas.Core.ViewModels;
using System.Text.RegularExpressions;

namespace Incas.Miniservices.TextEditor.ViewModels
{
    public class TextEditorViewModel : BaseViewModel
    {
        private string source = "";
        private string result = "";

        private bool upperCase = false;
        private bool lowerCase = false;
        private bool firstletterCase = false;

        private bool removeUselessSpaces = false;
        private bool replaceNewLines = false;
        private bool replaceSpacesToLines = false;
        private bool fixPunctuation = false;

        public string SourceText
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
                this.OnPropertyChanged(nameof(this.SourceText));
                this.ConvertText();
            }
        }
        public string ResultText
        {
            get
            {
                return this.result;
            }
            set
            {
                this.result = value;
                this.OnPropertyChanged(nameof(this.ResultText));
            }
        }
        public bool UpperCase
        {
            get
            {
                return this.upperCase;
            }
            set
            {
                this.upperCase = value;
                if (this.upperCase == true)
                {
                    this.lowerCase = false;
                    this.OnPropertyChanged(nameof(this.LowerCase));
                }
                this.OnPropertyChanged(nameof(this.UpperCase));
                this.ConvertText();
            }
        }
        public bool LowerCase
        {
            get
            {
                return this.lowerCase;
            }
            set
            {
                this.lowerCase = value;
                if (this.lowerCase == true)
                {
                    this.upperCase = false;
                    this.OnPropertyChanged(nameof(this.UpperCase));
                }
                this.OnPropertyChanged(nameof(this.LowerCase));
                this.ConvertText();
            }
        }
        public bool FirstLetterCase
        {
            get
            {
                return this.firstletterCase;
            }
            set
            {
                this.firstletterCase = value;
                this.OnPropertyChanged(nameof(this.FirstLetterCase));
                this.ConvertText();
            }
        }
        public bool RemoveUselessSpaces
        {
            get
            {
                return this.removeUselessSpaces;
            }
            set
            {
                this.removeUselessSpaces = value;
                this.OnPropertyChanged(nameof(this.RemoveUselessSpaces));
                this.ConvertText();
            }
        }
        public bool ReplaceNewLines
        {
            get
            {
                return this.replaceNewLines;
            }
            set
            {
                this.replaceNewLines = value;
                this.OnPropertyChanged(nameof(this.ReplaceNewLines));
                this.ConvertText();
            }
        }
        public bool ReplaceSpacesToLines
        {
            get
            {
                return this.replaceSpacesToLines;
            }
            set
            {
                this.replaceSpacesToLines = value;
                this.OnPropertyChanged(nameof(this.ReplaceSpacesToLines));
                this.ConvertText();
            }
        }
        public bool FixPunctuation
        {
            get
            {
                return this.fixPunctuation;
            }
            set
            {
                this.fixPunctuation = value;
                this.OnPropertyChanged(nameof(this.FixPunctuation));
                this.ConvertText();
            }
        }

        public void ConvertText()
        {
            this.result = this.source;
            if (this.UpperCase)
            {
                this.result = this.result.ToUpper();
            }
            if (this.LowerCase)
            {
                this.result = this.result.ToLower();
            }
            if (this.FirstLetterCase)
            {

            }
            if (this.RemoveUselessSpaces)
            {
                this.result = Regex.Replace(this.result, @"  +", " ").Trim();
            }
            if (this.ReplaceNewLines)
            {
                this.result = Regex.Replace(this.result, @"\t|\n|\r", " ");
            }
            if (this.ReplaceSpacesToLines)
            {
                this.result = this.result.Replace(" ", "\n");
            }
            if (this.FixPunctuation)
            {
                this.result = Regex.Replace(this.result, @" ,|, ", ",").Replace(",", ", ");
                this.result = Regex.Replace(this.result, @" \.| \. ", ".").Replace(".", ". ");
                this.result = this.result.Replace(" - ", " — ");
            }
            this.OnPropertyChanged(nameof(this.ResultText));
        }
    }
}
