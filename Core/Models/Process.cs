using Common;
using System.Collections.Generic;
using System.Data;

namespace Incas.Core.Models
{
    public class Process : Model
    {
        public string identifier { get; set; }
        public string content { get; set; }
        public bool received { get; set; }
        public Process()
        {
            this.tableName = "Processes";
        }
        public void Send(string port)
        {
            this.StartCommandToOtherPort(port)
                .Insert(new()
                {
                    { nameof(this.identifier), this.identifier },
                    { nameof(this.content), this.content },
                    { nameof(this.received), "0" }
                })
                .ExecuteVoid();
        }
        public List<string> GetNewProcesses()
        {
            DataTable dt = this.StartCommandToMyPort()
                .Select()
                .WhereEqual(nameof(this.received), "0")
                .Execute();
            List<string> result = [];
            if (dt.Rows.Count > 0)
            {
                this.SetReceived();
                foreach (DataRow dr in dt.Rows)
                {
                    result.Add(dr["content"].ToString());
                }
            }
            return result;
        }
        private void SetReceived()
        {
            this.StartCommandToMyPort()
                .Update(nameof(this.received), "1")
                .ExecuteVoid();
        }
    }
}
