using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using Incas.Rendering.Components;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Models;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateClass.xaml
    /// </summary>
    public partial class CreateClass : Window
    {
        private ClassViewModel vm;
        private IClassPartSettings partSettings;
        public CreateClass(ClassTypeSettings primary) // init class
        {
            DialogsManager.ShowWaitCursor();
            XmlReader reader = XmlReader.Create("Static\\Coding\\IncasPython.xshd");
            this.InitializeComponent();
            this.CodeModule.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            Class @class = new();
            @class.Name = primary.Name;
            @class.Component = primary.GetComponent();
            @class.Type = (ClassType)primary.Selector.SelectedObject;
            @class.Parents = primary.GetParents();

            this.vm = new(@class);
            this.vm.OnDrawCalling += this.Vm_OnDrawCalling;
            if (this.vm.Type == ClassType.Document)
            {
                this.vm.ShowCard = true;
            }
            this.DataContext = this.vm;
            this.ApplyPartSettings();
            DialogsManager.ShowWaitCursor(false);
        }
        public CreateClass(Guid id) // edit class
        {
            DialogsManager.ShowWaitCursor();
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
            Class cl = EngineGlobals.GetClass(id);
            this.vm = new(cl);
            this.vm.OnDrawCalling += this.Vm_OnDrawCalling;
            this.DataContext = this.vm;
            this.ApplyPartSettings();
            DialogsManager.ShowWaitCursor(false);
        }

        

        public CreateClass(ServiceClass @class) // edit service class
        {
#if !E_FREE
            DialogsManager.ShowWaitCursor();
            XmlReader reader = XmlReader.Create("Static\\Coding\\IncasPython.xshd");
            this.InitializeComponent();
            this.CodeModule.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            this.Title = $"Служебный класс: {@class.Name}";
            this.vm = new(@class);
            this.vm.OnDrawCalling += this.Vm_OnDrawCalling;
            this.DataContext = this.vm;
            this.ApplyPartSettings();
            DialogsManager.ShowWaitCursor(false);
#endif
        }
        private void ApplyPartSettings()
        {
            this.partSettings = ServiceExtensionFieldsManager.GetPartSettingsByType(this.vm);
            if (this.partSettings is not null)
            {
                TabItem item = new()
                {
                    Content = this.partSettings,
                    Header = this.partSettings.ItemName,
                    BorderBrush = this.FindResource("LightPurple") as Brush
                };
                this.TabControlMain.Items.Add(item);
            }
        }
        private void GetMoreInfoClick(object sender, RoutedEventArgs e)
        {
            //ProgramState.OpenWebPage("https://teletype.in/@incas/classes");
            DialogsManager.ShowHelp(Help.Components.HelpType.Classes_General);
        }

        private void AddField(Field data = null)
        {
            //Controls.FieldCreator fc = new(this.ContentPanel.Children.Count, data);
            //fc.OnRemove += this.Fc_OnRemove;
            //fc.OnMoveDownRequested += this.Fc_OnMoveDownRequested;
            //fc.OnMoveUpRequested += this.Fc_OnMoveUpRequested;
            //this.ContentPanel.Children.Add(fc);
            //Controls.FieldScriptViewer fsv = new(fc.vm);
            //fsv.OnBindingActionRequested += this.Fsv_OnBindingActionRequested;
            //fsv.OnBindingEventRequested += this.Fsv_OnBindingEventRequested;
            //fsv.OnLinkInsertingRequested += this.Fsv_OnLinkInsertingRequested;
            //this.ScriptFieldsPanel.Children.Add(fsv);
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

        private void AddFieldClick(object sender, RoutedEventArgs e)
        {
            this.vm.Fields.Add(new(this.vm));
            //this.AddField();
        }
        private void CopyFieldsFromAnotherClass(object sender, RoutedEventArgs e)
        {
            ClassSelector cs = new();
            if (cs.ShowDialog("Выбор класса", Core.Classes.Icon.Search))
            {
                IClassData cd = cs.GetSelectedClassData();
                foreach (Field f in cd.Fields)
                {
                    f.Id = Guid.NewGuid();
                    this.AddField(f);
                }
            }
        }
        private void AddVirtualFieldsClick(object sender, RoutedEventArgs e)
        {
            VirtualFieldsAppender appender = new();
            if (appender.ShowDialog("Настройка виртуальных полей", Core.Classes.Icon.Magic))
            {
                foreach (Field f in appender.GetFields())
                {
                    this.AddField(f);
                }
            }
        }
        private List<Field> GetActualFields()
        {
            List<Field> fields = [];
            //List<string> names = [];
            //foreach (Controls.FieldCreator item in this.ContentPanel.Children)
            //{
            //    Field f = item.GetField();
            //    if (names.Contains(f.Name))
            //    {
            //        throw new FieldDataFailed($"Поле [{f.Name}] встречается более одного раза. Имена полей должны быть уникальными.");
            //    }
            //    else
            //    {
            //        names.Add(f.Name);
            //    }
            //    f.SetId();
            //    fields.Add(f);
            //}
            return fields;
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            DialogsManager.ShowWaitCursor();
            try
            {
                this.partSettings?.Save();
                if (this.vm.Save())
                {
                    DialogsManager.ShowWaitCursor(false);
                    this.Close();
                }
            }
            catch (FieldDataFailed ex)
            {
                DialogsManager.ShowExclamationDialog(ex.Message, "Сохранение прервано");
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }           
        }
        #region Templates
        private void AddTemplateClick(object sender, MouseButtonEventArgs e)
        {
            //TemplateClassEditor ce = new();
            //ce.ShowDialog();
            //if (ce.Result)
            //{
            //    TemplateData td = new()
            //    {
            //        Name = ce.SelectedName,
            //        File = ce.SelectedPath
            //    };
            //    this.vm.SourceData.AddTemplate(td);
            //}
            //this.UpdateTemplatesList();
        }
        private void CopyTemplatesClick(object sender, MouseButtonEventArgs e)
        {
            //ClassSelector cs = new();
            //if (cs.ShowDialog("Выбор класса", Core.Classes.Icon.Search))
            //{
            //    ClassData cd = cs.GetSelectedClassData();
            //    if (cd.Templates == null)
            //    {
            //        return;
            //    }
            //    this.vm.SourceData.Templates = cd.Templates;
            //    //foreach (TemplateData sd in cd.Templates.Values)
            //    //{
            //    //    this.vm.SourceData.AddTemplate(sd);
            //    //}
            //}
            //this.UpdateTemplatesList();
        }
        private void UpdateTemplatesList()
        {
            //this.TemplatesPanel.Children.Clear();
            //if (this.vm.SourceData.Templates is null)
            //{
            //    return;
            //}
            //foreach (KeyValuePair<int, TemplateData> template in this.vm.SourceData.Templates)
            //{
            //    TemplateClassElement tce = new(template.Key, template.Value);
            //    tce.OnEdit += this.Tce_OnEdit;
            //    tce.OnRemove += this.Tce_OnRemove;
            //    tce.OnSearchInFileRequested += this.Tce_OnSearchInFileRequested;
            //    this.TemplatesPanel.Children.Add(tce);
            //}
        }

        private void Tce_OnSearchInFileRequested(string path)
        {
            bool CheckNameUniqueness(string name)
            {
                //foreach (Objects.Views.Controls.FieldCreator creator in this.ContentPanel.Children)
                //{
                //    if (creator.vm.Source.Name == name)
                //    {
                //        return false;
                //    }
                //}
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
                    Field tag = new()
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

        private void Tce_OnRemove(int index, Template data)
        {
            //this.vm.SourceData.Templates.Remove(index);
            //this.UpdateTemplatesList();
        }

        private void Tce_OnEdit(int index, Template data)
        {
            //this.vm.SourceData.EditTemplate(index, data);
            //this.UpdateTemplatesList();
        }
        #endregion

        #region Statuses
        private void AddStatusClick(object sender, MouseButtonEventArgs e)
        {
            //StatusSettings ss = new();
            //if (ss.ShowDialog("Настройка статуса", Core.Classes.Icon.Tag) == true)
            //{
            //    this.vm.SourceData.AddStatus(ss.GetData());
            //    this.UpdateStatusesList();
            //}
        }
        private void CopyStatusesClick(object sender, MouseButtonEventArgs e)
        {
            ClassSelector cs = new();
            if (cs.ShowDialog("Выбор класса", Core.Classes.Icon.Search))
            {
                IClassData cd = cs.GetSelectedClassData();
                //if (cd.Statuses == null)
                //{
                //    return;
                //}
                //this.vm.SourceData.Statuses = cd.Statuses;
                //foreach (StatusData sd in cd.Statuses.Values)
                //{
                //    this.vm.SourceData.AddStatus(sd);
                //}
            }
            this.UpdateStatusesList();
        }
        private void UpdateStatusesList()
        {
            //this.StatusesPanel.Children.Clear();
            //if (this.vm.SourceData.Statuses is null)
            //{
            //    return;
            //}
            //int index = 0;
            //foreach (StatusData data in this.vm.SourceData.Statuses.Values)
            //{
            //    index++;
            //    StatusElement se = new(index, data);
            //    se.OnEdit += this.Se_OnEdit;
            //    se.OnRemove += this.Se_OnRemove;
            //    this.StatusesPanel.Children.Add(se);
            //}
        }

        private void Se_OnRemove(int index, StatusData statusData)
        {
            //this.vm.SourceData.RemoveStatus(index);
            //this.UpdateStatusesList();
        }

        private void Se_OnEdit(int index, StatusData statusData)
        {
            //this.vm.SourceData.Statuses[index] = statusData;
            //this.UpdateStatusesList();
        }

        #endregion

        private void MinimizeAllClick(object sender, RoutedEventArgs e)
        {
            foreach (FieldViewModel f in this.vm.Fields)
            {
                f.IsExpanded = false;
            }
        }

        private void MaximizeAllClick(object sender, RoutedEventArgs e)
        {
            foreach (FieldViewModel f in this.vm.Fields)
            {
                f.IsExpanded = true;
            }
        }

        private void ShowFormClick(object sender, RoutedEventArgs e)
        {
            List<Field> fields = [];
            try
            {
                //foreach (Incas.Objects.Views.Controls.FieldCreator item in this.ContentPanel.Children)
                //{
                //    Field f = item.GetField();
                //    f.SetId();
                //    fields.Add(f);
                //}
                //this.vm.SourceData.Fields = fields;
                //ObjectsEditor oe = new(this.vm.Source, this.vm.SourceData);
                //oe.ShowDialog();
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Предпросмотр прерван");
            }
        }

        private void InsertFieldNameClick(object sender, RoutedEventArgs e)
        {
            //List<Field> fields = new();
            //foreach (Controls.FieldCreator item in this.ContentPanel.Children)
            //{
            //    fields.Add(item.vm.Source);
            //}
            //FieldNameInsertor fn = new(fields);
            //if (fn.ShowDialog("Вставка имени", Core.Classes.Icon.Tags))
            //{
            //    this.vm.NameTemplate = this.NameTemplate.Text + " " + fn.GetSelectedField();
            //}
        }

        private void GenerateBaseScriptClick(object sender, RoutedEventArgs e)
        {
            string result = $"class {this.vm.Source.Name.Replace(' ', '_')}:\n\tdef __init__(self, ";
            List<string> fieldsNames = new();
            string imports = "";
            string fieldsAllocation = "";
            List<Field> fields = new();
            try
            {
                fields = this.GetActualFields();
                foreach (Field field in fields)
                {                 
                    switch (field.Type)
                    {
                        case FieldType.Integer:
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

        private void SetPresettingFieldsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ClassFieldsPresetting cfp = new(this.GetActualFields(), this.vm.SourceData.RestrictFullView);
                if (cfp.ShowDialog("Настройка пресетов", Core.Classes.Icon.Puzzle))
                {
                    this.vm.SourceData.RestrictFullView = cfp.Constraint;
                }
            }
            catch (FieldDataFailed fd)
            {
                DialogsManager.ShowExclamationDialog(fd.Message, "Действие прервано");
                return;
            }
        }

        #region Fields Logics

        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private void UpClick(object sender, RoutedEventArgs e)
        {

        }

        private void DownClick(object sender, RoutedEventArgs e)
        {

        }

        private void ComboType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.vm.UpdateFieldCollections();
        }

        private void AddMethod(object sender, RoutedEventArgs e)
        {
            this.vm.Methods.Add(new(new()));
        }

        private void AddControlToCustomFormClick(object sender, RoutedEventArgs e)
        {
            this.vm.AddNewControlToCustomForm(
                new() { 
                    Name = $"Контейнер {this.vm.ViewControls?.Count+1}", 
                    Children = [] 
                }
                );
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.vm.SelectedViewControl = (ViewControlViewModel)e.NewValue;
        }

        private void AddChildContainerToViewControl(object sender, RoutedEventArgs e)
        {
            this.vm.SelectedViewControl.AddChild(new(new() { Name = $"Контейнер {this.vm.SelectedViewControl.Children.Count + 1}", Children = [] }));
        }
        private void Vm_OnDrawCalling()
        {
            if (this.vm.Validate())
            {
                FormDrawingManager.DrawForm(Helpers.CreateObjectByType(this.vm.Source), this.FormPreviewPanel);           
            }           
        }

        private void RemoveSelectedElementFromForm(object sender, RoutedEventArgs e)
        {
            this.vm.SelectedViewControl?.RemoveFromParent();
        }
    }
}
