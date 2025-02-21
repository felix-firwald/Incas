using IncasEngine.Workspace;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Incas.Core.Converters
{
    class CommandTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MenuCommand.CommandType commandType)
            {
                switch (commandType)
                {
                    case MenuCommand.CommandType.Add:
                        return Geometry.Parse("M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0M8.5 4.5a.5.5 0 0 0-1 0v3h-3a.5.5 0 0 0 0 1h3v3a.5.5 0 0 0 1 0v-3h3a.5.5 0 0 0 0-1h-3z");
                    case MenuCommand.CommandType.Parse:
                        return Geometry.Parse("M5.1725,20.3615a1.52,1.52,0,0,1-1.534-1.5337V6.1723A1.52,1.52,0,0,1,5.177,4.6385h9.2585v.923H5.177a.5923.5923,0,0,0-.6155.6155V18.823a.5923.5923,0,0,0,.6155.6155H17.823a.5923.5923,0,0,0,.6155-.6155V9.5645h.923V18.823a1.52,1.52,0,0,1-1.5337,1.5385ZM7.323,16.923h.9232V11.0462H7.323Zm3.7232,0h.923V8.077h-.923Zm3.7075,0h.9233V14.0155h-.9233Zm2.7-8.3923V6.5462H15.4692V5.623h1.9845V3.6385h.9233V5.623h1.9845v.9232H18.377V8.5307Z");
                    case MenuCommand.CommandType.Find:
                        return Geometry.Parse("M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001q.044.06.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1 1 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0");
                    case MenuCommand.CommandType.FindAndOpen:
                        return Geometry.Parse("M15.502 1.94a.5.5 0 0 1 0 .706L14.459 3.69l-2-2L13.502.646a.5.5 0 0 1 .707 0l1.293 1.293zm-1.75 2.456-2-2L4.939 9.21a.5.5 0 0 0-.121.196l-.805 2.414a.25.25 0 0 0 .316.316l2.414-.805a.5.5 0 0 0 .196-.12l6.813-6.814z");
                    case MenuCommand.CommandType.Aggregate:
                        return Geometry.Parse("M4.7441,14.4677l-.8172-.5787,4.25-6.827,2.8845,3.3845L14.8691,4.235l2.8848,4.3462L21.11,3.212l.7788.598-4.123,6.6-2.86-4.321-3.7037,6.04-2.89-3.39ZM16.0426,19.212a2.8773,2.8773,0,0,0,2.034-4.9,2.8772,2.8772,0,0,0-4.0687,4.0692A2.7677,2.7677,0,0,0,16.0426,19.212Zm5.1573,3-2.8153-2.8155a3.757,3.757,0,0,1-1.08.602,3.8758,3.8758,0,0,1-4.0005-.913,3.8876,3.8876,0,0,1-.001-5.4745,3.8836,3.8836,0,0,1,5.4745-.0023,3.7234,3.7234,0,0,1,1.13,2.7378,3.7843,3.7843,0,0,1-.2135,1.2722,3.7313,3.7313,0,0,1-.6017,1.09l2.796,2.8153Z");
                }
            }
            return Geometry.Parse("M 10,50 A 40,40 0 1 1 90,50 A 40,40 0 1 1 10,50");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
