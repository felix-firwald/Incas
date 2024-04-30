using Common;
using Forms;
using Incubator_2.Forms.OneInstance;
using Incubator_2.Windows;
using Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.Forms.Mail
{
    /// <summary>
    /// Логика взаимодействия для MailMain.xaml
    /// </summary>
    public partial class MailMain : UserControl
    {
        private string selectedCategory = "";
        public MailMain()
        {
            InitializeComponent();
            this.LoadCategories();
            this.LoadTemplatesByCategory("");
        }
        private void LoadCategories()
        {
            this.Categories.Children.Clear();
            this.AddCategory("Без категории", true);
            Template mt = new Template();
            mt.GetCategories(new() { "Mail" }).ForEach(c =>
            {
                if (!string.IsNullOrEmpty(c))
                {
                    this.AddCategory(c);
                }
            });
        }
        private void AddCategory(string category, bool selected = false)
        {
            RadioButton rb = new RadioButton();
            rb.Style = this.FindResource("CategoryButton") as Style;
            rb.Content = category;
            rb.Click += new RoutedEventHandler(this.SelectCategory);
            rb.IsChecked = selected;
            this.Categories.Children.Add(rb);
        }
        private void LoadTemplatesByCategory(string category)
        {
            this.TemplatesArea.Children.Clear();
            using (Template mt = new Template())
            {
                mt.GetAllMailTemplates(category).ForEach(c =>
                {
                    UC_TemplateElement te = new UC_TemplateElement(c);
                    te.OnUpdated += this.Refresh;
                    this.TemplatesArea.Children.Add(te);
                });
                if (this.TemplatesArea.Children.Count == 0)
                {
                    this.TemplatesArea.Children.Add(new NoContent());
                }
            }
            GC.Collect();
        }

        private void SelectCategory(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string text = rb.Content.ToString();
            if (text != "Без категории")
            {
                this.LoadTemplatesByCategory(text);
                this.selectedCategory = text;
            }
            else
            {
                this.LoadTemplatesByCategory("");
                this.selectedCategory = "";
            }

        }

        private void AddFC_Click(object sender, MouseButtonEventArgs e)
        {
            if (ProgramState.IsWorkspaceOpened())
            {
                CreateTemplateWord ctw = new CreateTemplateWord();
                ctw.OnCreated += this.Refresh;
                ctw.Show();
            }

        }
        private void FindSelectedInRefreshedList()
        {
            foreach (RadioButton rb in this.Categories.Children)
            {
                if (rb.Content.ToString() == this.selectedCategory)
                {
                    rb.IsChecked = true;
                    this.LoadTemplatesByCategory(this.selectedCategory);
                    return;
                }
            }
            RadioButton def = (RadioButton)this.Categories.Children[0];
            def.IsChecked = true;
            this.selectedCategory = "";
            this.LoadTemplatesByCategory("");
            return;
        }

        public void Refresh()
        {
            this.LoadCategories();
            this.FindSelectedInRefreshedList();
        }


        private void CreateTemplateClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void RefreshClick(object sender, MouseButtonEventArgs e)
        {
            this.Refresh();
        }
    }
}
