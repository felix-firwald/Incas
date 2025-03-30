using IncasEngine.ObjectiveEngine.Classes;
using IncasEngine.ObjectiveEngine.FieldComponents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Incas.Objects.Converters
{
    internal class FieldTypeToVisibleNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FieldType enumValue)
            {
                switch (enumValue)
                {
                    case FieldType.String:
                        return "Строка";
                    case FieldType.Text:
                        return "Текст";
                    case FieldType.LocalEnumeration:
                        return "Перечисление";
                    case FieldType.GlobalEnumeration:
                        return "Глобальное перечисление";
                    case FieldType.Date:
                        return "Дата";
                    case FieldType.Integer:
                        return "Целочисленное число";
                    case FieldType.Float:
                        return "Дробное число";
                    case FieldType.Boolean:
                        return "Логический флаг";
                    case FieldType.LocalConstant:
                        return "Константа";
                    case FieldType.GlobalConstant:
                        return "Глобальная константа";
                    case FieldType.HiddenField:
                        return "Скрытое поле";
                    case FieldType.Object:
                        return "Объект";
#if E_BUSINESS
                    case FieldType.Structure:
                        return "Структура";
#endif
                    case FieldType.Table:
                        return "Таблица";
                }
            }
            return "Неизвестно";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
