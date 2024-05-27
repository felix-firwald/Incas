using System;
using System.Windows;
using System.Windows.Threading;

namespace Incas.Core.Views.Windows
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
            this.InitializeComponent();
            if (type == BrokenType.Restart)
            {
                this.Header.Text = "Сессия принудительно остановлена для перезапуска";
                this.OtherText.Text = "Ожидайте";
                this.IconRestart.Visibility = System.Windows.Visibility.Visible;
                this.IconTerminate.Visibility = System.Windows.Visibility.Hidden;
            }
            this.StartCloseTimer();
        }
        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3d);
            timer.Tick += this.TimerTick;
            timer.Start();
        }
        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= this.TimerTick;
            Application.Current.Shutdown();
        }
    }
}
