using Common;

namespace Models
{
    public class Subtask : Model
    {
        public int id { get; set; }
        public int task { get; set; }
        public string text { get; set; }
        public bool passed { get; set; }
        public string performer { get; set; }

        public Subtask() 
        {
            tableName = "Subtasks";
            definition = "id INTEGER PRIMARY KEY AUTOINCREMENT,\n" +
                "task INT REFERENCES Tasks (id) ON DELETE CASCADE ON UPDATE CASCADE NOT NULL,\n" +
                "text TEXT NOT NULL,\npassed BOOLEAN NOT NULL DEFAULT (0),\n" +
                "performer STRING REFERENCES Users (username) NOT NULL,\n" +
                "passed BOOLEAN NOT NULL DEFAULT (0)";
        }
    }
}
