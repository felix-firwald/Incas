using Incas.Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Incas.Objects.Components
{
    public class DataGridNumericColumn : DataGridTextColumn
    {
        protected override object PrepareCellForEdit(System.Windows.FrameworkElement editingElement, System.Windows.RoutedEventArgs editingEventArgs)
        {
            TextBox edit = editingElement as TextBox;
            if (edit != null)
            {
                edit.PreviewTextInput += this.OnPreviewTextInput;
            }
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        private void OnPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            int value;
            if (!int.TryParse(e.Text, out value))
            {
                //Regex.Replace(e.Text, "[^А-Яа-я.]", "");
                e.Handled = true;
            }               
        }
    }
}
