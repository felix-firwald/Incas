using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.AutoUI;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TemplateClassEditor.xaml
    /// </summary>
    public partial class TemplateClassEditor : Window
    {
        public bool Result = false;
        public string SelectedPath
        {
            get => this.Path.Text;
            set => this.Path.Text = value;
        }
        public string SelectedName
        {
            get => this.TemplateName.Text;
            set => this.TemplateName.Text = value;
        }

        public TemplateClassEditor()
        {
            this.InitializeComponent();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Path.Text) || string.IsNullOrWhiteSpace(this.TemplateName.Text))
            {
                DialogsManager.ShowExclamationDialog("Не все данные заполнены!", "Сохранение прервано");
                return;
            }
            this.Result = true;
            this.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new()
            {
                Filter = "Word и Excel|*.docx;*.xlsx",
                InitialDirectory = ProgramState.TemplatesSources
            };
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ApplySource(fd.FileName);
            }
        }
        private void ApplySource(string path)
        {
            string result = System.IO.Path.GetFileName(path);
            this.Path.Text = result;
            if (!path.StartsWith(ProgramState.TemplatesSources)) // если файл еще не в каталоге рабочего пространства
            {
                if (File.Exists(ProgramState.GetFullnameOfDocumentFile(result))) // если в каталоге рабочего пространства есть файл с таким же именем
                {
                    if (DialogsManager.ShowQuestionDialog($"Файл с именем \"{result}\" уже существует в рабочем пространстве. Вы хотите выбрать присвоить выбранному файлу другое имя или использовать уже существующий файл?", "Файл уже существует", "Переименовать выбранный", "Использовать существующий") == DialogStatus.Yes)
                    {
                        RenameSourceFile rs = new(result);
                        if (rs.ShowDialog("Новое имя файла", Core.Classes.Icon.Subscript))
                        {                        
                            this.Path.Text = rs.Name;
                            File.Copy(path, ProgramState.GetFullnameOfDocumentFile(rs.Name));
                        }                      
                    }
                }
                else
                {
                    File.Copy(path, ProgramState.GetFullnameOfDocumentFile(result));
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
