using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Incas.DialogSimpleForm.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для DialogSimpleForm.xaml
    /// </summary>
    public partial class DialogSimpleForm : Window
    {
        /// <summary>
        /// SimpleFormGenerator for loading and saving data
        /// </summary>
        private SimpleFormGenerator SimpleForm;
        /// <summary>
        /// Target object
        /// </summary>
        public AutoUIBase Result;
        /// <summary>
        /// Base InitializeComponent method for constructors
        /// </summary>
        /// <param name="values"></param>
        /// <param name="title"></param>
        private void Initialize(AutoUIBase values, string title)
        {
            this.InitializeComponent();
            this.SimpleForm = new(values, this.Fields);
            this.Result = values;
            this.FinishText.Content = values.GetFinishButtonText();
            this.No.Content = values.GetCancelButtonText();
            this.TitleText.Content = title;
            this.Title = title;
        }
        /// <summary>
        /// Initialize Window with the default icon and color
        /// </summary>
        /// <param name="values"></param>
        /// <param name="title"></param>
        public DialogSimpleForm(AutoUIBase values, string title)
        {
            this.Initialize(values, title);
        }
        /// <summary>
        /// Initialize Window with the default color of icon
        /// </summary>
        /// <param name="values"></param>
        /// <param name="title"></param>
        /// <param name="pathIcon"></param>
        public DialogSimpleForm(AutoUIBase values, string title, Icon pathIcon)
        {
            this.Initialize(values, title);
            this.PathIcon.Data = Geometry.Parse(IconsManager.GetIconByName(pathIcon));
        }
        /// <summary>
        /// Initialize Window with full customizing
        /// </summary>
        /// <param name="values"></param>
        /// <param name="title"></param>
        /// <param name="pathIcon"></param>
        /// <param name="color"></param>
        public DialogSimpleForm(AutoUIBase values, string title, Icon pathIcon, IconColor color)
        {
            this.Initialize(values, title);
            this.PathIcon.Data = Geometry.Parse(IconsManager.GetIconByName(pathIcon));
            this.PathIcon.Fill = ColorManager.GetColor(color);
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
