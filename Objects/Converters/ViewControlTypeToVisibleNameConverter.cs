using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Incas.Objects.Converters
{
    public class ViewControlTypeToVisibleNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ControlType enumValue)
            {
                switch (enumValue)
                {
                    case ControlType.VerticalStack:
                        return "Вертикальный стек";
                    case ControlType.HorizontalStack:
                        return "Горизонтальный стек";
                    case ControlType.Grid:
                        return "Сетка";
                    case ControlType.Tab:
                        return "Группа вкладок";
                    case ControlType.TabItem:
                        return "Вкладка";
                    case ControlType.Group:
                        return "Группа";
                    case ControlType.FieldFiller:
                        return "Поле";
                }
            }
            return "Авто";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
