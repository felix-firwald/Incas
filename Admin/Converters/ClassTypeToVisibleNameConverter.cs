using IncasEngine.ObjectiveEngine.Classes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Incas.Admin.Converters
{
    public class ClassTypeToVisibleNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ClassType enumValue)
            {
                switch (enumValue)
                {
                    case ClassType.Model:
                        return "Модель данных";
                    case ClassType.Document:
                        return "Модель документа";
                    case ClassType.Event:
                        return "Модель события";
                    case ClassType.Process:
                        return "Модель процесса";
                    case ClassType.ServiceClassTask:
                        return "Модель задачи";
                    case ClassType.ServiceClassUser:
                        return "Модель пользователя";
                    case ClassType.ServiceClassGroup:
                        return "Модель группы";
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
