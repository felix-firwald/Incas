using Incas.Objects.Documents.Components;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Incas.Objects.Documents.Converters
{
    public class TagTargetToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TagTarget enumValue)
            {
                switch (enumValue)
                {
                    case TagTarget.Property:
                        return "Импортировать как свойство шаблона";
                    case TagTarget.Field:
                        return "Импортировать как поле класса";
                    case TagTarget.Useless:
                        return "Не импортировать";
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
