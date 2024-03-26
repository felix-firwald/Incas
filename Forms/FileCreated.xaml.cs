using Common;
using Incubator_2.Common;
using Incubator_2.Forms.Templates;
using Incubator_2.Models;
using Incubator_2.Windows;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tag = Models.Tag;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для FileCreated.xaml
    /// Для долбоебов которые считают себя умнее других
    /// этот контрол НЕ ИМЕЕТ ПРАВА использовать MVVM паттерн
    /// его на странице могут быть одновременно открыты в количестве > 200 штук
    /// 2 сотни сука инстансов на 200 мелких контролов? ага, щас
    /// пока эта хуета загрузится в память юзер постареет
    /// а куча НЕ РЕЗИНОВАЯ БЛЯТЬ и она запомнит этот финт ушами надолго
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
            SetTitle();
            this.AuthorName.Text = record.author;
            this.GenerationTime.Content = record.generatedTime.ToString("f") + ", Автор:";
            this.StatusBar.Value = (int)record.status;
            if (record.status == DocumentStatus.Done)
            {
                this.Selector.Visibility = Visibility.Collapsed;
            }
            SetTooltip();
        }

        private void SetTitle()
        {
            this.Filename.Content = $"N {record.fullNumber} {record.fileName}";
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
            try
            {
                switch (record.status)
                {
                    case DocumentStatus.Done:
                        ProgramState.ShowExclamationDialog("Документ нельзя сгенерировать, если он находится на статусе \"Завершен\"");
                        return;
                }
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
            catch (GeneratorUndefinedStateException ex)
            {
                ProgramState.ShowExclamationDialog(ex.Message);
            }
        }

        private void CreateFileForAnotherClick(object sender, RoutedEventArgs e)
        {
            switch (record.status)
            {
                case DocumentStatus.Done:
                    ProgramState.ShowExclamationDialog("Документ нельзя сгенерировать, если он находится на статусе \"Завершен\"");
                    return;
            }
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
        private void SetTooltip()
        {
            string status = "";
            switch (record.status)
            {
                case DocumentStatus.Draft:
                    status = "Черновик\nДокумент можно полностью редактировать";
                    break;
                case DocumentStatus.Created:
                    status = "Создан\nДокумент можно полностью редактировать";
                    break;
                case DocumentStatus.Approved:
                    status = "Утвержден\nНомер документа изменить нельзя, поля редактировать можно";
                    break;
                case DocumentStatus.Printed:
                    status = "Распечатан\nНомер документа изменить нельзя, поля редактировать можно";
                    break;
                case DocumentStatus.Done:
                    status = "Готов (завершен)\nДокумент нельзя отредактировать или удалить";
                    break;
            }
            this.ToolTip = "Статус документа: " + status;
        }
        private bool CheckUnique()
        {
            bool result = this.record.AsModel().CheckUniqueNumber();
            if (!result)
            {
                ProgramState.ShowExclamationDialog("Изменение статуса невозможно: номер документа не уникален!", "Сохранение прервано");
            }
            return result;
        }

        private void SetStatusClick(object sender, RoutedEventArgs e)
        {
            DocumentStatus newStatus = (DocumentStatus)Enum.Parse(typeof(DocumentStatus), ((MenuItem)sender).Tag.ToString(), true);
            if (newStatus < record.status && !Permission.IsUserHavePermission(PermissionGroup.Moderator))
            {
                ProgramState.ShowAccessErrorDialog("Нельзя откатить статус документа, не обладая правами модератора!");
                return;
            }
            switch (newStatus)
            {
                case DocumentStatus.Created:
                    this.Selector.Visibility = Visibility.Visible;
                    break;
                case DocumentStatus.Approved:
                    this.Selector.Visibility = Visibility.Visible;
                    if (!CheckUnique()) return;
                    break;
                case DocumentStatus.Printed:
                    this.Selector.Visibility = Visibility.Visible;
                    if (!CheckUnique()) return;
                    break;
                case DocumentStatus.Done:
                    if (!CheckUnique()) return;
                    this.Selector.Visibility = Visibility.Collapsed;
                    break;
            }
            this.Selector.IsChecked = false;
            using (GeneratedDocument gd = record.AsModel())
            {
                gd.status = newStatus;
                gd.UpdateRecord();
                record = gd.AsStruct();
                this.StatusBar.Value = (int)record.status;
            }
            SetTooltip();
        }

        private void ChangeNumberClick(object sender, RoutedEventArgs e)
        {
            switch (record.status)
            {
                case DocumentStatus.Printed:
                case DocumentStatus.Done:
                    ProgramState.ShowExclamationDialog("Статусы \"Распечатан\" и \"\" не позволяют менять номер документа.", "Номер неизменяем");
                    return;
            }
            string input = ProgramState.ShowInputBox("Новый номер", "Введите номер без префикса и постфикса");
            TemplateSettings settings = new Template(this.record.template).GetTemplateSettings();
            using (GeneratedDocument gd = record.AsModel())
            {
                gd.number = input;
                gd.fullNumber = settings.NumberPrefix + input + settings.NumberPostfix; ;
                gd.UpdateRecord();
                record = gd.AsStruct();
            }
            SetTitle();
        }
    }
}
