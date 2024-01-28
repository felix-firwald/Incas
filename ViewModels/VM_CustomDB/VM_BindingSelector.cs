using Incubator_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_BindingSelector : VM_Base
    {
        private SDatabase _selectedDB;
        private string _selectedTable;
        private string _selectedField;
        public VM_BindingSelector()
        {

        }
        public List<SDatabase> Databases
        {
            get
            {
                using (Database db = new())
                {
                    return db.GetActualDatabases();
                }
            }
        }
        public SDatabase SelectedDatabase
        {
            get
            {
                return _selectedDB;
            }
            set
            {
                _selectedDB = value;
                OnPropertyChanged(nameof(SelectedDatabase));
                OnPropertyChanged(nameof(Tables));
            }
        }
        public List<string> Tables
        {
            get
            {
                using (CustomTable ct = new())
                {
                    return ct.GetTablesList(SelectedDatabase.path, TableType.All);
                }
            }
        }
        public string SelectedTable
        {
            get
            {
                return _selectedTable;
            }
            set
            {
                _selectedTable = value;
                OnPropertyChanged(nameof(SelectedTable));
                OnPropertyChanged(nameof(Fields));
            }
        }
        public List<string> Fields
        {
            get
            {
                using (CustomTable ct = new())
                {
                    return ct.GetTableFieldsSimple(SelectedTable, SelectedDatabase.path);
                }
            }
        }
        public string SelectedField
        {
            get
            {
                return _selectedField;
            }
            set
            {
                _selectedField = value;
                OnPropertyChanged(nameof(SelectedField));
            }
        }
    }
}
