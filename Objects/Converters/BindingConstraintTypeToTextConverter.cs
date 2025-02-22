using IncasEngine.ObjectiveEngine.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Incas.Objects.Converters
{
    public class BindingConstraintTypeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConstraintValue.ConstraintValueType enumValue)
            {
                switch (enumValue)
                {
                    case ConstraintValue.ConstraintValueType.ByFixedValue:
                        return "по значению";
                    case ConstraintValue.ConstraintValueType.ByField:
                        return "по полю класса";
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
