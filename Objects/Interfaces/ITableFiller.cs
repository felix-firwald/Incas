using System.Data;
using System.Windows;

namespace Incas.Objects.Interfaces
{
    public interface ITableFiller
    {
        public DataTable GetValue();
        public event RoutedEventHandler OnCustomButtonClicked;
    }
}
