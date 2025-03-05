using Incas.Core.Extensions;
using IncasEngine.ObjectiveEngine.Common.FunctionalityUtils.CustomForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Incas.Objects.Converters
{
    class ViewControlTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ControlType enumValue)
            {
                switch (enumValue)
                {
                    case ControlType.FieldFiller:
                        return IncasEngine.Core.Color.FromRGB(127, 192, 77).AsBrush();                    
                }
            }
            return IncasEngine.Core.Color.FromRGB(245, 166, 35).AsBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
