using Incas.Objects.Interfaces;
using IncasEngine.ObjectiveEngine;
using IncasEngine.ObjectiveEngine.Interfaces;
using IncasEngine.ObjectiveEngine.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для ObjectBackReferenceViewer.xaml
    /// </summary>
    public partial class ObjectBackReferenceViewer : UserControl, IObjectFieldViewer
    {
        private bool visible = false;
        public IClass Class { get; set; } // our class
        public Guid TargetObject { get; set; } // our object
        public ObjectBackReferenceViewer(IClass cl, Guid targetObject)
        {
            this.InitializeComponent();
            this.Class = cl;
            this.TargetObject = targetObject;
        }

        private void FieldValue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.visible)
            {
                this.visible = false;
                this.ContentPanel.Children.Clear();
                this.FieldValue.Visibility = Visibility.Visible;
                this.FieldValue2.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.visible = true;
                this.FieldValue2.Visibility = Visibility.Visible;
                this.FieldValue.Visibility = Visibility.Collapsed;
                using Class cl = new();
                List<Class> list = cl.FindBackReferences(this.Class.Id);
                foreach (Class c in list)
                {
                    Expander exp = new()
                    {
                        Header = c.Name,
                        Style = this.FindResource("ExpanderMain") as Style,
                        Uid = c.Id.ToString()
                    };
                    exp.Expanded += this.Exp_Expanded;
                    this.ContentPanel.Children.Add(exp);
                }
            }

        }

        private void Exp_Expanded(object sender, RoutedEventArgs e)
        {
            //Expander exp = (Expander)sender;
            //Guid id = Guid.Parse(exp.Uid); // back class
            //Class back = new(id);
            //StackPanel panel = new()
            //{
            //    Margin = new(0, 5, 0, 5)
            //};
            //DataTable dt = Processor.GetSimpleObjectsListWhereEqual(
            //    back,
            //    null,
            //    back.GetClassData().FindFieldByBackReference(this.Class.Id).VisibleName, // не находит
            //    this.TargetObject.ToString());
            //foreach (DataRow dr in dt.Rows)
            //{
            //    string elId = dr[Helpers.IdField].ToString();
            //    string elName = dr[Helpers.NameField].ToString();
            //    panel.Children.Add(new ObjectElement(back, elId, elName));
            //}
            //exp.Content = panel;
        }

        public void HideSeparator()
        {
            this.Separator.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
