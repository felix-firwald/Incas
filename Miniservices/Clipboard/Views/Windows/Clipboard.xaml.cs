using Incas.Miniservices.Clipboard.Classes;
using Incas.Miniservices.Clipboard.AutoUI;
using System.Windows;
using Incas.Miniservices.Clipboard.Views.Controls;
using System;
using Incas.Core.Classes;
using System.Windows.Input;
using Incas.Core.Views.Controls;
using System.Windows.Documents;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Incas.Miniservices.Clipboard.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для Clipboard.xaml
    /// </summary>
    public partial class Clipboard : Window
    {
        private bool AutoClose = false;
        public string SelectedText = "";
        public Clipboard(bool close = false)
        {
            this.AutoClose = close;
            this.InitializeComponent();
            this.UpdateRecordsView();
        }
        public void UpdateRecordsView()
        {
            this.PlaceElements(ClipboardManager.GetClipboardRecords());          
        }
        private void PlaceElements(List<ClipboardRecord> elements)
        {
            this.ContentPanel.Children.Clear();
            foreach (ClipboardRecord rec in elements)
            {
                ClipboardElement ce = new(rec);
                ce.OnUpdateRequested += this.OnUpdateRequested;
                ce.OnClicked += this.OnClicked;
                this.ContentPanel.Children.Add(ce);
            }
            if (this.ContentPanel.Children.Count == 0)
            {
                this.ContentPanel.Children.Add(new NoContent());
            }
        }

        private void OnClicked(string text)
        {
            this.SelectedText = text;
            if (this.AutoClose)
            {
                ClipboardManager.ClearCache();
                this.Close();
            }
            else
            {
                try
                {
                    System.Windows.Clipboard.SetText(this.SelectedText);
                }
                catch (Exception e)
                {
                    DialogsManager.ShowErrorDialog(e);
                }
            }
        }

        private void OnUpdateRequested()
        {
            this.FindText.Text = "";
            this.UpdateRecordsView();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            ClipboardManager.ClearCache();
            this.Close();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            ClipboardRecord cr = new();
            cr.ShowDialog("Добавление записи");
            this.UpdateRecordsView();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (this.FindText.Text.Length > 3)
            {
                this.PlaceElements(ClipboardManager.FindByName(this.FindText.Text));
            }
            else
            {
                this.UpdateRecordsView();
            }
        }
    }
}
