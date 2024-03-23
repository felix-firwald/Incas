using Common;
using DocumentFormat.OpenXml.Wordprocessing;
using Incubator_2.Common;
using Incubator_2.Models;
using Incubator_2.Windows;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public SGeneratedDocument record { get; private set; }
        public delegate void SelectorNotify(FileCreated my);
        public event SelectorNotify OnSelectorChecked;
        public event SelectorNotify OnSelectorUnchecked;
        public FileCreated(SGeneratedDocument rec, int counter = 1)
        {
            InitializeComponent();
            record = rec;
            this.Counter.Content = counter;
            this.Filename.Content = record.fileName;
            this.AuthorName.Text = record.author;
            this.GenerationTime.Content = record.generatedTime.ToString("f") + ", Автор:";
            this.StatusBar.Value = (int)record.status;
            if (record.status == DocumentStatus.Done )
            {
                this.Selector.Visibility = Visibility.Collapsed;
            }
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
            if (this.Selector.Visibility == Visibility.Visible)
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
        }
        private void CreateFile(string folder)
        {
            using (Template templ = new())
            {
                using (Tag t = new())
                {
                    UC_FileCreator fc = new(templ.GetTemplateById(record.template), t.GetAllTagsByTemplate(record.template));
                    fc.ApplyRecord(record);
                    fc.CreateFile(folder, record.templateName, false, false);
                }
            }
        }

        private void CreateFileClick(object sender, RoutedEventArgs e)
        {
            ProgramState.ShowWaitCursor();
            string plus = DateTime.Now.ToString("HH.mm.ss ");
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

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            this.Selector.IsChecked = false;
            OnSelectorUnchecked?.Invoke(this);
            using (Template tm = new())
            {
                ProgramState.ShowWaitCursor();
                if (tm.GetTemplateById(this.record.template) != null)
                {
                    try
                    {
                        List<SGeneratedDocument> tj = [this.record];
                        UseTemplate ut = new UseTemplate(tm, tj);
                        ut.Show();
                    }
                    catch (IOException ex)
                    {
                        ProgramState.ShowWaitCursor(false);
                        ProgramState.ShowErrorDialog($"Файл поврежден или удален. Пожалуйста, попробуйте ещё раз.\n{ex}");
                    }
                }
            }
        }

        private void SetStatusClick(object sender, RoutedEventArgs e)
        {
            DocumentStatus newStatus = (DocumentStatus)Enum.Parse(typeof(DocumentStatus), ((MenuItem)sender).Tag.ToString(), true);
            if (newStatus < record.status)
            {
                ProgramState.ShowAccessErrorDialog("Нельзя откатить статус документа, не обладая правами администратора!");
                return;
            }
            this.Selector.IsChecked = false;
            using (GeneratedDocument gd = record.AsModel())
            {
                gd.status = newStatus;
                gd.UpdateRecord();
                record = gd.AsStruct();
                this.StatusBar.Value = (int)record.status;
            }

        }
    }
}
