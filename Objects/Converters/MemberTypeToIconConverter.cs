using Incas.Core.Classes;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using static Incas.Objects.Interfaces.IClassMemberViewModel;

namespace Incas.Objects.Converters
{
    public class MemberTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MemberType enumValue)
            {
                switch (enumValue)
                {
                    case MemberType.Field:
                        return Geometry.Parse("M200-400q-33.85 0-56.92-23.08Q120-446.15 120-480t23.08-56.92Q166.15-560 200-560h560q33.85 0 56.92 23.08Q840-513.85 840-480t-23.08 56.92Q793.85-400 760-400H200Zm360-40h200q17 0 28.5-11.5T800-480q0-17-11.5-28.5T760-520H560v80Z");
                    case MemberType.Method:
                        return Geometry.Parse("M406.15-400H286.77q-16.31 0-23.58-14.19t2.12-27.27l225.15-326.16q6.16-8.61 15.23-11.42 9.08-2.81 19.16.89 10.07 3.69 14.61 12.15 4.54 8.46 3.31 18.54L514.62-520h143.46q16.77 0 23.81 15.31 7.03 15.31-3.81 28.38L427.54-176.15q-6.39 7.61-15.46 9.69-9.08 2.08-17.93-1.85-8.84-3.92-13.5-11.88-4.65-7.96-3.42-18.04L406.15-400Z");
                    case MemberType.Table:
                        return Geometry.Parse("M460-364.62H160v140q0 26.66 18.98 45.64T224.62-160H460v-204.62Zm40 0V-160h235.38q26.66 0 45.64-18.98T800-224.62v-140H500Zm-40-40v-204.61H160v204.61h300Zm40 0h300v-204.61H500v204.61ZM160-649.23h640v-86.15q0-26.66-18.98-45.64T735.38-800H224.62q-26.66 0-45.64 18.98T160-735.38v86.15Z");
                }
            }
            return IconsManager.GetGeometryIconByName(Icon.Database);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
