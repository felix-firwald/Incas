using Incas.Core.Classes;
using Incas.DialogSimpleForm.Components;
using IncasEngine.AdditionalFunctionality;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Windows.Storage;

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
        public StaticAutoUIBase Result;

        /// <summary>
        /// Target object
        /// </summary>
        public DynamicAutoUIForm DynamicResult;

        public enum DSFMode
        {
            Static,
            Dynamic
        }
        /// <summary>
        /// Mode is how to get-set data, without changes for drawing system
        /// </summary>
        private DSFMode Mode;
        /// <summary>
        /// Base InitializeComponent method for constructors
        /// </summary>
        /// <param name="values"></param>
        /// <param name="title"></param>
        private void Initialize(StaticAutoUIBase values, string title)
        {
            this.InitializeComponent();
            this.Mode = DSFMode.Static;
            this.SimpleForm = new(values, this.Fields);
            this.Result = values;
            this.FinishText.Content = values.GetFinishButtonText();
            this.No.Content = values.GetCancelButtonText();
            this.Title = title;
            DialogsManager.ShowWaitCursor(false);
        }
        private void Initialize(DynamicAutoUIForm form)
        {
            this.InitializeComponent();
            this.Mode = DSFMode.Dynamic;
            this.SimpleForm = new(form, this.Fields);
            this.DynamicResult = form;
            this.FinishText.Content = form.FinishButtonText;
            this.No.Content = form.CancelButtonText;
            this.Title = form.Name;
            DialogsManager.ShowWaitCursor(false);
        }
        /// <summary>
        /// Initialize Window with the default icon and color
        /// </summary>
        /// <param name="values"></param>
        /// <param name="title"></param>
        public DialogSimpleForm(StaticAutoUIBase values, string title)
        {
            this.Initialize(values, title);
        }
        /// <summary>
        /// Initialize Window with the default color of icon
        /// </summary>
        /// <param name="values"></param>
        /// <param name="title"></param>
        /// <param name="pathIcon"></param>
        public DialogSimpleForm(StaticAutoUIBase values, string title, Icon pathIcon)
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
        public DialogSimpleForm(StaticAutoUIBase values, string title, Icon pathIcon, IconColor color)
        {
            this.Initialize(values, title);
            this.PathIcon.Data = Geometry.Parse(IconsManager.GetIconByName(pathIcon));
            this.PathIcon.Fill = ColorManager.GetColor(color);
        }
        public DialogSimpleForm(DynamicAutoUIForm form)
        {
            this.Initialize(form);
            this.PathIcon.Fill = ColorManager.GetColor(IconColor.Yellow);
            this.PathIcon.Data = Geometry.Parse(IconsManager.GetIconByName(Core.Classes.Icon.Lightning));
        }

        private void FinishClick(object sender, RoutedEventArgs e)
        {
            switch (this.Mode)
            {
                case DSFMode.Static:
                    if (this.SimpleForm.Save())
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                    break;
                case DSFMode.Dynamic:
                    if (this.SimpleForm.SaveDynamic())
                    {
                        this.DialogResult = true;
                        this.Close();
                    }
                    break;
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
