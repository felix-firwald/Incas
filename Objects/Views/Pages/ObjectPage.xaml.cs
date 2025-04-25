using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using IncasEngine.Core.Registry;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Exceptions;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using IncasEngine.ObjectiveEngine.Types.ServiceClasses.Groups.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Incas.Objects.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ObjectPage.xaml
    /// </summary>
    public partial class ObjectPage : UserControl
    {
        public readonly IClass Class;
        public readonly IClassData ClassData;
        public ObjectCreator Creator { get; set; }
        public GroupClassPermissionSettings PermissionSettings { get; set; }
        public ObjectPage(IClass source, IObject obj)
        {
            this.InitializeComponent();
            this.Class = source;
            this.ClassData = source.GetClassData();
            this.PermissionSettings = ProgramState.CurrentWorkspace.CurrentGroup.GetClassPermissions(source.Id);
            this.AddObjectCreator(obj);
            DialogsManager.ShowWaitCursor(false);
        }
        public ObjectPage(IClass source)
        {
            this.InitializeComponent();
            this.Class = source;
            this.ClassData = source.GetClassData();
            this.PermissionSettings = ProgramState.CurrentWorkspace.CurrentGroup.GetClassPermissions(source.Id);
            this.AddObjectCreator();
            DialogsManager.ShowWaitCursor(false);
        }

        private ObjectCreator AddObjectCreator(IObject obj = null)
        {
            this.Creator = new(this.Class, obj);
            this.Creator.PermissionSettings = this.PermissionSettings;
            this.Creator.HideNCA();
            if (obj is not null)
            {
                this.Creator.ApplyObject(obj, true);
            }
            this.ContentPanel.Children.Add(this.Creator);        
            
            return this.Creator;
        }

        private async void CreateObjectsClick(object sender, RoutedEventArgs e)
        {
            await Processor.WriteObjects(this.Class, this.Creator.PullObject());
        }

        private async void RenderObjectsClick(object sender, RoutedEventArgs e)
        {
            List<IObject> objects = [];
            IncasEngine.ObjectiveEngine.Types.Documents.Document doc = (this.ContentPanel.Children[0] as ObjectCreator).PullObject() as IncasEngine.ObjectiveEngine.Types.Documents.Document;

            Template templateFile = new();
            DocumentClassData docData = this.ClassData as DocumentClassData;
            if (docData.Documents?.Count == 1)
            {
                templateFile = docData.Documents[0];
            }
            else if (docData.Documents?.Count > 1)
            {
                TemplateSelection ts = new(docData);
                if (ts.ShowDialog("Выбор шаблона", Incas.Core.Classes.Icon.Magic) == true)
                {
                    templateFile = ts.GetSelectedPath();
                }
            }
            else
            {
                DialogsManager.ShowExclamationDialog("Не найдены привязанные шаблоны к этому классу.", "Рендеринг невозможен");
                return;
            }
            string path = RegistryData.GetClassTemplatePrefferedPath(this.Class.Id, templateFile);
            if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
            {
                try
                {
                    RegistryData.SetClassTemplatePrefferedPath(this.Class.Id, templateFile, path);
                    ProgramStatusBar.SetText("Выполняется рендеринг объектов...");
                    foreach (ObjectCreator c in this.ContentPanel.Children)
                    {
                        objects.Add(c.PullObject());
                        await c.GenerateDocument(templateFile, path, false);
                    }
                    ProgramStatusBar.Hide();
                    bool result = await Processor.WriteObjects(this.Class, objects);
                    //if (result)
                    //{
                    //    this.OnUpdateRequested?.Invoke();
                    //}
                    ProgramState.OpenFolder(path);
                }
                catch (FieldDataFailed fdf)
                {
                    DialogsManager.ShowExclamationDialog(fdf.Message, "Рендеринг прерван");
                }
                catch (NotNullFailed nnex)
                {
                    DialogsManager.ShowExclamationDialog(nnex.Message, "Рендеринг прерван");
                }
                catch (AuthorFailed af)
                {
                    DialogsManager.ShowAccessErrorDialog(af.Message);
                }
                catch (Exception ex)
                {
                    DialogsManager.ShowErrorDialog(ex);
                }
            }
        }
    }
}
