using Common;
using Models;
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

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_EnumerationElement.xaml
    /// </summary>
    public partial class UC_EnumerationElement : UserControl
    {
        Enumeration enumeration;
        public UC_EnumerationElement(Enumeration en)
        {
            InitializeComponent();
            this.enumeration = en;
            this.MainLabel.Content = this.enumeration.name;
            this.ToolTip = MakeContent();
        }
        private string MakeContent()
        {
            string res = "У данного перечисления следующие элементы:\n";
            int i = 1;
            foreach (string s in this.enumeration.content.Split('\n'))
            {
                res += $"{i}. {s}\n";
                i++;
            }
            return res;
        }

        private void RemoveElement(object sender, MouseButtonEventArgs e)
        {
            if (ProgramState.IsWorkspaceOpened())
            {
                if (ProgramState.ShowQuestionDialog(
                    "Это глобальное перечисление будет безвозвратно удалено, " +
                    "но у тех шаблонов, которые уже его используют, " +
                    "оно сохранится в качестве локального.",
                    "Удалить перечисление?", "Удалить", "Не удалять") == Windows.DialogStatus.Yes)
                {
                    Tag t = new Tag();
                    t.SwitchGlobalToLocalEnumeration(this.enumeration);
                    this.enumeration.RemoveEnumeration();
                }
            }
        }
    }
}
