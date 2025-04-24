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

                    case FieldType.Object:
#if E_BUSINESS
                    case FieldType.Structure:
                        return new SolidColorBrush(Color.FromRgb(132, 189, 253));
#endif
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
