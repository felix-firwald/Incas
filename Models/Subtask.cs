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
        }
    }
}
