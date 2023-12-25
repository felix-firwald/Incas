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
        private Tag _tag;
        public NotOverridenTag(Tag tag)
        {
            InitializeComponent();
            _tag = tag;
            this.TagName.Content = tag.name;
        }

        private void OverrideClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.IsEnabled = false;
            this.Override.Visibility = Visibility.Collapsed;
            onOverride(_tag);
        }
    }
}
