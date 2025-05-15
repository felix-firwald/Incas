using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Incas.Admin.ViewModels;
using Incas.Core.Views.Windows;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using Incas.Scripting.AutoUI;
using IncasEngine.Scripting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MethodEditor.xaml
    /// </summary>
    public partial class MethodEditor : UserControl, IClassDetailsSettings
    {
        public MethodViewModel vm { get; set; }
        public IMembersContainerViewModel Class { get; set; }
        public GeneralizatorViewModel Generalizator { get; set; }
        public string ItemName { get; private set; }

        public MethodEditor(MethodViewModel method)
        {
            this.InitializeComponent();

            this.vm = method;
            this.ItemName = $"Настройка метода [{method.Name}]";
            this.DataContext = this.vm;
            this.Code.Text = method.Code;
            this.Code.TextArea.TextEntered += this.TextArea_TextEntered;
        }

        public void SetUpContext(IMembersContainerViewModel vm)
        {
            this.Class = vm;
            XmlReader reader = XmlReader.Create("Static\\Coding\\IncasPython.xshd");
            ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition highlightingDefinition = HighlightingLoader.Load(reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
            ICSharpCode.AvalonEdit.Highlighting.HighlightingRuleSet ruleSet = highlightingDefinition.MainRuleSet;
            foreach (IClassMemberViewModel field in vm.Members)
            {
                string pattern = @"\b" + Regex.Escape(field.Name) + @"\b";
                ICSharpCode.AvalonEdit.Highlighting.HighlightingRule rule = new()
                {
                    Regex = new System.Text.RegularExpressions.Regex(pattern)
                };
                HighlightingColor color = new()
                {
                    Foreground = new SimpleHighlightingBrush(System.Windows.Media.Color.FromRgb(191, 255, 76))
                };
                rule.Color = color;
                ruleSet.Rules.Add(rule);
            }
            List<string> keywords = ObjectScriptManager.GetKeywordsLibraries();
            keywords.Add("this");
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

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            this.MenuPanel.Children.Clear();
            if (e.Text == ".")
            {
                // Получаем позицию курсора
                int offset = this.Code.CaretOffset;
                string textBeforeDot = this.GetTextBeforeDot(offset);
                ContextMenu contextMenu = this.CreateContextMenu(textBeforeDot);
                if (contextMenu != null)
                {
                    contextMenu.PlacementTarget = this.Code;
                    contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint; // Показываем меню под курсором
                    contextMenu.IsOpen = true;
                }
            }
        }
        private ContextMenu CreateContextMenu(string textBeforeDot)
        {
            ContextMenu contextMenu = new();
            this.MenuPanel.Children.Clear();
            switch (textBeforeDot)
            {
                case "this":
                    foreach (IClassMemberViewModel member in this.Class.Members)
                    {
                        MenuItem item1 = new() { Header = member.Name };
                        if (member.ClassMemberType == IClassMemberViewModel.MemberType.Method)
                        {
                            item1.Click += (sender, e) => this.InsertText(member.Name + "()");
                        }
                        else
                        {
                            item1.Click += (sender, e) => this.InsertText(member.Name);
                        }
                        contextMenu.Items.Add(item1);
                    }
                    break;
                default:
                    Dictionary<string, Type> types = ObjectScriptManager.GetNETLibraries();
                    if (types.ContainsKey(textBeforeDot))
                    {
                        Type t = types[textBeforeDot];
                        foreach (PropertyInfo mi in t.GetProperties())
                        {
                            MenuItem item1 = new() { Header = mi.Name };
                            item1.Click += (sender, e) => this.InsertText(mi.Name);
                            this.MenuPanel.Children.Add(item1);
                        }
                        foreach (FieldInfo fi in t.GetFields())
                        {
                            MenuItem item1 = new() { Header = fi.Name };
                            item1.Click += (sender, e) => this.InsertText(fi.Name);
                            this.MenuPanel.Children.Add(item1);
                        }
                        foreach (MethodInfo mi in t.GetMethods())
                        {
                            if (mi.IsPublic)
                            {
                                string paramsText = "";
                                foreach (ParameterInfo pi in mi.GetParameters())
                                {
                                    paramsText += $"{pi.Name} - {pi.ParameterType} (optional: {pi.IsOptional})\n";
                                }
                                MenuItem item1 = new() { Header = mi.Name };
                                item1.Click += (sender, e) => this.InsertText(mi.Name + "()");
                                item1.ToolTip = paramsText;
                                this.MenuPanel.Children.Add(item1);
                            }
                        }
                    }
                    // Не показывать меню, если контекст не распознан
                    return null;
            }

            return contextMenu;
        }
        private void InsertText(string text)
        {
            int insertionOffset = this.Code.CaretOffset;
            this.Code.Document.Insert(insertionOffset, text);

            // Перемещаем каретку в конец вставленного текста. Важно пересчитать offset
            this.Code.CaretOffset = insertionOffset + text.Length;

            //Опционально: Фокусируем редактор чтобы каретка была видна
            this.Code.Focus();
        }
        private string GetTextBeforeDot(int offset)
        {
            TextDocument document = this.Code.Document;
            string text = document.GetText(0, offset); // Получаем текст до точки
            string pattern = @"(\w+)\s*\.$"; // Регулярное выражение: слово, пробелы, точка
            Match match = Regex.Match(text, pattern, RegexOptions.RightToLeft);

            if (match.Success)
            {
                return match.Groups[1].Value; // Возвращаем захваченную группу (слово)
            }
            else
            {
                return "";
            }
        }

        private void CodeChanged(object sender, System.EventArgs e)
        {
            this.vm.Code = this.Code.Text;
        }

        private void ChooseIconClick(object sender, System.Windows.RoutedEventArgs e)
        {
            IconSelector selector = new();
            selector.ShowDialog();
            if (selector.IsSelected)
            {
                this.vm.Icon = selector.SelectedIconPath;
            }
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
            ScriptHelperCreateIf form = new(this.Class);
            if (form.ShowDialog("Генерация ветвления", Core.Classes.Icon.Magic, DialogSimpleForm.Components.IconColor.Yellow) == true)
            {
                this.InsertTextAtCaret(form.GetScript());
            }
        }
    }
}
