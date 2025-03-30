using System;
using System.Globalization;
using System.Windows.Data;
using static IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents.PropertyReplicator;

namespace Incas.Objects.Documents.Converters
{
    public class ReplicationTargetTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReplicationTargetType enumValue)
            {
                switch (enumValue)
                {
                    case ReplicationTargetType.Text:
                        return "Текст";
                    case ReplicationTargetType.FormattedText:
                        return "Форматированный текст";
                    case ReplicationTargetType.Table:
                        return "Таблица";
                    case ReplicationTargetType.Image:
                        return "Изображение";
                    case ReplicationTargetType.QRCode:
                        return "Генерация QR-кода";
                    case ReplicationTargetType.Barcode:
                        return "Генерация штрих-кода";
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
