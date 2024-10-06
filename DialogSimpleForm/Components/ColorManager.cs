using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Incas.DialogSimpleForm.Components
{
    public static class ColorManager
    {
        private static Dictionary<IconColor, Color> Colors = new()
        {
            {
                IconColor.Red,
                Color.FromRgb(255, 0, 51)
            },
            {
                IconColor.Blue,
                Color.FromRgb(30, 144, 255)
            },
            {
                IconColor.Green,
                Color.FromRgb(52, 201, 36)
            },
            {
                IconColor.Yellow,
                Color.FromRgb(245, 166, 35)
            }
        };
        public static SolidColorBrush GetColor(IconColor name)
        {
            return new(Colors[name]);
        }
    }
}
