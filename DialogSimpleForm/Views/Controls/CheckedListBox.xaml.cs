using Incas.DialogSimpleForm.Components;
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
                this.ContentPanel.Children.Add(box);
            }
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
