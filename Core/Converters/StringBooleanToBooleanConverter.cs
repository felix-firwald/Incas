using System;
using System.Globalization;
using System.Windows.Data;

namespace Incas.Core.Converters
{
    public class StringBooleanToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string enumValue)
            {
                switch (enumValue)
                {
                    case "True":
                        return true;
                    case "False":
                        return false;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
