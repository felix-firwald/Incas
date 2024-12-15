using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using DocumentFormat.OpenXml.Bibliography;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Exceptions;
using Incas.Objects.Models;
using Incas.Objects.ViewModels;
using Incas.Objects.Views.Controls;
using Incas.Templates.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Field = Incas.Objects.Models.Field;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateClass.xaml
    /// </summary>
    public partial class CreateClass : Window
    {
        private ClassViewModel vm;
        public CreateClass(ClassTypeSettings primary)
        {
            this.InitializeComponent();
            this.vm = new(new())
            {
                CategoryOfClass = primary.Category,
                NameOfClass = primary.Name,
                Type = (ClassType)primary.Selector.SelectedObject
            };
            if (this.vm.Type == ClassType.Document)
            {
                this.vm.ShowCard = true;
                this.TemplatesArea.Visibility = Visibility.Visible;
            }
            else
            {
                this.TemplatesArea.Visibility = Visibility.Collapsed;
            }
            this.DataContext = this.vm;
        }
        public CreateClass(Guid id)
        {
            XmlReader reader = XmlReader.Create("Static\\Coding\\IncasPython.xshd");        
            this.InitializeComponent();
            this.CodeModule.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            if (id == Guid.Empty)
            {
                this.Title = "(класс не выбран)";
                this.IsEnabled = false;
                return;
            }
            this.Title = "Редактирование класса";
            Class cl = new(id);
            this.vm = new(cl);
            this.DataContext = this.vm;
            foreach (Models.Field f in cl.GetClassData().Fields)
            {
                this.AddField(f);
            }
            this.TemplatesArea.Visibility = this.vm.Type == ClassType.Document ? Visibility.Visible : Visibility.Collapsed;
            this.UpdateStatusesList();
            this.UpdateTemplatesList();
        }

        private void GetMoreInfoClick(object sender, MouseButtonEventArgs e)
        {
            ProgramState.OpenWebPage("https://teletype.in/@incas/classes");
        }

        private void AddField(Incas.Objects.Models.Field data = null)
        {
            Controls.FieldCreator fc = new(this.ContentPanel.Children.Count, data);
            fc.OnRemove += this.Fc_OnRemove;
            fc.OnMoveDownRequested += this.Fc_OnMoveDownRequested;
            fc.OnMoveUpRequested += this.Fc_OnMoveUpRequested;
            this.ContentPanel.Children.Add(fc);
            Controls.FieldScriptViewer fsv = new(fc.vm);
            fsv.OnBindingActionRequested += this.Fsv_OnBindingActionRequested;
            fsv.OnBindingEventRequested += this.Fsv_OnBindingEventRequested;
            fsv.OnLinkInsertingRequested += this.Fsv_OnLinkInsertingRequested;
            this.ScriptFieldsPanel.Children.Add(fsv);
        }

        private void Fsv_OnLinkInsertingRequested(Field field)
        {
            this.CodeModule.SelectedText = $"self.{field.Name}";
        }

        private void Fsv_OnBindingEventRequested(Field field)
        {
            try
            {
                string searchText = ObjectiveScripting.GetPragmaStartEventsRegion();
                int position = this.CodeModule.Document.Text.IndexOf(searchText, 1);
                if (position == -1)
                {
                    DialogsManager.ShowExclamationDialog("Невозможно вставить метод в пустой модуль.", "Действие прервано");
                }
                string result = $"\n\tdef {field.Name}_changed(self):\n\t\tpass\n";
                this.CodeModule.Document.Insert(position + searchText.Length, result);
                this.CodeModule.SelectionStart = position + searchText.Length + result.Length;
            }
            catch { }
        }

        private void Fsv_OnBindingActionRequested(Field field)
        {
            try
            {
                string searchText = ObjectiveScripting.GetPragmaStartActionsRegion();
                int position = this.CodeModule.Document.Text.IndexOf(searchText, 1);
                if (position == -1)
                {
                    DialogsManager.ShowExclamationDialog("Невозможно вставить метод в пустой модуль.", "Действие прервано");
                }
                string result = $"\n\tdef {field.Name}_action(self):\n\t\tpass\n";
                this.CodeModule.Document.Insert(position + searchText.Length, result);
                this.CodeModule.SelectionStart = position + searchText.Length + result.Length;
            }
            catch { }
        }

        private int Fc_OnMoveUpRequested(Controls.FieldCreator t)
        {
            int position = this.ContentPanel.Children.IndexOf(t);
            if (position < this.ContentPanel.Children.Count - 1)
            {
                position += 1;
            }
            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private int Fc_OnMoveDownRequested(Controls.FieldCreator t)
        {
            int position = this.ContentPanel.Children.IndexOf(t);
            if (position > 0)
            {
                position -= 1;
            }

            this.ContentPanel.Children.Remove(t);
            this.ContentPanel.Children.Insert(position, t);
            return position;
        }

        private bool Fc_OnRemove(Controls.FieldCreator t)
        {
            BindingData data = new()
            {
                Class = this.vm.Source.identifier,
                Field = t.vm.Source.Id
            };
            if (data.Class == Guid.Empty || data.Field == Guid.Empty)
            {
                this.ContentPanel.Children.Remove(t);
                return true;
            }
            using Class cl = new();
            List<string> list = cl.FindBackReferencesNames(data);
            if (list.Count > 0)
            {
                DialogsManager.ShowExclamationDialog($"Поле невозможно удалить, поскольку на него ссылаются следующие классы:\n{string.Join(",\n", list)}", "Удаление невозможно");
                return false;
            }
            else
            {
                this.ContentPanel.Children.Remove(t);
                foreach (FieldScriptViewer viewer in this.ScriptFieldsPanel.Children)
                {
                    if (viewer.vm == t.vm)
                    {
                        this.ScriptFieldsPanel.Children.Remove(viewer);
                        break;
                    }
                }
            }
            return true;
        }

        private void AddFieldClick(object sender, MouseButtonEventArgs e)
        {
            this.AddField();
        }
        private void CopyFieldsFromAnotherClass(object sender, MouseButtonEventArgs e)
        {
            ClassSelector cs = new();
            if (cs.ShowDialog("Выбор класса", Core.Classes.Icon.Search))
            {
                ClassData cd = cs.GetSelectedClassData();
                foreach (Objects.Models.Field f in cd.Fields)
                {
                    f.Id = Guid.NewGuid();
                    this.AddField(f);
                }
            }
        }
        private void AddVirtualFieldsClick(object sender, MouseButtonEventArgs e)
        {
            VirtualFieldsAppender appender = new();
            if (appender.ShowDialog("Настройка виртуальных полей", Core.Classes.Icon.Magic))
            {
                foreach (Models.Field f in appender.GetFields())
                {
                    this.AddField(f);
                }
            }
        }
        private List<Models.Field> GetActualFields()
        {
            List<Models.Field> fields = [];
            List<string> names = [];
            foreach (Controls.FieldCreator item in this.ContentPanel.Children)
            {
                Models.Field f = item.GetField();
                if (names.Contains(f.Name))
                {
                    throw new FieldDataFailed($"Поле [{f.Name}] встречается более одного раза. Имена полей должны быть уникальными.");
                }
                else
                {
                    names.Add(f.Name);
                }
                f.SetId();
                fields.Add(f);
            }
            return fields;
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(this.vm.NameOfClass))
                {
                    DialogsManager.ShowExclamationDialog("Классу не присвоено наименование!", "Сохранение прервано");
                    return;
                }
                
                
                if (this.ContentPanel.Children.Count == 0)
                {
                    DialogsManager.ShowExclamationDialog("Класс не может не содержать полей.", "Сохранение прервано");
                    return;
                }
                List<Models.Field> fields = this.GetActualFields();
                if (string.IsNullOrWhiteSpace(this.vm.NameTemplate))
                {
                    FieldNameInsertor fn = new(fields);
                    if (fn.ShowDialog("Поле для наименования объектов", Core.Classes.Icon.Subscript))
                    {
                        this.vm.NameTemplate = $"{this.vm.NameOfClass} {fn.GetSelectedField()}";
                    }
                    else
                    {
                        this.vm.NameTemplate = $"{this.vm.NameOfClass} [{fields[0].Name}]";
                    }
                }
                this.vm.SetData(fields);
                this.vm.Source.Save();
                this.Close();
                //ProgramState.DatabasePage.UpdateAll();
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Сохранение прервано");
            }
        }
        #region Templates
        private void AddTemplateClick(object sender, MouseButtonEventArgs e)
        {
            TemplateClassEditor ce = new();
            ce.ShowDialog();
            if (ce.Result)
            {
                TemplateData td = new()
                {
                    Name = ce.SelectedName,
                    File = ce.SelectedPath
                };
                this.vm.SourceData.AddTemplate(td);
            }
            this.UpdateTemplatesList();
        }
        private void CopyTemplatesClick(object sender, MouseButtonEventArgs e)
        {
            ClassSelector cs = new();
            if (cs.ShowDialog("Выбор класса", Core.Classes.Icon.Search))
            {
                ClassData cd = cs.GetSelectedClassData();
                if (cd.Templates == null)
                {
                    return;
                }
                this.vm.SourceData.Templates = cd.Templates;
                //foreach (TemplateData sd in cd.Templates.Values)
                //{
                //    this.vm.SourceData.AddTemplate(sd);
                //}
            }
            this.UpdateTemplatesList();
        }
        private void UpdateTemplatesList()
        {
            this.TemplatesPanel.Children.Clear();
            if (this.vm.SourceData.Templates is null)
            {
                return;
            }
            foreach (KeyValuePair<int, TemplateData> template in this.vm.SourceData.Templates)
            {
                TemplateClassElement tce = new(template.Key, template.Value);
                tce.OnEdit += this.Tce_OnEdit;
                tce.OnRemove += this.Tce_OnRemove;
                tce.OnSearchInFileRequested += this.Tce_OnSearchInFileRequested;
                this.TemplatesPanel.Children.Add(tce);
            }
        }

        private void Tce_OnSearchInFileRequested(string path)
        {
            bool CheckNameUniqueness(string name)
            {
                foreach (Objects.Views.Controls.FieldCreator creator in this.ContentPanel.Children)
                {
                    if (creator.vm.Source.Name == name)
                    {
                        return false;
                    }
                }
                return true;
            }
            try
            {
                WordTemplator wt = new(path);
                foreach (string tagname in wt.FindAllTags())
                {
                    if (!CheckNameUniqueness(tagname))
                    {
                        continue;
                    }
                    Objects.Models.Field tag = new()
                    {
                        Name = tagname,
                        VisibleName = tagname.Replace("_", " ")
                    };
                    this.AddField(tag);
                }
            }
            catch (IOException)
            {
                DialogsManager.ShowErrorDialog("Файл занят другим процесом. Его использование невозможно.");
            }
        }

        private void Tce_OnRemove(int index, TemplateData data)
        {
            this.vm.SourceData.Templates.Remove(index);
            this.UpdateTemplatesList();
        }

        private void Tce_OnEdit(int index, TemplateData data)
        {
            this.vm.SourceData.EditTemplate(index, data);
            this.UpdateTemplatesList();
        }
        #endregion

        #region Statuses
        private void AddStatusClick(object sender, MouseButtonEventArgs e)
        {
            StatusSettings ss = new();
            if (ss.ShowDialog("Настройка статуса", Core.Classes.Icon.Tag) == true)
            {
                this.vm.SourceData.AddStatus(ss.GetData());
                this.UpdateStatusesList();
            }
        }
        private void CopyStatusesClick(object sender, MouseButtonEventArgs e)
        {
            ClassSelector cs = new();
            if (cs.ShowDialog("Выбор класса", Core.Classes.Icon.Search))
            {
                ClassData cd = cs.GetSelectedClassData();
                if (cd.Statuses == null)
                {
                    return;
                }
                this.vm.SourceData.Statuses = cd.Statuses;
                //foreach (StatusData sd in cd.Statuses.Values)
                //{
                //    this.vm.SourceData.AddStatus(sd);
                //}
            }
            this.UpdateStatusesList();
        }
        private void UpdateStatusesList()
        {
            this.StatusesPanel.Children.Clear();
            if (this.vm.SourceData.Statuses is null)
            {
                return;
            }
            int index = 0;
            foreach (StatusData data in this.vm.SourceData.Statuses.Values)
            {
                index++;
                StatusElement se = new(index, data);
                se.OnEdit += this.Se_OnEdit;
                se.OnRemove += this.Se_OnRemove;
                this.StatusesPanel.Children.Add(se);
            }
        }

        private void Se_OnRemove(int index, StatusData statusData)
        {
            this.vm.SourceData.RemoveStatus(index);
            this.UpdateStatusesList();
        }

        private void Se_OnEdit(int index, StatusData statusData)
        {
            this.vm.SourceData.Statuses[index] = statusData;
            this.UpdateStatusesList();
        }

        #endregion

        private void MinimizeAllClick(object sender, MouseButtonEventArgs e)
        {
            foreach (Incas.Objects.Views.Controls.FieldCreator item in this.ContentPanel.Children)
            {
                item.Minimize();
            }
        }

        private void MaximizeAllClick(object sender, MouseButtonEventArgs e)
        {
            foreach (Incas.Objects.Views.Controls.FieldCreator item in this.ContentPanel.Children)
            {
                item.Maximize();
            }
        }

        private void ShowFormClick(object sender, MouseButtonEventArgs e)
        {
            List<Incas.Objects.Models.Field> fields = [];
            try
            {
                foreach (Incas.Objects.Views.Controls.FieldCreator item in this.ContentPanel.Children)
                {
                    Incas.Objects.Models.Field f = item.GetField();
                    f.SetId();
                    fields.Add(f);
                }
                this.vm.SourceData.Fields = fields;
                ObjectsEditor oe = new(this.vm.Source, this.vm.SourceData);
                oe.ShowDialog();
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Предпросмотр прерван");
            }
        }

        private void InsertFieldNameClick(object sender, RoutedEventArgs e)
        {
            List<Field> fields = new();
            foreach (Controls.FieldCreator item in this.ContentPanel.Children)
            {
                fields.Add(item.vm.Source);
            }
            FieldNameInsertor fn = new(fields);
            if (fn.ShowDialog("Вставка имени", Core.Classes.Icon.Tags))
            {
                this.vm.NameTemplate = this.NameTemplate.Text + " " + fn.GetSelectedField();
            }
        }

        private void GenerateBaseScriptClick(object sender, RoutedEventArgs e)
        {
            string result = $"class {this.vm.Source.name.Replace(' ', '_')}:\n\tdef __init__(self, ";
            List<string> fieldsNames = new();
            string imports = "";
            string fieldsAllocation = "";
            List<Models.Field> fields = new();
            try
            {
                fields = this.GetActualFields();
                foreach (Models.Field field in fields)
                {                 
                    switch (field.Type)
                    {
                        case FieldType.Number:
                            fieldsNames.Add($"{field.Name}=0");
                            fieldsAllocation += $"\t\tself.{field.Name} = {field.Name} # {field.VisibleName}\n";
                            break;
                        case FieldType.Date:
                            fieldsNames.Add($"{field.Name}=None");
                            if (!imports.Contains("import datetime"))
                            {
                                imports += "import datetime\n";
                            }
                            fieldsAllocation += $"\t\tself.{field.Name} = {field.Name} # {field.VisibleName}\n";
                            //fieldsAllocation += $"\t\tself.{field.Name} = datetime.datetime.strptime({field.Name}, \"%d.%m.%Y %H:%M:%S\") # {field.VisibleName}\n";
                            break;
                        default:
                            fieldsNames.Add($"{field.Name}=None");
                            fieldsAllocation += $"\t\tself.{field.Name} = {field.Name} # {field.VisibleName}\n";
                            break;
                    }                   
                }
                result = imports + result;
                result += string.Join(", ", fieldsNames) + "):\n";
                result += fieldsAllocation;
                result += $"\n\n\t{ObjectiveScripting.GetPragmaStartMethodsRegion()}\n\t{ObjectiveScripting.GetPragmaEndMethodsRegion()}";
                result += $"\n\n\t{ObjectiveScripting.GetPragmaStartEventsRegion()}\n\t{ObjectiveScripting.GetPragmaEndEventsRegion()}";
                result += $"\n\n\t{ObjectiveScripting.GetPragmaStartActionsRegion()}\n\t{ObjectiveScripting.GetPragmaEndActionsRegion()}";
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Генерация прервана");
                return;
            }
            this.CodeModule.Text = result;         
        }
    }
}
