using Incas.Core.Classes;
using System.ComponentModel;
using System.Windows.Controls;

namespace Incas.Core.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для PathSelector.xaml
    /// </summary>
    public partial class PathSelector : UserControl, INotifyPropertyChanged
    {
        private string currentValue;
        public string Value
        {
            get => this.currentValue is null ? this.Input.Text : this.currentValue;

            set
            {
                this.currentValue = value;
                this.OnPropertyChanged(nameof(this.Value));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public PathSelector()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private void Path_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string path = this.currentValue;
            if (DialogsManager.ShowFolderDialog(ref path))
            {
                this.Value = path;
            }
        }
    }
}
