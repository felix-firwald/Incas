using Incas.Objects.AutoUI;
using IncasEngine.ObjectiveEngine.Common;
using System.Windows.Controls;
using System.Windows.Media;

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для StatusElement.xaml
    /// </summary>
    public partial class StatusElement : UserControl
    {
        private StatusData data;
        private int index;
        public delegate void StatusElementAction(int index, StatusData statusData);
        public event StatusElementAction OnEdit;
        public event StatusElementAction OnRemove;
        public StatusElement(int index, StatusData data)
        {
            this.InitializeComponent();
            this.index = index;
            this.data = data;
            this.MainBorder.Background = this.GetColor(data.Color, 50);
            this.MainLabel.Foreground = this.GetColor(data.Color);
            this.MainLabel.Content = data.Name;
        }
        public SolidColorBrush GetColor(IncasEngine.ObjectiveEngine.Common.Color color, byte a = 255)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(a, color.R, color.G, color.B));
        }

        private void EditClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            StatusSettings ss = new(this.data);
            if (ss.ShowDialog("Редактирование статуса", Core.Classes.Icon.Tag) == true)
            {
                this.OnEdit?.Invoke(this.index, ss.GetData());
            }
        }

        private void RemoveClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.OnRemove?.Invoke(this.index, this.data);
        }
    }
}
