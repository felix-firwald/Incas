using IncasEngine.Workspace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Incas.Core.Converters
{
    public class CommandTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MenuCommand.CommandType commandType)
            {
                switch (commandType)
                {
                    case MenuCommand.CommandType.Add:
                    case MenuCommand.CommandType.Parse:
                        return new SolidColorBrush(Color.FromRgb(186, 255, 120));
                    case MenuCommand.CommandType.Find:
                    case MenuCommand.CommandType.FindAndOpen:
                    case MenuCommand.CommandType.Aggregate:
                        return new SolidColorBrush(Color.FromRgb(30, 144, 255));
                }
            }
            return new SolidColorBrush(Color.FromRgb(178, 85, 255));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
