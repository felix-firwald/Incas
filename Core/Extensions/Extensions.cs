using Avalonia.Controls;
using Incas.Core.Classes;
using IncasEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Incas.Core.Extensions
{
    public static class Extensions
    {
        public static System.Windows.Media.Color AsMediaColor(this IncasEngine.Core.Color input)
        {
            System.Windows.Media.Color output = new()
            {
                R = input.R,
                G = input.G,
                B = input.B
            };
            return output;
        }
        public static System.Windows.Media.SolidColorBrush AsBrush(this IncasEngine.Core.Color input)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromRgb(input.R, input.G, input.B));
        }
        public static IncasEngine.Core.Color AsIncasEngineColor(this System.Windows.Media.Color input)
        {
            IncasEngine.Core.Color output = new()
            {
                R = input.R,
                G = input.G,
                B = input.B
            };
            return output;
        }
        public static System.Windows.Media.Geometry ParseAsGeometry(this string geometryString)
        {
            return Geometry.Parse(geometryString);
        }
    }
}
