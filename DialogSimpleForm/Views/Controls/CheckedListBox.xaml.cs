using Incas.DialogSimpleForm.Components;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Windows.Networking.BackgroundTransfer;

namespace Incas.DialogSimpleForm.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для CheckedListBox.xaml
    /// </summary>
    public partial class CheckedListBox : UserControl
    {
        private readonly CheckedList List;
        public delegate void CheckedDelegate(string content, object id, bool isChecked);
        public event CheckedDelegate OnCheckedStateChanged;
        public CheckedListBox(CheckedList list)
        {
            this.InitializeComponent();
            this.List = list;
            this.PlaceUIControls();
        }
        private void PlaceUIControls()
        {
            this.ContentPanel.Children.Clear();
            foreach (KeyValuePair<CheckedItem, bool> pair in this.List.Pairs)
            {
                CheckBox box = new()
                {
                    Content = pair.Key.Name,
                    Tag = pair.Key.Target,
                    IsChecked = pair.Value
                };
                box.Checked += this.Box_Checked;
                box.Unchecked += this.Box_Unchecked;
                this.ContentPanel.Children.Add(box);
            }
        }
        private void CheckedHandle(object sender, bool status)
        {
            CheckBox cb = (CheckBox)sender;
            this.OnCheckedStateChanged?.Invoke(cb.Content.ToString(), cb.Tag, status);
        }
        private void Box_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.CheckedHandle(sender, false);
        }

        private void Box_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.CheckedHandle(sender, true);
        }

        public CheckedList GetResult()
        {
            Dictionary<CheckedItem, bool> newDict = new();
            foreach (CheckBox cb in this.ContentPanel.Children)
            {
                foreach (KeyValuePair<CheckedItem, bool> pair in this.List.Pairs)
                {
                    if (pair.Key.Target == cb.Tag)
                    {
                        newDict.Add(pair.Key, (bool)cb.IsChecked);
                        break;
                    }
                }
            }
            this.List.Pairs = newDict;
            return this.List;
        }
    }
}
