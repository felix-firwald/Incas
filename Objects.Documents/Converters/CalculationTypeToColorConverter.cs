using Incas.Core.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;
using static IncasEngine.ObjectiveEngine.Types.Documents.ClassComponents.TemplateProperty;

namespace Incas.Objects.Documents.Converters
{
    /// <summary>
    /// Конвертирует значения перечисления в отображаемые на форме цвета
    /// </summary>
    class CalculationTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CalculationType enumValue)
            {
                switch (enumValue)
                {
                    case CalculationType.GlobalConstant:
                    case CalculationType.Constant:
                        return IncasEngine.Core.Color.FromRGB(255, 0, 51).AsBrush();
                    case CalculationType.Script:
                    case CalculationType.ScriptTable:
                    case CalculationType.ScriptPattern:
                    case CalculationType.Switch:
                    case CalculationType.Replication:
                        return IncasEngine.Core.Color.FromRGB(51, 255, 0).AsBrush();
                }
            }
            return IncasEngine.Core.Color.FromRGB(255, 255, 255).AsBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
