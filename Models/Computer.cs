using Common;
using System.Collections.Generic;
using System.Data;

namespace Models
{
    public class Computer : Model
    {
        public string authId { get; set; }
        public string name { get; set; }
        public bool blocked { get; set; }

        public Computer() 
        {
            tableName = "Computers";
            definition = "authId STRING UNIQUE ON CONFLICT ROLLBACK NOT NULL,\n" +
                "name STRING NOT NULL,\n" +
                "blocked BOOLEAN NOT NULL DEFAULT (0)";
        }
        public List<Computer> GetAllComputers()
        {
            List<Computer> results = new List<Computer>();
            DataTable dt = GetAll();
            foreach (DataRow dr in dt.Rows)
            {
                Computer mc = new Computer();
                mc.Serialize(dr);
                results.Add(mc);
            }
            return results;
        }
        public bool IsComputerExists(string id)
        {
            if (ParseCount(StartCommand().Count().WhereEqual("authId", id).Execute()) == 0)
            {
                return false;
            }
            return true;
        }
        public void AddComputer()
        {
            if (!IsComputerExists(authId)) 
            {
                StartCommand()
                    .Insert(new Dictionary<string, string>
                    {
                        { "authId", $"'{authId}'" },
                        { "name", $"'{name}'" },
                        { "bloced", blocked.ToString() },
                    })
                    .ExecuteVoid();
            }
        }
        public void UpdateComputer()
        {
            StartCommand()
                .Update("name", name)
                .Update("blocked", blocked.ToString())
                .WhereEqual("authId", authId).ExecuteVoid();
        }
    }

}
