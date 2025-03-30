using IncasEngine.ObjectiveEngine.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Incas.Objects.Converters
{
    public class FieldTypeToColorConverter : IValueConverter
    {
        public static Dictionary<FieldType, SolidColorBrush> Dictionary = new()
        {
            { FieldType.String, new(new() {R=52, G=201, B=36}) },
            { FieldType.Text, new(new() {R=52, G=201, B=36}) },

            { FieldType.LocalEnumeration, new(new() {R=245, G=166, B=35}) },
            { FieldType.GlobalEnumeration, new(new() {R=245, G=166, B=35}) },
            { FieldType.Date, new(new() {R=245, G=166, B=35}) },
            { FieldType.Integer, new(new() {R=245, G=166, B=35}) },
            { FieldType.Boolean, new(new() {R=245, G=166, B=35}) },

            { FieldType.LocalConstant, new(new() {R=255, G=0, B=51}) },
            { FieldType.GlobalConstant, new(new() {R=255, G=0, B=51}) },
            { FieldType.HiddenField, new(new() {R=255, G=0, B=51}) },

            { FieldType.Object, new(new() {R=139, G=0, B=255}) },
            { FieldType.Table, new(new() {R=139, G=0, B=255}) }
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FieldType enumValue)
            {
                switch (enumValue)
                {
                    case FieldType.String:
                        return new SolidColorBrush(Color.FromRgb(132, 189, 253));

                    case FieldType.Text:
                        return new SolidColorBrush(Color.FromRgb(149, 240, 172));

                    case FieldType.LocalEnumeration:
                    case FieldType.GlobalEnumeration:
                    case FieldType.Boolean:
                        return new SolidColorBrush(Color.FromRgb(250, 174, 122));

                    case FieldType.Date:

                    case FieldType.Integer:
                    case FieldType.Float:
                        return new SolidColorBrush(Color.FromRgb(132, 189, 253));

                    case FieldType.LocalConstant:
                    case FieldType.GlobalConstant:
                    case FieldType.HiddenField:
                        return new SolidColorBrush(Color.FromRgb(255, 0, 51));

                    case FieldType.Object:
#if E_BUSINESS
                    case FieldType.Structure:
#endif
                    case FieldType.Table:
                        return new SolidColorBrush(Color.FromRgb(183, 153, 241));
                }
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
