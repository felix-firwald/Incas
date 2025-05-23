﻿using Incas.Core.Extensions;
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
                    case ControlType.Table:
                        return IncasEngine.Core.Color.FromRGB(127, 192, 77).AsBrush();
                    case ControlType.Button:
                        return IncasEngine.Core.Color.FromRGB(191, 77, 166).AsBrush();
                    case ControlType.Text:
                        return IncasEngine.Core.Color.FromRGB(119, 221, 231).AsBrush();
                }
            }
            return IncasEngine.Core.Color.FromRGB(247, 189, 91).AsBrush();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
