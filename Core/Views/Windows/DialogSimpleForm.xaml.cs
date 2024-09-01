using Incas.Core.Attributes;
using Incas.Core.AutoUI;
using Incas.Core.Classes;
using Incas.Core.Views.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Incas.Core.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogSimpleForm.xaml
    /// </summary>
    public partial class DialogSimpleForm : Window
    {
        private SimpleFormGenerator SimpleForm;
        public AutoUIBase Result;
        private void Initialize(AutoUIBase values, string title)
        {
            this.InitializeComponent();
            this.SimpleForm = new(values, this.Fields);
            this.Result = values;
            this.TitleText.Content = title;
            this.Title = title;
        }
        public DialogSimpleForm(AutoUIBase values, string title)
        {
            this.Initialize(values, title);
        }
        public DialogSimpleForm(AutoUIBase values, string title, Icon pathIcon)
        {
            this.Initialize(values, title);
            this.PathIcon.Data = Geometry.Parse(IconsManager.GetIconByName(pathIcon));
        }

        private void FinishClick(object sender, RoutedEventArgs e)
        {
            if (this.SimpleForm.Save())
            {
                this.DialogResult = true;
                this.Close();
            }    
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
