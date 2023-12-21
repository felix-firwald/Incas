using Incubator_2.ViewModels;
using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;


namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для TagCreator.xaml
    /// </summary>
    public partial class TagCreator : UserControl
    {
        Tag tag;
        private bool IsCollapsed = false;
        public TagCreator(Tag t, bool isNew = false)
        {
            InitializeComponent();
            tag = t;
            this.DataContext = new VM_Tag(t);
            if (!isNew)
            {

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

        private void Minimize(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsCollapsed)
            {
                this.MainBorder.Height = this.ContentPanel.Height + 40;
            }
            else
            {
                this.MainBorder.Height = 40;
            }
            this.IsCollapsed = !this.IsCollapsed;
        }
    }
}
