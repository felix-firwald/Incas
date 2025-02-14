using IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    case TemplateProperty.CalculationType.Switch:
                        return "Разветвитель";
                    case TemplateProperty.CalculationType.Script:
                        return "Скрипт";
                    case TemplateProperty.CalculationType.Replication:
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
