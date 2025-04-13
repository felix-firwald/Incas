using IncasEngine.ObjectiveEngine.Classes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Incas.Admin.Converters
{
    public class ClassTypeToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ClassType enumValue)
            {
                switch (enumValue)
                {
                    case ClassType.Model:
                        return "Базовый класс, предназначенный для хранения структурированных данных";
                    case ClassType.Document:
                        return "Класс, позволяющий создавать документы по определенному шаблону и хранить структурированные данные";
                    case ClassType.Event:
                        return "Класс, позволяющий настроить автоматические оповещения о событиях, происходящих в рабочем пространства";
                    case ClassType.Process:
                        return "Класс, позволяющий создавать бизнес-процессы";
                    case ClassType.StaticModel:
                        return "Класс, позволяющий хранить структурированные данные в одном экземпляре в разрезе периода времени";
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
