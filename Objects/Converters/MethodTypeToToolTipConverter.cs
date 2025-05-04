using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Incas.Objects.Converters
{
    public class MethodTypeToToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (boolValue) // if static
                {
                    return "Это статический метод (применяемый к неопределенному множеству объектов)";
                }
                else // if requires instance
                {
                    return "Это объективный метод (применяемый к конкретному объекту)";
                }
            }
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
