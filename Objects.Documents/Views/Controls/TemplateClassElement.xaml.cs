using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.Views.Windows;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Objects.Documents.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TemplateClassElement.xaml
    /// </summary>
    public partial class TemplateClassElement : UserControl
    {
        private TemplateData data;
        private int index;
        public delegate void TemplateElementAction(int index, TemplateData data);
        public delegate void TemplateFileAction(string path);
        public event TemplateElementAction OnEdit;
        public event TemplateElementAction OnRemove;
        public event TemplateFileAction OnSearchInFileRequested;
        public TemplateClassElement(int index, TemplateData data)
        {
            this.InitializeComponent();
            this.index = index;
            this.data = data;
            this.MainLabel.Content = data.Name;
        }

        private void EditClick(object sender, MouseButtonEventArgs e)
        {
            TemplateClassEditor editor = new()
            {
                SelectedName = this.data.Name,
                SelectedPath = this.data.File
            };
            editor.ShowDialog();
            this.data.Name = editor.SelectedName;
            this.data.File = editor.SelectedPath;
            this.OnEdit?.Invoke(this.index, this.data);
        }

        private void RemoveClick(object sender, MouseButtonEventArgs e)
        {
            this.OnRemove?.Invoke(this.index, this.data);
        }

        private void ViewFileClick(object sender, MouseButtonEventArgs e)
        {
            string pathFile = ProgramState.CurrentWorkspace.GetFullnameOfDocumentFile(this.data.File);

            if (!File.Exists(pathFile))
            {
                DialogsManager.ShowExclamationDialog($"Файл ({this.data.File}) не существует!", "Действие прервано");
                return;
            }
            try
            {
                System.Diagnostics.Process proc = new();
                proc.StartInfo.FileName = pathFile;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog($"При попытке открытия файла возникла ошибка:\n{ex}");
            }
        }

        private void SearchFields(object sender, MouseButtonEventArgs e)
        {
            string pathFile = ProgramState.CurrentWorkspace.GetFullnameOfDocumentFile(this.data.File);

            if (!File.Exists(pathFile))
            {
                DialogsManager.ShowExclamationDialog($"Файл ({this.data.File}) не существует!", "Действие прервано");
                return;
            }
            this.OnSearchInFileRequested?.Invoke(pathFile);
        }
    }
}
