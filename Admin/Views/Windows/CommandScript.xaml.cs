using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using Incas.Objects.Interfaces;
using IncasEngine.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Incas.Scripting.AutoUI;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace Incas.Admin.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CommandScript.xaml
    /// </summary>
    public partial class CommandScript : Window
    {
        public CommandScript(string name, string code)
        {
            this.InitializeComponent();
            this.Title = name;
            this.Code.Text = code;
            this.SetUpMarkdown();
        }
        private void SetUpMarkdown()
        {
            XmlReader reader = XmlReader.Create("Static\\Coding\\IncasPython.xshd");
            ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition highlightingDefinition = HighlightingLoader.Load(reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
            ICSharpCode.AvalonEdit.Highlighting.HighlightingRuleSet ruleSet = highlightingDefinition.MainRuleSet;
            List<string> keywords = ObjectScriptManager.GetKeywordsLibraries();
            foreach (string keyword in keywords)
            {
                string thisPattern = $@"\b{Regex.Escape(keyword)}\b";
                ICSharpCode.AvalonEdit.Highlighting.HighlightingRule thisRule = new()
                {
                    Regex = new System.Text.RegularExpressions.Regex(thisPattern)
                };
                HighlightingColor thisColor = new()
                {
                    Foreground = new SimpleHighlightingBrush(System.Windows.Media.Color.FromRgb(30, 144, 255))
                };
                thisRule.Color = thisColor;
                ruleSet.Rules.Add(thisRule);
            }

            this.Code.SyntaxHighlighting = highlightingDefinition;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void InsertTextAtCaret(string textToInsert)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.Invoke(() => this.InsertTextAtCaret(textToInsert));
                return;
            }

            TextArea textArea = this.Code.TextArea;
            TextDocument document = this.Code.Document;
            int caretOffset = textArea.Caret.Offset;
            document.Insert(caretOffset, textToInsert);
            textArea.Caret.Offset = caretOffset + textToInsert.Length;
        }
        private void ScriptHelperCreateObjectClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ScriptHelperCreateObject form = new();
            if (form.ShowDialog("Создание объекта", Core.Classes.Icon.Magic, DialogSimpleForm.Components.IconColor.Yellow) == true)
            {
                this.InsertTextAtCaret(form.GetScript());
            }
        }

        private void ScriptHelperCreateFormClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ScriptHelperCreateForm form = new();
            if (form.ShowDialog("Создание формы DSF", Core.Classes.Icon.Magic, DialogSimpleForm.Components.IconColor.Yellow) == true)
            {
                this.InsertTextAtCaret(form.GetScript());
            }
        }
        private void ScriptHelperCreateDialogClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ScriptHelperCreateDialog form = new();
            if (form.ShowDialog("Создание диалога", Core.Classes.Icon.Magic, DialogSimpleForm.Components.IconColor.Yellow) == true)
            {
                this.InsertTextAtCaret(form.GetScript());
            }
        }

        private void ScriptHelperCreateLoopClick(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void ScriptHelperReplaceClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ScriptHelperReplace form = new(this.Code);
            form.ShowDialog("Создание объекта", Core.Classes.Icon.Magic, DialogSimpleForm.Components.IconColor.Yellow);
        }

        private void ScriptHelpersGetConstantClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ScriptHelperGetConstant form = new();
            if (form.ShowDialog("Получение константы", Core.Classes.Icon.Magic, DialogSimpleForm.Components.IconColor.Yellow) == true)
            {
                this.InsertTextAtCaret(form.GetScript());
            }
        }

        private void ScriptHelperCreateIfClick(object sender, System.Windows.RoutedEventArgs e)
        {
            //ScriptHelperCreateIf form = new(this.Class);
            //if (form.ShowDialog("Генерация ветвления", Core.Classes.Icon.Magic, DialogSimpleForm.Components.IconColor.Yellow) == true)
            //{
            //    this.InsertTextAtCaret(form.GetScript());
            //}
        }
    }
}
