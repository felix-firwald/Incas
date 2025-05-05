using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Incas.Admin.ViewModels;
using Incas.Objects.Documents.ViewModels;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Incas.Objects.Documents.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для DocumentTemplateScriptSettings.xaml
    /// </summary>
    public partial class DocumentTemplateScriptSettings : UserControl, IClassDetailsSettings
    {
        public ClassViewModel Source { get; set; }
        public TemplateViewModel SourceTemplate { get; set; }
        public DocumentTemplateScriptSettings(TemplateViewModel templ)
        {
            this.InitializeComponent();
            this.SourceTemplate = templ;
            this.Source = templ.ClassViewModel;
            this.Code.Text = templ.Code;
        }

        public string ItemName => $"Скрипт [{this.SourceTemplate.Name}]";

        public void SetUpContext(ClassViewModel vm)
        {
            this.Source = vm;
            XmlReader reader = XmlReader.Create("Static\\Coding\\IncasPython.xshd");
            ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition highlightingDefinition = HighlightingLoader.Load(reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
            ICSharpCode.AvalonEdit.Highlighting.HighlightingRuleSet ruleSet = highlightingDefinition.MainRuleSet;

            foreach (FieldViewModel field in vm.Fields)
            {
                ICSharpCode.AvalonEdit.Highlighting.HighlightingRule rule = new()
                {
                    Regex = new System.Text.RegularExpressions.Regex(@"\b" + System.Text.RegularExpressions.Regex.Escape(field.Name) + @"\b")
                };
                HighlightingColor color = new()
                {
                    Foreground = new SimpleHighlightingBrush(Color.FromRgb(191, 255, 76))
                };
                rule.Color = color;
                color.Background = new SimpleHighlightingBrush(Color.FromRgb(47, 52, 34));
                ruleSet.Rules.Add(rule);
            }
            foreach (PropertyViewModel field in this.SourceTemplate.Properties)
            {
                ICSharpCode.AvalonEdit.Highlighting.HighlightingRule rule = new()
                {
                    Regex = new System.Text.RegularExpressions.Regex(@"\b" + System.Text.RegularExpressions.Regex.Escape(field.PropertyName) + @"\b")
                };
                HighlightingColor color = new()
                {
                    Foreground = new SimpleHighlightingBrush(Color.FromRgb(191, 255, 76))
                };
                rule.Color = color;
                color.Background = new SimpleHighlightingBrush(Color.FromRgb(47, 52, 34));
                ruleSet.Rules.Add(rule);
            }

            this.Code.SyntaxHighlighting = highlightingDefinition;
        }

        private void CodeChanged(object sender, EventArgs e)
        {
            this.SourceTemplate.Code = this.Code.Text;
        }

        public void SetUpContext(GeneralizatorViewModel vm)
        {
            throw new NotImplementedException();
        }
    }
}
