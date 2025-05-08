using DIaLOGIKa.b2xtranslator.OfficeGraph;
using Incas.Admin.Components.ExternalFunctionality;
using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using Incas.Rendering.Components;
using IncasEngine.Core;
using IncasEngine.Models;
using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.Events.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.Processes.ClassComponents;
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
        private const string part_settings_tag = "PART_SETTINGS";
        private bool _isClosingProgrammatically = false;
        public ClassViewModel vm;
        private IClassPartSettings partSettings;

        public CreateClass(Class primary) // init class
        {
            DialogsManager.ShowWaitCursor();
            this.InitializeComponent();
            this.ApplyDraftButton.IsEnabled = false;
            this.SaveDraftButton.IsEnabled = false;
            this.vm = new(primary);
            if (primary.Generalizators is not null && primary.Generalizators.Count > 0)
            {
                using (Generalizator g = new())
                {
                    this.ApplyGeneralizators(g.GetGeneralizators(primary.Generalizators));
                }
            }
            this.ApplyViewModelEvents();
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
            this.ApplyViewModelEvents();
            this.DataContext = this.vm;
            this.ApplyPartSettings();
            DialogsManager.ShowWaitCursor(false);
        }

        public CreateClass(ServiceClass @class) // edit service class
        {
            DialogsManager.ShowWaitCursor();
            XmlReader reader = XmlReader.Create("Static\\Coding\\IncasPython.xshd");
            this.InitializeComponent();
            //this.CodeModule.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            this.Title = $"Служебный класс: {@class.Name}";
            this.vm = new(@class);
            this.ApplyViewModelEvents();
            this.DataContext = this.vm;
            this.ApplyPartSettings();
            DialogsManager.ShowWaitCursor(false);
        }
        private void ApplyGeneralizators(List<Generalizator> gens)
        {
            foreach (Generalizator g in gens)
            {
                foreach (Field f in g.Data.Fields)
                {
                    this.vm.AddField(f);
                }
                foreach (Method f in g.Data.Methods)
                {
                    this.vm.AddMethod(f);
                }
                foreach (Method f in g.Data.StaticMethods)
                {
                    this.vm.AddStaticMethod(f);
                }
                foreach (Table f in g.Data.Tables)
                {
                    this.vm.AddTable(f);
                }
            }
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
                Tag = part_settings_tag,
                Style = this.FindResource("TabItemRemovable") as Style,
                BorderBrush = this.FindResource("LightPurple") as Brush
            };
            item.IsSelected = true;
            item.IsEnabledChanged += this.Item_IsEnabledChanged1;
            this.TabControlMain.Items.Add(item);
        }

        private void ApplyViewModelEvents()
        {
            this.vm.OnAdditionalSettingsOpenRequested += this.Vm_OnAdditionalSettingsOpenRequested;
            this.vm.OnDrawCalling += this.Vm_OnDrawCalling;
        }

        private void Item_IsEnabledChanged1(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.TabControlMain.Items.Remove(sender);
        }

        private void ApplyPartSettings()
        {
            List<TabItem> itemsToRemove = new();
            foreach (TabItem ti in this.TabControlMain.Items)
            {
                if (ti.Tag?.ToString() == part_settings_tag)
                {
                    itemsToRemove.Add(ti);
                }
            }
            foreach (TabItem item in itemsToRemove)
            {
                this.TabControlMain.Items.Remove(item);
            }
            this.partSettings = ServiceExtensionFieldsManager.GetPartSettingsByType(this.vm);
            if (this.partSettings is not null)
            {
                TabItem item = new()
                {
                    Content = this.partSettings,
                    Header = this.partSettings.ItemName,
                    Tag = part_settings_tag,
                    BorderBrush = this.FindResource("LightPurple") as Brush
                };
                this.TabControlMain.Items.Add(item);
                this.partSettings.OnAdditionalSettingsOpenRequested += this.Vm_OnAdditionalSettingsOpenRequested;
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
        private void LockUI()
        {
            this.vm.IsEditingEnabled = false;
        }
        private void UnlockUI()
        {
            this.vm.IsEditingEnabled = true;       
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
            if (this.vm.SelectedViewControl is null)
            {
                return;
            }
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
            f.ListVisibility = true;
            this.vm.AddField(f);
        }

        private void AddMethodClick(object sender, RoutedEventArgs e)
        {
            Method m = new();
            m.SetId();
            m.Icon = "M340-302.23v-355.54q0-15.84 10.87-26 10.87-10.15 25.37-10.15 4.53 0 9.5 1.31 4.97 1.3 9.49 3.92l279.84 178.15q8.24 5.62 12.35 13.46 4.12 7.85 4.12 17.08 0 9.23-4.12 17.08-4.11 7.84-12.35 13.46L395.23-271.31q-4.53 2.62-9.52 3.92-4.98 1.31-9.51 1.31-14.51 0-25.35-10.15-10.85-10.16-10.85-26Z";
            m.Color = new() { R = 255, G = 255, B = 255 };
            this.vm.AddMethod(m);
        }
        private void AddStaticMethodClick(object sender, RoutedEventArgs e)
        {
            Method m = new();
            m.SetId();
            m.Icon = "M340-302.23v-355.54q0-15.84 10.87-26 10.87-10.15 25.37-10.15 4.53 0 9.5 1.31 4.97 1.3 9.49 3.92l279.84 178.15q8.24 5.62 12.35 13.46 4.12 7.85 4.12 17.08 0 9.23-4.12 17.08-4.11 7.84-12.35 13.46L395.23-271.31q-4.53 2.62-9.52 3.92-4.98 1.31-9.51 1.31-14.51 0-25.35-10.15-10.85-10.16-10.85-26Z";
            m.Color = new() { R = 255, G = 255, B = 255 };
            this.vm.AddStaticMethod(m);
        }

        private void AddTableClick(object sender, RoutedEventArgs e)
        {
            Table t = new();
            t.SetId();
            this.vm.AddTable(t);
        }

        private void AddStateClick(object sender, RoutedEventArgs e)
        {
            State s = new();
            s.SetId();
            s.Name = $"Состояние_{this.vm.States.Count + 1}";
            this.vm.AddState(s);
        }

        private void RemoveSelectedStateClick(object sender, RoutedEventArgs e)
        {
            this.vm.RemoveSelectedState();
        }

        private void AddFieldsFromStringSourceClick(object sender, RoutedEventArgs e)
        {
            ClassFieldsString source = new();
            if (source.ShowDialog("Вставка полей"))
            {
                List<Field> fields = source.GetFields();
                foreach (Field f in fields)
                {
                    this.vm.AddField(f);
                }
            }
        }

        private void WrapViewControlClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedViewControl is null)
            {
                return;
            }
            ViewControlViewModel child = this.vm.SelectedViewControl;
            ViewControlViewModel parent = this.vm.FindParent(child);
            if (parent is null)
            {
                return;
            }
            ViewControlViewModel newWrapper = new(new() { Children = [], Name = $"Обертка над {child.Name}", Type = ControlType.Group });
            parent.Children.Remove(child);
            newWrapper.AddChild(child);
            parent.AddChild(newWrapper);
            this.Vm_OnDrawCalling();
            this.ZoomPanel.FitToBounds();
        }

        private void AddTextLabelToFormClick(object sender, RoutedEventArgs e)
        {
            if (this.vm.SelectedViewControl is null)
            {
                return;
            }
            ViewControlViewModel result = new(new() { Type = ControlType.Text, Name = "Текст" });
            this.vm.SelectedViewControl.AddChild(result);
        }
        private void CloseProgrammatically()
        {
            this._isClosingProgrammatically = true;
            this.Close();
            _isClosingProgrammatically = false;
        }
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            DialogsManager.ShowWaitCursor();
            this.LockUI();
            try
            {
                this.partSettings?.Validate();
                this.partSettings?.Save();
                this.vm.Save();
                DialogsManager.ShowWaitCursor(false);
                this.CloseProgrammatically();
            }
            catch (FieldDataFailed ex)
            {
                DialogsManager.ShowExclamationDialog(ex.Message, "Сохранение прервано");
                this.UnlockUI();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
                this.UnlockUI();
            }
        }
        private void SaveDraft(object sender, RoutedEventArgs e)
        {
            if (this.vm.Source.Id == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Сохранять черновик станет возможным только после первого сохранения класса.", "Действие невозможно");
                return;
            }
            DialogsManager.ShowWaitCursor();
            this.LockUI();
            try
            {
                this.partSettings?.Save();
                IClassData cd = this.vm.GetData();
                ClassHelpers.ClassDraft draft = new();
                draft.Name = this.vm.Source.Name;
                draft.InternalName = this.vm.Source.InternalName;
                draft.Description = this.vm.Source.Description;
                switch (this.vm.Source.Type)
                {
                    case ClassType.Model:
                    case ClassType.ServiceClassGroup:
                    case ClassType.ServiceClassUser:
                        draft.ClassData = cd as ClassData;
                        break;
                    case ClassType.Document:
                        draft.DocumentClassData = cd as DocumentClassData;
                        break;
                    case ClassType.Process:
                        draft.ProcessClassData = cd as ProcessClassData;
                        break;
                    case ClassType.Event:
                        draft.EventClassData = cd as EventClassData;
                        break;
                }
                ClassHelpers.SaveClassDraft(this.vm.Source, draft);
                DialogsManager.ShowWaitCursor(false);
                this.UnlockUI();
            }
            catch (FieldDataFailed ex)
            {
                DialogsManager.ShowExclamationDialog(ex.Message, "Сохранение прервано");
                this.UnlockUI();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog(ex);
                this.UnlockUI();
            }
        }

        private void LoadDraft(object sender, RoutedEventArgs e)
        {
            try
            {
                ClassHelpers.ClassDraft draft = ClassHelpers.LoadClassDraft(this.vm.Source);
                if (!draft.IsRealRecord)
                {
                    DialogsManager.ShowExclamationDialog("Не удалось найти на этом устройстве сохраненный черновик этого класса, либо же его данные повреждены.", "Действие невозможно");
                    return;
                }
                if (DialogsManager.ShowQuestionDialog($"Вы уверены, что хотите загрузить черновик, созданный {draft.DateSaved}? Обратите внимание, что все текущие изменения (если они были) будут потеряны.", "Загрузить черновик?", "Загрузить", "Не загружать") == Core.Views.Windows.DialogStatus.Yes)
                {
                    this.vm.Source.Name = draft.Name;
                    this.vm.Source.InternalName = draft.InternalName;
                    this.vm.Source.Description = draft.Description;
                    switch (this.vm.Type)
                    {
                        case ClassType.Model:
                        case ClassType.ServiceClassUser:
                        case ClassType.ServiceClassGroup:
                            this.vm.Source.SetClassData(draft.ClassData);
                            break;
                        case ClassType.Document:
                            this.vm.Source.SetClassData(draft.DocumentClassData);
                            break;
                        case ClassType.Process:
                            this.vm.Source.SetClassData(draft.ProcessClassData);
                            break;
                        case ClassType.Event:
                            this.vm.Source.SetClassData(draft.EventClassData);
                            break;
                    }
                    this.vm = ClassViewModel.Init(this.vm.Source);
                    this.ApplyViewModelEvents();
                    this.DataContext = this.vm;
                    this.ApplyPartSettings();
                }                
            }
            catch
            {
                DialogsManager.ShowExclamationDialog("Не удалось найти на этом устройстве сохраненный черновик этого класса, либо же его данные повреждены.", "Действие невозможно");
                return;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Проверяем, закрывается ли окно программно
            if (!_isClosingProgrammatically)
            {
                EngineGlobals.ClearClassFromCache(this.vm.Source);
            }

            base.OnClosing(e);
        }
    }
}
