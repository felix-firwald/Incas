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
            this.Name = nam;
            this.TypeOf = typeOf;
        }
        private string GetNull()
        {
            return this.NotNULL ? "NOT NULL" : "";
        }
        private string GetFK()
        {
            return this.FKtable != null ? $"REFERENCES [{this.FKtable}] ([{this.FKfield}]) ON DELETE CASCADE" : "";
        }

        private string GetUniq()
        {
            return this.IsUNIQUE ? "UNIQUE ON CONFLICT ROLLBACK" : "";
        }
        public override string ToString()
        {
            return this.IsPK
                ? $"[{this.Name}] INTEGER PRIMARY KEY AUTOINCREMENT"
                : $"[{this.Name}] {this.TypeOf} {this.GetNull()} {this.GetUniq()} {this.GetFK()}".Trim();
        }
    }
}
