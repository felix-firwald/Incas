using Incas.Core.Classes;
using IncasEngine.ObjectiveEngine.Interfaces;
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

namespace Incas.Objects.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для StateUndefinedMessage.xaml
    /// </summary>
    public partial class StateUndefinedMessage : UserControl
    {
        public IObject Object { get; set; }
        public StateUndefinedMessage(IObject obj)
        {
            this.InitializeComponent();
            this.Object = obj;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateButton.Visibility = Visibility.Collapsed;
            if (this.Object == null || this.Object.Id == Guid.Empty)
            {
                DialogsManager.ShowExclamationDialog("Объект не имеет идентификатора, поэтому состояние не может быть сброшено.", "Действие невозможно");
                return;
            }
            IClassData data = this.Object.Class.GetClassData();
            if (data.States.Count > 0)
            {
                this.Object.State = data.States[0].Id;
            }           
        }
    }
}
