using Incas.Objects.Components;
using Incas.Objects.Models;
using Incas.Objects.Views.Pages;
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
        public ObjectsEditor(Class source, List<Components.Object> objects = null)
        {
            this.InitializeComponent();
            this.Class = source;
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
        private void AddObjectCreator(Components.Object obj = null)
        {
            ObjectCreator creator = new(this.Class, obj);
            creator.OnSaveRequested += this.Creator_OnSaveRequested;
            creator.OnRemoveRequested += this.Creator_OnRemoveRequested;
            this.ContentPanel.Children.Add(creator);
        }

        private void Creator_OnRemoveRequested(ObjectCreator creator)
        {
            this.ContentPanel.Children.Remove(creator);
        }

        private void Creator_OnSaveRequested(ObjectCreator creator)
        {
            ObjectProcessor.WriteObjects(this.Class, creator.PullObject());
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

        }

        private void MaximizeAll(object sender, MouseButtonEventArgs e)
        {

        }

        private void CreateObjectsClick(object sender, RoutedEventArgs e)
        {
            List<Components.Object> objects = new();
            foreach (ObjectCreator c in this.ContentPanel.Children)
            {
                objects.Add(c.PullObject());
            }
            ObjectProcessor.WriteObjects(this.Class, objects);
        }
    }
}
