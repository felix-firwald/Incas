using Incubator_2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    class VM_CustomDatabase : VM_Base
    {
        private string _selectedTable = "";
        CustomTable requester = new();
        public VM_CustomDatabase() { }
        public DataTable Table
        {
            get
            {
                if (_selectedTable != "")
                {
                    return requester.GetTable(SelectedTable);
                }
                else
                {
                    return new();
                }
            }
        }
        public List<string> Tables
        {
            get
            {
                return requester.GetTablesList();
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
                OnPropertyChanged(nameof(Table));
            }
        }
    }
}
