using Common;
using Forms;
using Incubator_2.Forms.OneInstance;
using Incubator_2.Windows;
using Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Incubator_2.Forms
{
    /// <summary>
    /// Логика взаимодействия для UC_ListOfDocuments.xaml
    /// </summary>
    public partial class UC_ListOfDocuments : UserControl
    {
        private string selectedCategory = "";
        public UC_ListOfDocuments()
        {
            InitializeComponent();
            LoadCategories();
            LoadTemplatesByCategory("");
            //LoadChildrenForTemplates("");
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
        private void AddCategory(string category, bool selected = false)
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
            using (Template mt = new Template())
            {
                mt.GetAllWordExcelTemplates(category).ForEach(c =>
                {
                    UC_TemplateElement te = new UC_TemplateElement(c);
                    te.OnUpdated += Refresh;
                    this.TemplatesArea.Children.Add(te);
                });
                if (this.TemplatesArea.Children.Count == 0)
                {
                    this.TemplatesArea.Children.Add(new NoContent());
                }
            }
            GC.Collect();
        }
        private void LoadChildrenForTemplates()
        {
            List<int> ids = new List<int>();
            if (this.TemplatesArea.Children[0] is NoContent)
            {
                return;
            }
            foreach (UC_TemplateElement element in this.TemplatesArea.Children)
            {
                ids.Add(element.template.id);
            }
            if (ids.Count > 0)
            {
                using (Template mt = new Template())
                {
                    mt.GetAllChildren(ids).ForEach(c =>
                    {
                        foreach (UC_TemplateElement element in this.TemplatesArea.Children)
                        {
                            if (element.template.id == c.parent)
                            {
                                element.AddChild(c);
                            }
                        }
                    });
                }
            }

        }

        private void SelectCategory(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string text = rb.Content.ToString();
            if (text != "Без категории")
            {
                LoadTemplatesByCategory(text);
                LoadChildrenForTemplates();
                this.selectedCategory = text;
            }
            else
            {
                LoadTemplatesByCategory("");
                LoadChildrenForTemplates();
                this.selectedCategory = "";
            }

        }

        private void AddFC_Click(object sender, MouseButtonEventArgs e)
        {
            if (ProgramState.IsWorkspaceOpened())
            {
                CreateTemplateWord ctw = new CreateTemplateWord();
                ctw.OnCreated += Refresh;
                ctw.Show();
            }

        }
        private void FindSelectedInRefreshedList()
        {
            foreach (RadioButton rb in this.Categories.Children)
            {
                if (rb.Content.ToString() == selectedCategory)
                {
                    rb.IsChecked = true;
                    LoadTemplatesByCategory(selectedCategory);
                    return;
                }
            }
            RadioButton def = (RadioButton)this.Categories.Children[0];
            def.IsChecked = true;
            selectedCategory = "";
            LoadTemplatesByCategory("");
            return;
        }

        public void Refresh()
        {
            LoadCategories();
            FindSelectedInRefreshedList();
            LoadChildrenForTemplates();
        }

        private void Refresh_Click(object sender, MouseButtonEventArgs e)
        {
            Refresh();
        }
    }
}
