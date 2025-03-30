using System;
using System.Globalization;
using System.Windows.Data;
using static IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents.PropertyReplicator;

namespace Incas.Objects.Documents.Converters
{
    public class ReplicationSourceTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReplicationSourceType enumValue)
            {
                switch (enumValue)
                {
                    case ReplicationSourceType.Field:
                        return "Поле класса";
                    case ReplicationSourceType.Property:
                        return "Свойство шаблона";
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
