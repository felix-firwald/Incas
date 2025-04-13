using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using Incas.Rendering.Components;
using IncasEngine.Core;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateClass.xaml
    /// </summary>
    public partial class CreateClass : Window
    {
        public ClassViewModel vm;
        private IClassPartSettings partSettings;
        public CreateClass(Class primary) // init class
        {
            DialogsManager.ShowWaitCursor();
            this.InitializeComponent();
            this.vm = new(primary);
            this.vm.OnAdditionalSettingsOpenRequested += this.Vm_OnAdditionalSettingsOpenRequested;
            this.vm.OnDrawCalling += this.Vm_OnDrawCalling;
            if (this.vm.Type == ClassType.Document)
            {
                this.vm.ShowCard = true;
            }
            this.DataContext = this.vm;
            this.ApplyPartSettings();
            DialogsManager.ShowWaitCursor(false);
        }

        private void Vm_OnAdditionalSettingsOpenRequested(IClassDetailsSettings settings)
        {
            foreach (TabItem tabitem in this.TabControlMain.Items)
            {
                if (tabitem.Header.ToString() == settings.ItemName)
                {
                    tabitem.IsSelected = true;
                    return;
                }
            }
            TabItem item = new()
            {
                Content = settings,
                Header = settings.ItemName,
                Style = this.FindResource("TabItemRemovable") as Style,
                BorderBrush = this.FindResource("LightPurple") as Brush
            };
            item.IsSelected = true;
            item.IsEnabledChanged += this.Item_IsEnabledChanged1;
            this.TabControlMain.Items.Add(item);
        }

        private void Item_IsEnabledChanged1(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.TabControlMain.Items.Remove(sender);
        }

        public CreateClass(Guid id) // edit class
        {
            DialogsManager.ShowWaitCursor();      
            this.InitializeComponent();
            if (id == Guid.Empty)
            {
                this.Title = "(класс не выбран)";
                this.IsEnabled = false;
                return;
            }
            this.Title = "Редактирование класса";
            Class cl = EngineGlobals.GetClass(id);
            this.vm = new(cl);
            this.vm.OnAdditionalSettingsOpenRequested += this.Vm_OnAdditionalSettingsOpenRequested;
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
            //this.CodeModule.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
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
        private void ShowNewTab(Control control, string name)
        {
            TabItem item = new()
            {
                Content = control,
                Header = name,
                Style = this.FindResource("TabItemRemovable") as Style,
                BorderBrush = this.FindResource("LightPurple") as Brush
            };
            item.IsEnabledChanged += this.Item_IsEnabledChanged;
            this.TabControlMain.Items.Add(item);
        }

        private void Item_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.TabControlMain.Items.Remove(sender);
        }

        private void GetMoreInfoClick(object sender, RoutedEventArgs e)
        {
            //ProgramState.OpenWebPage("https://teletype.in/@incas/classes");
            DialogsManager.ShowHelp(Help.Components.HelpType.Classes_General);
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
                    this.vm.AddField(f);
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
                    this.vm.AddField(tag);
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
            this.vm.AddMethod(new());
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
            ControlType type = ControlType.VerticalStack;
            switch (this.vm.SelectedViewControl.Type)
            {
                case ControlType.VerticalStack:
                    type = ControlType.HorizontalStack;
                    break;
                case ControlType.HorizontalStack:
                    type = ControlType.VerticalStack;
                    break;
                case ControlType.Tab:
                    type = ControlType.TabItem;
                    break;
                case ControlType.TabItem:
                case ControlType.Group:
                    this.vm.SelectedViewControl.Children.Clear();
                    type = ControlType.VerticalStack;
                    break;
            }
            this.vm.SelectedViewControl.AddChild(new(new() { Name = $"Контейнер {this.vm.SelectedViewControl.Children.Count + 1}", Type = type, Children = [] }));
        }
        private void Vm_OnDrawCalling()
        {
            FormDrawingManager.Start().DrawDebugForm(this.vm, this.FormPreviewPanel);                     
        }

        private void RemoveSelectedElementFromForm(object sender, RoutedEventArgs e)
        {
            this.vm.SelectedViewControl?.RemoveFromParent();
        }

        private void MoveUpElementInForm(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedViewControl is null)
            {
                return;
            }
            ViewControlViewModel selected = this.vm.SelectedViewControl;
            this.vm.SelectedViewControl.MoveUp();
        }

        private void MoveDownElementInForm(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedViewControl is null)
            {
                return;
            }
            ViewControlViewModel selected = this.vm.SelectedViewControl;
            this.vm.SelectedViewControl.MoveDown();
        }

        private void UpdateFormPreview(object sender, RoutedEventArgs e)
        {
            this.Vm_OnDrawCalling();
            this.ZoomPanel.FitToBounds();
        }

        private void AddFieldClick(object sender, RoutedEventArgs e)
        {
            Field f = new();
            f.SetId();
            f.Name = $"Поле_{this.vm.Fields.Count + 1}";            
            this.vm.AddField(f);
        }

        private void AddMethodClick(object sender, RoutedEventArgs e)
        {
            Method m = new();
            m.SetId();
            m.Name = $"Метод_{this.vm.Methods.Count + 1}";
            this.vm.AddMethod(m);
        }

        private void AddTableClick(object sender, RoutedEventArgs e)
        {
            Table t = new();
            t.SetId();
            t.Name = $"Таблица_{this.vm.Tables.Count + 1}";
            this.vm.AddTable(t);
        }

        private void AddStateClick(object sender, RoutedEventArgs e)
        {
            State s = new();
            s.SetId();
            s.Name = $"Состояние_{this.vm.States.Count + 1}";
            this.vm.AddState(s);
        }
    }
}
