using System;
using System.Collections.Generic;
using System.Reflection;

namespace Incas.Core.Classes
{
    public struct FieldCreator
    {
        public string Name = "";
        public string TypeOf = "";
        public bool NotNULL = false;
        public bool IsUNIQUE = false;
        public bool IsPK = false;
        public string FKtable = null;
        public string FKfield = null;

        public FieldCreator(string nam, string typeOf)
        {
            Name = nam;
            TypeOf = typeOf;
        }
        private string GetNull()
        {
            return NotNULL ? "NOT NULL" : "";
        }
        private string GetFK()
        {
            return FKtable != null ? $"REFERENCES [{FKtable}] ([{FKfield}]) ON DELETE CASCADE" : "";
        }

        private string GetUniq()
        {
            return IsUNIQUE ? "UNIQUE ON CONFLICT ROLLBACK" : "";
        }
        public override string ToString()
        {
            return IsPK
                ? $"[{Name}] INTEGER PRIMARY KEY AUTOINCREMENT"
                : $"[{Name}] {TypeOf} {GetNull()} {GetUniq()} {GetFK()}".Trim();
        }
    }
}
