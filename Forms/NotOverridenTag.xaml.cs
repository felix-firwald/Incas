using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Incubator_2.Windows;
using Models;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для NotOverridenTag.xaml
    /// </summary>
    public partial class NotOverridenTag : UserControl
    {
        public delegate void MethodContainer(Tag t);
        public event MethodContainer onOverride;
        public Tag tag;
        public NotOverridenTag(Tag t)
        {
            InitializeComponent();
            tag = t;
            this.TagName.Content = tag.name;
        }

        public void OverrideTag(bool notify = true)
        {
            this.IsEnabled = false;
            this.Override.Visibility = Visibility.Collapsed;
            if (notify) { onOverride(tag); }
        }
        public void DeOverrideTag()
        {
            this.IsEnabled = true;
            this.Override.Visibility = Visibility.Visible;
        }

        private void OverrideClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OverrideTag();
        }
    }
}
