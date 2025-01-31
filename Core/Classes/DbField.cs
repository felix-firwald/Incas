namespace Incas.Core.Classes
{
    public struct DbField
    {
        public string Name;
        public string Type = "TEXT";
        public bool NotNull;
        public bool Unique;
        public string FKTable;
        public string FKField;
        public OnDeleteUpdate Constraint;
        public DbField(string name, string type, bool notnull = false, string fkt = null, string fkf = "id", OnDeleteUpdate constraint = OnDeleteUpdate.CASCADE)
        {
            this.Name = name;
            this.Type = type;
            this.NotNull = notnull;
            if (fkt != null)
            {
                this.FKTable = fkt;
                this.FKField = fkf;
                this.Constraint = constraint;
            }
            else
            {
                this.FKTable = "";
                this.FKField = "";
                this.Constraint = OnDeleteUpdate.CASCADE;
            }
        }
        private string GetNotNull()
        {
            return this.NotNull ? " NOT NULL" : "";
        }
        private string GetFK()
        {
            return !string.IsNullOrEmpty(this.FKTable) ? $" REFERENCES {this.FKTable} ({this.FKField})" : "";
        }
        private string GetUnique()
        {
            return this.Unique ? " UNIQUE" : "";
        }
        public override string ToString()
        {
            return $"[{this.Name}] {this.Type} {this.GetUnique()} {this.GetFK()}{this.GetNotNull()}";
        }
    }
}
