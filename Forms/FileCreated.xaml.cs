using Common;
using Incas.CreatedDocuments.Components;
using Incas.CreatedDocuments.Models;
using Incas.Templates.Components;
using Incas.Templates.Models;
using Incas.Templates.Views.Windows;
using Incas.Users.Models;
using Incubator_2.Common;
using Incubator_2.Forms.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tag = Incas.Templates.Models.Tag;


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
        public FileCreated(ref SGeneratedDocument rec, int counter = 1)
        {
            this.InitializeComponent();
            this.record = rec;
            this.Counter.Content = counter;
            this.SetTitle();
            this.AuthorName.Text = this.record.author;
            this.GenerationTime.Content = this.record.generatedTime.ToString("f") + ", Автор:";
            this.StatusBar.Value = (int)this.record.status;
            if (this.record.status == DocumentStatus.Done)
            {
                this.Selector.Visibility = Visibility.Collapsed;
            }
            this.SetTooltip();
        }

        private void SetTitle()
        {
            this.Filename.Content = $"N {this.record.fullNumber} {this.record.fileName}";
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
                this.Selector.IsChecked = this.Selector.IsChecked != true;
            }
        }
        private void CreateFile(string folder)
        {
            using Template templ = new();
            using Tag t = new();
            templ.GetTemplateById(this.record.template);
            TemplateSettings settings = templ.GetTemplateSettings();
            UC_FileCreator fc = new(templ, settings: ref settings, t.GetAllTagsByTemplate(this.record.template));
            fc.ApplyRecord(this.record);
            fc.CreateFile(folder, this.record.templateName, false, false);
        }

        private void CreateFileClick(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (this.record.status)
                {
                    case DocumentStatus.Done:
                        ProgramState.ShowExclamationDialog("Документ нельзя сгенерировать, если он находится на статусе \"Завершен\"");
                        return;
                }
                ProgramState.ShowWaitCursor();
                string plus = DateTime.Now.ToString("HH.mm.ss ");
                string filename = $"{ProgramState.TemplatesRuntime}\\{this.record.fileName}.docx";
                this.CreateFile(ProgramState.TemplatesRuntime);
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
            switch (this.record.status)
            {
                case DocumentStatus.Done:
                    ProgramState.ShowExclamationDialog("Документ нельзя сгенерировать, если он находится на статусе \"Завершен\"");
                    return;
            }
            Session session;
            if (ProgramState.ShowActiveUserSelector(out session, "Выберите пользователя для отправки файла"))
            {
                ProgramState.ShowWaitCursor();
                string filename = $"{this.record.fileName}.docx";
                this.CreateFile(ProgramState.Exchanges);
                ProgramState.ShowWaitCursor(false);
                ServerProcessor.SendOpenFileProcess(filename, ProgramState.Exchanges + "\\" + filename, session.slug);
            }
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            this.Selector.IsChecked = false;
            OnSelectorUnchecked?.Invoke(this);
            using Template tm = new();
            ProgramState.ShowWaitCursor();
            if (tm.GetTemplateById(this.record.template) != null)
            {
                try
                {
                    List<SGeneratedDocument> tj = [this.record];
                    UseTemplate ut = new(tm, tj);
                    ut.Show();
                }
                catch (IOException ex)
                {
                    ProgramState.ShowWaitCursor(false);
                    ProgramState.ShowErrorDialog($"Файл поврежден или удален. Пожалуйста, попробуйте ещё раз.\n{ex}");
                }
            }
        }
        private void SetTooltip()
        {
            string status = "";
            switch (this.record.status)
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
            if (newStatus < this.record.status && !Permission.IsUserHavePermission(PermissionGroup.Moderator))
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
                    if (!this.CheckUnique()) return;
                    break;
                case DocumentStatus.Printed:
                    this.Selector.Visibility = Visibility.Visible;
                    if (!this.CheckUnique()) return;
                    break;
                case DocumentStatus.Done:
                    if (!this.CheckUnique()) return;
                    this.Selector.Visibility = Visibility.Collapsed;
                    break;
            }
            this.Selector.IsChecked = false;
            using (GeneratedDocument gd = this.record.AsModel())
            {
                gd.status = newStatus;
                gd.UpdateRecord();
                this.record = gd.AsStruct();
                this.StatusBar.Value = (int)this.record.status;
            }
            this.SetTooltip();
        }

        private void ChangeNumberClick(object sender, RoutedEventArgs e)
        {
            switch (this.record.status)
            {
                case DocumentStatus.Printed:
                case DocumentStatus.Done:
                    ProgramState.ShowExclamationDialog("Статусы \"Распечатан\" и \"Завершен\" не позволяют менять номер документа.", "Номер неизменяем");
                    return;
            }
            string input = ProgramState.ShowInputBox("Новый номер", "Введите номер без префикса и постфикса");
            TemplateSettings settings = new Template(this.record.template).GetTemplateSettings();
            using (GeneratedDocument gd = this.record.AsModel())
            {
                gd.number = input;
                gd.fullNumber = settings.NumberPrefix + input + settings.NumberPostfix; ;
                gd.UpdateRecord();
                this.record = gd.AsStruct();
            }
            this.SetTitle();
        }
    }
}
