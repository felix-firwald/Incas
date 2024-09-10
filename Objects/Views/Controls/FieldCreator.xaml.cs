using Incas.Objects.Models;
using Incas.Objects.ViewModels;
using Incubator_2.Forms;
using System.Windows;
using System.Windows.Controls;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для FieldCreator.xaml
    /// </summary>
    public partial class FieldCreator : UserControl
    {
        public delegate void FieldAction(FieldCreator t);
        public event FieldAction onDelete;
        public FieldViewModel vm;
        public FieldCreator(Field data = null)
        {
            this.InitializeComponent();
            if (data == null)
            {
                this.vm = new();
            }
            else
            {
                this.vm = new(data);
            }           
            this.DataContext = this.vm;
        }
        public FieldCreator()
        {
            this.InitializeComponent();
        }

        private void MaximizeClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MainBorder.Height = this.ContentPanel.Height + 40;
            this.NumberUp.Visibility = Visibility.Collapsed;
        }

        private void MinimizeClick(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MainBorder.Height = 40;
            this.NumberUp.Visibility = Visibility.Visible;
        }

        private void RemoveClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.onDelete?.Invoke(this);
        }

        private void UpClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void DownClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void DefineRelationClick(object sender, RoutedEventArgs e)
        {

        }

        private void DefineGeneratorClick(object sender, RoutedEventArgs e)
        {

        }

        private void DefineTableClick(object sender, RoutedEventArgs e)
        {

        }

        private void EditScriptClick(object sender, RoutedEventArgs e)
        {

        }

        private void CopyNameToVisible(object sender, RoutedEventArgs e)
        {

        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {

        }

        private void AddVirtualTagClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
