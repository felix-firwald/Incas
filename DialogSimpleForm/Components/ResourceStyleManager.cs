using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Incas.DialogSimpleForm.Components
{
    class ResourceStyleManager : FrameworkElement
    {
        public const string TextBoxStyle = "TextBoxMain";
        public const string TextBoxBigStyle = "TextBoxBig";
        public const string TextBoxGridStyle = "TextBoxGrid";
        public const string IntegerUpDownStyle = "NumericUpDown";
        public const string IntegerUpDownGridStyle = "NumericUpDownGrid";
        public const string DatePickerStyle = "DatePickerMain";
        public const string DatePickerGridStyle = "DatePickerGrid";
        public const string ComboboxStyle = "ComboBoxMain";
        public const string ComboboxGridStyle = "ComboBoxGrid";
        public const string CheckboxStyle = "CheckBoxMain";
        public const string CheckboxEditingGridStyle = "CheckBoxDataGrid";
        public const string CheckboxNotEditableGridStyle = "CheckBoxDataGridUsual";
        public const string ButtonRectangle = "ButtonRectangle";
        public const string ButtonSquare = "ButtonSquare";
        public const string FontRubik = "Rubik";
        public const string FontJetBrains = "JetBrains";
        private static Dictionary<string, Style> cache = new();
        private static Dictionary<string, FontFamily> cacheFonts = new();
        public static Style FindStyle(string name)
        {
            if (cache.ContainsKey(name))
            {
                return cache[name];
            }
            ResourceStyleManager inst = new();
            cache[name] = inst.FindResource(name) as Style;
            return cache[name];
        }
        public static FontFamily FindFontFamily(string name)
        {
            if (cacheFonts.ContainsKey(name))
            {
                return cacheFonts[name];
            }
            ResourceStyleManager inst = new();
            cacheFonts[name] = inst.FindResource(name) as FontFamily;
            return cacheFonts[name];
        }
    }
}
