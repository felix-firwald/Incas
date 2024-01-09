using DocumentFormat.OpenXml.Drawing.Charts;
using Incubator_2.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для TagCreator.xaml
    /// </summary>
    public partial class TagCreator : UserControl
    {
        public delegate void MethodContainer(TagCreator t);
        public event MethodContainer onDelete;
        public Tag tag;
        VM_Tag vm;

        private bool IsCollapsed = false;
        public TagCreator(Tag t, bool isNew = false)
        {
            InitializeComponent();
            tag = t;
            vm = new(t);
            this.DataContext = vm;
            if (t.parent is not 0)
            {
                this.OverridenLabel.Visibility = System.Windows.Visibility.Visible;
                this.TagName.IsEnabled = false;
            }
        }

        

        public void SaveTag(int templ, bool isEdit=false)
        {
            tag.template = templ;
            if (isEdit)
            {
                tag.UpdateTag();
            }
            else
            {
                tag.AddTag();
            }
        }
        public void Minimize()
        {
            this.MainBorder.Height = 40;
            this.IsCollapsed = !this.IsCollapsed;
        }
        public void Maximize()
        {
            this.MainBorder.Height = this.ContentPanel.Height + 40;
            this.IsCollapsed = !this.IsCollapsed;
        }

        private void TurnSizeClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsCollapsed)
            {
                Maximize();
            }
            else
            {
                Minimize();
            }
        }

        private void RemoveClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            onDelete?.Invoke(this);
            tag.RemoveTag();
        }

        private void CopyAllClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Clipboard.SetText(this.tag.value);
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            this.vm.DefaultValue = "";
        }
    }
}
