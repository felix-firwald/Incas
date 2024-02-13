using Common;
using DocumentFormat.OpenXml.Wordprocessing;
using Incubator_2.Common;
using Incubator_2.Models;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tag = Models.Tag;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для FileCreated.xaml
    /// </summary>
    public partial class FileCreated : UserControl
    {
        public readonly SGeneratedDocument record;
        public delegate void SelectorNotify(FileCreated my);
        public event SelectorNotify OnSelectorChecked;
        public event SelectorNotify OnSelectorUnchecked;
        public FileCreated(SGeneratedDocument rec, int counter = 1)
        {
            InitializeComponent();
            record = rec;
            this.Counter.Content = counter;
            this.Filename.Content = record.fileName;
            this.TemplateName.Text = record.templateName;
            this.GenerationTime.Content = record.generatedTime.ToString("f");
        }

        private void Selector_Checked(object sender, RoutedEventArgs e)
        {
            OnSelectorChecked(this);
        }

        private void Selector_Unchecked(object sender, RoutedEventArgs e)
        {
            OnSelectorUnchecked(this);
        }

        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            if (this.Selector.IsChecked == true)
            {
                this.Selector.IsChecked = false;
            }
            else
            {
                this.Selector.IsChecked = true;
            }
        }
        private void CreateFile(string folder)
        {
            using (Template templ = new())
            {
                using (Tag t = new())
                {
                    UC_FileCreator fc = new(templ.GetTemplateById(record.template), t.GetAllTagsByTemplate(record.template));
                    fc.ApplyRecord(record.fileName, record.GetFilledTags());
                    fc.CreateFile(folder, false, false);
                }
            }
        }

        private void CreateFileClick(object sender, RoutedEventArgs e)
        {
            ProgramState.ShowWaitCursor();
            string filename = $"{ProgramState.TemplatesRuntime}\\{record.fileName}.docx";
            CreateFile(ProgramState.TemplatesRuntime);
            ProgramState.ShowWaitCursor(false);
            try
            {
                System.Diagnostics.Process proc = new();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (Exception ex)
            {
                ProgramState.ShowErrorDialog($"Не удалось открыть файл:\n{ex}", "Действие невозможно");
            }
        }

        private void CreateFileForAnotherClick(object sender, RoutedEventArgs e)
        {
            string recipient = ProgramState.ShowActiveUserSelector("Выберите пользователя для отправки файла").slug;
            ProgramState.ShowWaitCursor();
            string filename = $"{record.fileName}.docx";
            CreateFile(ProgramState.Exchanges);
            ProgramState.ShowWaitCursor(false);
            ServerProcessor.SendOpenFileProcess(filename, ProgramState.Exchanges + "\\" + filename, recipient);
        }
    }
}
