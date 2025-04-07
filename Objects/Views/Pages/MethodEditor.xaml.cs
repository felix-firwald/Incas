using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MethodEditor.xaml
    /// </summary>
    public partial class MethodEditor : UserControl, IClassDetailsSettings
    {
        public MethodViewModel vm { get; set; }
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

        public void SetUpContext(ClassViewModel vm)
        {
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

            this.Code.SyntaxHighlighting = highlightingDefinition;
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
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

            switch (textBeforeDot)
            {
                case "Декан":
                    // Добавляем пункты меню для MyObject
                    MenuItem item1 = new() { Header = "Property1" };
                    item1.Click += (sender, e) => this.InsertText("MyObject.Property1");

                    MenuItem item2 = new() { Header = "Method1()" };
                    item2.Click += (sender, e) => this.InsertText("MyObject.Method1()");

                    contextMenu.Items.Add(item1);
                    contextMenu.Items.Add(item2);
                    break;

                case "AnotherObject":
                    // Добавляем пункты меню для AnotherObject
                    MenuItem item3 = new() { Header = "PropertyA" };
                    item3.Click += (sender, e) => this.InsertText("AnotherObject.PropertyA");

                    contextMenu.Items.Add(item3);
                    break;

                default:
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
    }
}
