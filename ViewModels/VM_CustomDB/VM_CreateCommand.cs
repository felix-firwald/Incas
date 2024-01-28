using Incubator_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.ViewModels.VM_CustomDB
{
    public class VM_CreateCommand : VM_Base
    {
        public string Database { get; private set; }
        private string _table;
        public CustomTable Requester = new();

        public VM_CreateCommand(string database, string table)
        {
            Database = database;
            _table = table;
        }
        public string Table
        {
            get
            {
                return _table;
            }
        }
    }
}
