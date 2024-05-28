namespace Incas.CustomDatabases.Models
{
    public struct SDatabase
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string sectors { get; set; }
        public Database AsModel()
        {
            Database db = new()
            {
                id = this.id,
                name = this.name,
                sectors = this.sectors,
                path = this.path
            };
            return db;
        }
    }
}
