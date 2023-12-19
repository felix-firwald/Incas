using Common;
using Forms;
using Incubator_2.Forms.OneInstance;
using Incubator_2.Windows;
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
    /// Логика взаимодействия для UC_ListOfDocuments.xaml
    /// </summary>
    public partial class UC_ListOfDocuments : UserControl
    {
        public UC_ListOfDocuments()
        {
            InitializeComponent();
            LoadCategories();
            LoadTemplatesByCategory("");
        }
        private void LoadCategories()
        {
            this.Categories.Children.Clear();
            AddCategory("Без категории", true);
            Template mt = new Template();
            mt.GetCategories().ForEach(c =>
            {
                if (!string.IsNullOrEmpty(c))
                {
                    AddCategory(c);
                }
            });
        }
        private void AddCategory(string category, bool selected=false)
        {
            RadioButton rb = new RadioButton();
            rb.Style = FindResource("CategoryButton") as Style;
            rb.Content = category;
            rb.Click += new RoutedEventHandler(this.SelectCategory);
            rb.IsChecked = selected;
            this.Categories.Children.Add(rb);
        }
        private void LoadTemplatesByCategory(string category)
        {
            this.TemplatesArea.Children.Clear();
            Template mt = new Template();
            mt.GetWordTemplates(category).ForEach(c =>
            {
                this.TemplatesArea.Children.Add(new UC_TemplateElement(c));
            });
            if (this.TemplatesArea.Children.Count == 0)
            {
                this.TemplatesArea.Children.Add(new NoContent());
            }
        }

        private void SelectCategory(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string text = rb.Content.ToString();
            if (text != "Без категории")
            {
                LoadTemplatesByCategory(text);
            }
            else
            {
                LoadTemplatesByCategory("");
            }
            
        }

        private void AddFC_Click(object sender, MouseButtonEventArgs e)
        {
            //ProgramState.ShowErrorDialog("Данная функция ещё находится в разработке", "Функция недоступна");
            CreateTemplateWord ctw = new CreateTemplateWord();
            ctw.Show();
        }

        private void Refresh_Click(object sender, MouseButtonEventArgs e)
        {
            LoadCategories();
            LoadTemplatesByCategory("");
        }
    }
}
