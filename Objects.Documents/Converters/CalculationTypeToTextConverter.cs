﻿using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Incas.Objects.Documents.Converters
{
    public class CalculationTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TemplateProperty.CalculationType type)
            {
                switch (type)
                {
                    case TemplateProperty.CalculationType.Constant:
                        return "Константа";
                    case TemplateProperty.CalculationType.GlobalConstant:
                        return "Глобальная константа";
                    case TemplateProperty.CalculationType.Switch:
                        return "Словарь сопоставления";
                    case TemplateProperty.CalculationType.Script:
                        return "Переменная скрипта (текст)";
                    case TemplateProperty.CalculationType.ScriptTable:
                        return "Переменная скрипта (таблица)";
                    case TemplateProperty.CalculationType.ScriptPattern:
                        return "Переменная скрипта (коллекция паттернов)";
                    case TemplateProperty.CalculationType.ScriptQRCode:
                        return "Репликация";
                }
            }
            return "Не определено";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
