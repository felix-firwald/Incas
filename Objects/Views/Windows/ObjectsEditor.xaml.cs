using Incas.Core.Classes;
using Incas.Objects.AutoUI;
using Incas.Objects.Components;
using Incas.Objects.Exceptions;
using Incas.Objects.Models;
using Incas.Objects.Views.Pages;
using Org.BouncyCastle.Utilities;
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
using System.Windows.Shapes;

namespace Incas.Objects.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ObjectsEditor.xaml
    /// </summary>
    public partial class ObjectsEditor : Window
    {
        public readonly Class Class;
        public readonly ClassData ClassData;
        public delegate void UpdateRequested();
        public event UpdateRequested OnUpdateRequested;
        public ObjectsEditor(Class source, List<Components.Object> objects = null)
        {
            this.InitializeComponent();
            this.Title = source.name;
            this.Class = source;
            this.ClassData = source.GetClassData();
            if (this.ClassData.ClassType == ClassType.Document)
            {
                this.RenderButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.RenderButton.Visibility = Visibility.Collapsed;
            }
            if (objects != null)
            {
                foreach (Components.Object obj in objects)
                {
                    this.AddObjectCreator(obj);
                }
            }
            else
            {
                this.AddObjectCreator();
            }
        }
        public ObjectsEditor(Class source, ClassData data) // dev
        {
            this.InitializeComponent();
            this.Title = "Режим предпросмотра: " + source.name;
            this.Class = source;
            this.ClassData = data;
            this.GenerateButton.IsEnabled = false;
            this.RenderButton.IsEnabled = false;
            this.ToolBar.IsEnabled = false;
            ObjectCreator creator = new(this.ClassData);
            this.ContentPanel.Children.Add(creator);
            if (this.ClassData.ClassType == ClassType.Document)
            {
                this.RenderButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.RenderButton.Visibility = Visibility.Collapsed;
            }
        }

        private void AddObjectCreator(Components.Object obj = null)
        {
            ObjectCreator creator = new(this.Class, this.ClassData, obj);
            creator.OnSaveRequested += this.Creator_OnSaveRequested;
            creator.OnRemoveRequested += this.Creator_OnRemoveRequested;
            creator.OnInsertRequested += this.Creator_OnInsertRequested;
            this.ContentPanel.Children.Add(creator);
        }

        private void Creator_OnInsertRequested(Guid id, string text)
        {
            foreach (ObjectCreator c in this.ContentPanel.Children)
            {
                c.InsertToField(id, text);
            }
        }

        private void Creator_OnRemoveRequested(ObjectCreator creator)
        {
            this.ContentPanel.Children.Remove(creator);
        }

        private void Creator_OnSaveRequested(ObjectCreator creator)
        {
            try
            {
                ObjectProcessor.WriteObjects(this.Class, creator.PullObject());
                this.OnUpdateRequested?.Invoke();
            }
            catch (NotNullFailed nnex)
            {
                DialogsManager.ShowExclamationDialog(nnex.Message, "Сохранение прервано");
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

        private void AddClick(object sender, MouseButtonEventArgs e)
        {
            this.AddObjectCreator();
        }

        private void GetFromExcel(object sender, MouseButtonEventArgs e)
        {

        }

        private void SendToExcel(object sender, MouseButtonEventArgs e)
        {

        }

        private void MinimizeAll(object sender, MouseButtonEventArgs e)
        {
            foreach (ObjectCreator c in this.ContentPanel.Children)
            {
                c.Minimize();
            }
        }

        private void MaximizeAll(object sender, MouseButtonEventArgs e)
        {
            foreach (ObjectCreator c in this.ContentPanel.Children)
            {
                c.Maximize();
            }
        }

        private void CreateObjectsClick(object sender, RoutedEventArgs e)
        {
            List<Components.Object> objects = new();
            try
            {
                foreach (ObjectCreator c in this.ContentPanel.Children)
                {
                    objects.Add(c.PullObject());
                }
                ObjectProcessor.WriteObjects(this.Class, objects);
                this.Close();
                this.OnUpdateRequested?.Invoke();
            }            
            catch (NotNullFailed nnex)
            {
                DialogsManager.ShowExclamationDialog(nnex.Message, "Сохранение прервано");
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

        private void RenderObjectsClick(object sender, RoutedEventArgs e)
        {
            List<Components.Object> objects = new();
            string templateFile = "";
            if (this.ClassData.Templates?.Count == 1)
            {
                templateFile = this.ClassData.Templates[1].File;
            }
            else if (this.ClassData.Templates?.Count > 1)
            {
                TemplateSelection ts = new(this.ClassData);
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
            string path = "";
            if (DialogsManager.ShowFolderBrowserDialog(ref path) == true)
            {
                try
                {
                    foreach (ObjectCreator c in this.ContentPanel.Children)
                    {
                        objects.Add(c.PullObject());
                        c.GenerateDocument(templateFile, path);
                    }
                    ObjectProcessor.WriteObjects(this.Class, objects);
                    this.OnUpdateRequested?.Invoke();
                    ProgramState.OpenFolder(path);
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
