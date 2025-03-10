using Incas.Core.Classes;
using Incas.Objects.Components;
using Incas.Objects.ViewModels;
using Incas.Objects.Views.Controls;
using Incas.Rendering.Components;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для TableSettings.xaml
    /// </summary>
    public partial class TableSettings : Window
    {
        private string TableName { get; set; }
        public TableSettingsViewModel vm { get; set; }
        public TableSettings(Field field)
        {
            this.InitializeComponent();
            this.TableName = field.Name;
            this.Title = $"Редактирование таблицы ({field.Name})";
            if (field.TableSettings is null)
            {
                this.vm = new(new());
            }
            else
            {
                this.vm = new(field.TableSettings);
            }            
            this.DataContext = this.vm;
        }

        private void FinishClick(object sender, RoutedEventArgs e)
        {
            try
            {
                this.vm.Save();
                this.DialogResult = true;
            }
            catch (FieldDataFailed fail)
            {
                DialogsManager.ShowExclamationDialog(fail.Message, "Сохранение прервано");
            }
        }

        private void AddFieldClick(object sender, RoutedEventArgs e)
        {
            this.vm.AddColumn(new());
        }

        private void MinimizeAllClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void MaximizeAllClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void FindColumnsInFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new()
            {
                Filter = "Word и Excel|*.docx;*.xlsx",
                InitialDirectory = ProgramState.CurrentWorkspace.GetSourcesTemplatesFolder(),
            };
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    ITemplator templ;
                    if (fd.FileName.EndsWith(".docx"))
                    {
                        templ = new WordTemplator(fd.FileName);
                        this.ApplyColumnsFromTemplator(templ.FindTableTags(this.TableName));
                    }
                    else
                    {
                        templ = new ExcelTemplator(fd.FileName);
                        this.ApplyColumnsFromTemplator(templ.FindTableTags(this.TableName));
                    }
                }
                catch (IOException ioex)
                {
                    DialogsManager.ShowErrorDialog("При доступе к файл возникла ошибка. Описание ошибки: " + ioex.Message);
                }
                catch { }
            }
        }
        private void ApplyColumnsFromTemplator(List<string> columns)
        {
            foreach (string column in columns)
            {
                //TableFieldColumnData tf = new();
                //tf.Name = column;
                //tf.VisibleName = column.Replace("_", " ");
                //this.AddColumnCreator(tf);
            }
        }
    }
}
