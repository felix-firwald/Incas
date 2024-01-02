using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Incubator_2.Windows
{
    public enum BrokenType
    {
        Terminate,
        Restart
    }
    /// <summary>
    /// Логика взаимодействия для SessionBroken.xaml
    /// </summary>
    public partial class SessionBroken : Window
    {
        public SessionBroken(BrokenType type = BrokenType.Terminate)
        {
            InitializeComponent();
            if (type == BrokenType.Restart)
            {
                this.Header.Text = "Сессия принудительно остановлена для перезапуска";
                this.OtherText.Text = "Ожидайте";
                this.IconRestart.Visibility = System.Windows.Visibility.Visible;
                this.IconTerminate.Visibility = System.Windows.Visibility.Hidden;
            }
            StartCloseTimer();
        }
        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3d);
            timer.Tick += TimerTick;
            timer.Start();
        }
        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            Application.Current.Shutdown();
        }
    }
}
