using Incas.Core.Classes;
using Incas.Core.Views.Windows;
using Incas.Objects.AutoUI;
using Incas.Objects.Documents.ViewModels;
using Incas.Objects.Documents.Views.Windows;
using Incas.Objects.Interfaces;
using Incas.Objects.ViewModels;
using Incas.Rendering.Components;
using IncasEngine.ObjectiveEngine.Models;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Incas.Objects.Documents.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для DocumentClassPart.xaml
    /// </summary>
    public partial class DocumentClassPart : System.Windows.Controls.UserControl, IClassPartSettings
    {
        public string ItemName => "Настройка шаблонов";
        public DocumentClassPartViewModel vm { get; set; }
        public DocumentClassPart()
        {
            this.InitializeComponent();
        }

        public event IClassPartSettings.OpenAdditionalSettings OnAdditionalSettingsOpenRequested;

        public IClassPartSettings SetUp(IMembersContainerViewModel classViewModel)
        {

            this.vm = new(classViewModel as ClassViewModel);
            this.DataContext = this.vm;
            return this;
        }

        public void Validate()
        {
            this.vm.Validate();
        }

        public void Save()
        {
            this.vm.Save();
        }
        private void AddTemplateClick(object sender, System.Windows.RoutedEventArgs e)
        {           
            this.vm.SelectedTemplate = this.vm.AddTemplate();
            this.vm.UpdateAll();
        }

        private void AddTemplatePropertyClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.vm.SelectedTemplate.AddProperty(new());
        }

        private void FindTagsInFileClick(object sender, System.Windows.RoutedEventArgs e)
        {
            bool CheckNameExistsInFields(string tagname)
            {
                foreach (FieldViewModel f in this.vm.BaseViewModel.Fields)
                {
                    if (f.Name == tagname)
                    {
                        return false;
                    }
                }
                return true;
            }
            bool CheckNameExistsInProperties(string tagname)
            {
                foreach (PropertyViewModel pvm in this.vm.SelectedTemplate.Properties)
                {
                    if (pvm.PropertyName == tagname)
                    {
                        return false;
                    }
                }
                return true;
            }
            try
            {
                FileTemplator templator = new(ProgramState.CurrentWorkspace.GetFullnameOfDocumentFile(this.vm.SelectedTemplate.FilePath));
                List<string> itemsFound = templator.FindAllTags();
                TemplateFileTagsImporter importer = new(itemsFound);
                if (importer.ShowDialog() == true)
                {
                    foreach (FoundTemplateTagViewModel tag in importer.vm.FoundItems)
                    {
                        switch (tag.Target)
                        {
                            case Components.TagTarget.Property:
                                if (!CheckNameExistsInProperties(tag.Name))
                                {
                                    continue;
                                }
                                TemplateProperty prop = new()
                                {
                                    Name = tag.Name,
                                };
                                this.vm.SelectedTemplate.AddProperty(prop);
                                break;
                            case Components.TagTarget.Field:
                                if (!CheckNameExistsInFields(tag.Name))
                                {
                                    continue;
                                }
                                Field f = new()
                                {
                                    Name = tag.Name,
                                    VisibleName = ClassDataBase.HandleVisibleName(tag.Name)
                                };
                                f.SetId();
                                this.vm.BaseViewModel.AddField(f);
                                break;
                            case Components.TagTarget.Useless:
                                break;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                DialogsManager.ShowErrorDialog(ex);
            }
        }

        private void MinimizeAllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (PropertyViewModel pvm in this.vm.SelectedTemplate?.Properties)
            {
                pvm.IsExpanded = false;
            }
        }

        private void MaximizeAllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (PropertyViewModel pvm in this.vm.SelectedTemplate?.Properties)
            {
                pvm.IsExpanded = true;
            }
        }

        private void GetMoreInfoClick(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void DefineTemplatePathClick(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog fd = new()
            {
                Filter = "Word и Excel|*.docx;*.xlsx",
                InitialDirectory = ProgramState.CurrentWorkspace.GetSourcesTemplatesFolder(),
            };
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ApplySource(fd.FileName);
            }
        }
        private void ApplySource(string path)
        {
            string result = System.IO.Path.GetFileName(path);
            this.vm.SelectedTemplate.FilePath = result;


            if (!ProgramState.CurrentWorkspace.IsTemplatePathContainsWorkspacePath(path)) // если файл еще не в каталоге рабочего пространства
            {
                if (ProgramState.CurrentWorkspace.HasTemplateWithSuchName(result)) // если в каталоге рабочего пространства есть файл с таким же именем
                {
                    if (DialogsManager.ShowQuestionDialog($"Файл с именем \"{result}\" уже существует в рабочем пространстве. Вы хотите выбрать присвоить выбранному файлу другое имя или использовать уже существующий файл?", "Файл уже существует", "Переименовать выбранный", "Использовать существующий") == DialogStatus.Yes)
                    {
                        RenameSourceFile rs = new(result);
                        if (rs.ShowDialog("Новое имя файла", Core.Classes.Icon.Subscript))
                        {
                            this.vm.SelectedTemplate.FilePath = rs.Name;
                            File.Copy(path, ProgramState.CurrentWorkspace.GetFullnameOfDocumentFile(rs.Name));
                        }
                    }
                }
                else
                {
                    File.Copy(path, ProgramState.CurrentWorkspace.GetFullnameOfDocumentFile(result));
                }
            }
        }

        private void ShowTemplateDebuggerClick(object sender, System.Windows.RoutedEventArgs e)
        {
            TemplateDebug debugger = new(this.vm.SelectedTemplate.Source);
            debugger.ShowDialog();
        }

        private void OpenTemplateFiel(object sender, System.Windows.RoutedEventArgs e)
        {
            string pathFile = ProgramState.CurrentWorkspace.GetFullnameOfDocumentFile(this.vm.SelectedTemplate.FilePath);

            if (!File.Exists(pathFile))
            {
                DialogsManager.ShowExclamationDialog($"Файл ({this.vm.SelectedTemplate.FilePath}) не существует!", "Действие прервано");
                return;
            }
            try
            {
                System.Diagnostics.Process proc = new();
                proc.StartInfo.FileName = pathFile;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
            catch (Exception ex)
            {
                DialogsManager.ShowErrorDialog($"При попытке открытия файла возникла ошибка:\n{ex}");
            }
        }

        private void RemoveSelectedTemplate(object sender, System.Windows.RoutedEventArgs e)
        {
            this.vm.Templates.Remove(this.vm.SelectedTemplate);
        }

        private void OpenScriptClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            DocumentTemplateScriptSettings scriptSettings = new(this.vm.SelectedTemplate);
            scriptSettings.SetUpContext(this.vm.BaseViewModel);
            this.OnAdditionalSettingsOpenRequested?.Invoke(scriptSettings);
        }
    }
}
