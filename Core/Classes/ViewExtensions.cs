using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Incas.Core.Classes
{
    public static class ViewExtensions
    {
        public static string TryGetGeometryFromSvgPath(string svgContent)
        {
            if (string.IsNullOrWhiteSpace(svgContent))
            {
                return null;
            }
            // Регулярное выражение для извлечения атрибута "d" из элемента <path>
            Regex regex = new(@"<path[^>]*?d=""(?<data>[^""]*)""[^>]*?\/>", RegexOptions.IgnoreCase);
            Match match = regex.Match(svgContent);

            if (match.Success)
            {
                string pathData = match.Groups["data"].Value;
                try
                {
                    Geometry.Parse(pathData);
                    return pathData;
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
