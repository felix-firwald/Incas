using Common;

namespace Models
{
    public class Subtask : Model
    {
        public int id { get; set; }
        public int task { get; set; }
        public string text { get; set; }
        public bool done { get; set; }
        public int performer { get; set; }

        public Subtask() 
        {
            tableName = "Subtasks";
        }
    }
}
