using ClosedXML.Excel;
using Common;
using DocumentFormat.OpenXml.Office2021.PowerPoint.Designer;
using DocumentFormat.OpenXml.Spreadsheet;
using Incubator_2.Forms;
using Incubator_2.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;


namespace Incubator_2.Windows
{
    /// <summary>
    /// Логика взаимодействия для UseTemplate.xaml
    /// </summary>
    public partial class UseTemplate : Window
    {
        Template template;
        List<Template> children;
        List<Tag> tags;
        List<UC_FileCreator> creators = new List<UC_FileCreator>();
        public UseTemplate(Template t)
        {
            InitializeComponent();
            this.template = t;
            LoadTags();
            this.children = template.GetChildren();
            AddFileCreator();
            this.dir.Text = RegistryData.GetTemplatePreferredPath(this.template.id.ToString());
        }
        private void LoadTags()
        {
            Tag t = new Tag();
            tags = t.GetAllTagsByTemplate(template.id);
        }
        private List<string> GetChildrenChoice()
        {
            List<string> result = new List<string> { "Основной" };
            foreach (Template item in this.children)
            {
                result.Add(item.name);
            }
            return result;
        }

        private void AddFileCreator()
        {
            UC_FileCreator fc = new UC_FileCreator(tags, GetChildrenChoice());
            this.ContentPanel.Children.Add(fc);
            creators.Add(fc);
        }

        private bool ValidateContent()
        {
            if (!Directory.Exists(this.dir.Text))
            {
                ProgramState.ShowErrorDialog($"Папка, указанная в качестве выходной для генерации документов ({this.dir.Text}) не существует.\nУкажите существующую!", "Несуществующий выходной путь");
                return false;
            }
            return true;
        }

        private void CreateFilesClick(object sender, RoutedEventArgs e)
        {
            if (ValidateContent())
            {
                RegistryData.AddTemplate(this.template.id.ToString(), this.dir.Text, "", "");
                foreach (UC_FileCreator fc in creators)
                {
                    fc.CreateFile(this.dir.Text, template.path);
                }
            }
        }

        private void AddFC_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddFileCreator();
        }

        private void review_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fb = new FolderBrowserDialog();
            fb.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            if (fb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.dir.Text = fb.SelectedPath;
            }
        }
        private void MinimizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (UC_FileCreator fc in this.ContentPanel.Children)
            {
                fc.Minimize();
            }
        }

        private void MaximizeAll(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            foreach (UC_FileCreator fc in this.ContentPanel.Children)
            {
                fc.Maximize();
            }
        }
        private void RecalculateNamesClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            List<string> names = new List<string>();
            foreach (Tag tag in tags)
            {
                if (tag.type != TypeOfTag.LocalConstant && tag.type != TypeOfTag.Text)
                {
                    names.Add(tag.name);
                }
            }
            FilenamesRecalculator fr = new FilenamesRecalculator(names);
            fr.ShowDialog();
            if (fr.status == DialogStatus.Yes)
            {
                foreach (UC_FileCreator fc in this.creators)
                {
                    fc.RenameByTag(fr.SelectedTag, fr.Prefix, fr.Postfix);
                }
            }
        }

        private void GetFromExcel(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //OpenFileDialog fd = new OpenFileDialog();
            //fd.Filter = "MS Excel|*.xlsx";
            //if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    XLWorkbook wb = new XLWorkbook(fd.FileName);
            //    IXLWorksheet ws = wb.Worksheet(1);
            //    foreach (Tag tag in tags)
            //    {
            //        IXLCell colCell;
            //        try
            //        {
            //            colCell = ws.Search(tag.name, CompareOptions.IgnoreCase).First();
            //        }
            //        catch (Exception)
            //        {
            //            continue;
            //        }
            //        int columnNumber = colCell.WorksheetColumn().ColumnNumber();
            //        int rowNumber = colCell.WorksheetRow().RowNumber() + 1;
            //        for (int i = rowNumber; i < ws.LastRowUsed().RowNumber(); i++)
            //        {
            //            string ValueToInsert = ws.Cell(i, columnNumber).Value.ToString();
            //        }
            //    }
            //} 
        }
    }
}
