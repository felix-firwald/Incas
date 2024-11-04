using Incas.Core.Classes;
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

namespace Incas.Core.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : UserControl
    {
        public StartPage()
        {
            this.InitializeComponent();
            this.SetMainText();
        }
        public void SetMainText()
        {
            string result = "";
            string user = ProgramState.CurrentUser.secondName;
            int hour = DateTime.Now.Hour;
            switch (hour)
            {
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                    result = "Доброе утро, " + user;
                    break;
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                    result = "Добрый день, " + user;
                    break;
                default:
                    result = "Добрый вечер, " + user;
                    break;
            }
            this.MainText.Content = result;
        }
    }
}
