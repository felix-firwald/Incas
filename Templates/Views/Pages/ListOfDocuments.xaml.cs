using Incas.Common;
using Incas.Core.Views.Controls;
using Incas.Templates.Models;
using Incas.Templates.Views.Controls;
using Incas.Templates.Views.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incas.Templates.Views.Pages
{

    /// <summary>
    /// Логика взаимодействия для ListOfDocuments.xaml
    /// </summary>
    public partial class ListOfDocuments : UserControl
    {
        private string selectedCategory = "";
        public ListOfDocuments()
        {
            this.InitializeComponent();
            this.LoadCategories();
            this.LoadTemplatesByCategory("");
            //LoadChildrenForTemplates("");
        }
        private void LoadCategories()
        {
            this.Categories.Children.Clear();
            this.AddCategory("Без категории", true);
            Template mt = new();
            mt.GetCategories(["Excel", "Word"]).ForEach(c =>
            {
                if (!string.IsNullOrEmpty(c))
                {
                    this.AddCategory(c);
                }
            });
        }
        private void AddCategory(string category, bool selected = false)
        {
            RadioButton rb = new()
            {
                Style = this.FindResource("CategoryButton") as Style,
                Content = category
            };
            rb.Click += new RoutedEventHandler(this.SelectCategory);
            rb.IsChecked = selected;
            this.Categories.Children.Add(rb);
        }
        private void LoadTemplatesByCategory(string category)
        {
            this.TemplatesArea.Children.Clear();
            using (Template mt = new())
            {
                mt.GetAllWordExcelTemplates(category).ForEach(c =>
                {
                    TemplateElement te = new(c);
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
                CreateDocumentTemplate ctw = new();
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

        private void Refresh_Click(object sender, MouseButtonEventArgs e)
        {
            this.Refresh();
        }
    }
}
