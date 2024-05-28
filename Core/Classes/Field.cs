namespace Incas.Core.Classes
{
    public struct Field
    {
        public string Name;
        public string Type;
        public bool NotNull;
        public string FKTable;
        public string FKField;
        public OnDeleteUpdate Constraint;
        public Field(string name, string type, bool notnull = false, string fkt = null, string fkf = "id", OnDeleteUpdate constraint = OnDeleteUpdate.CASCADE)
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
        public override string ToString()
        {
            return $"{this.Name} {this.Type}{this.GetFK()}{this.GetNotNull()}";
        }
    }
}
