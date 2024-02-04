using Common;
using Incubator_2.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    public class Process : Model
    {
        public string identifier { get; set; }
        public string content { get; set; }
        public bool received { get; set; }
        public Process()
        {
            tableName = "Processes";
        }
        public void Send(string port)
        {
            StartCommandToOtherPort(port)
                .Insert(new()
                {
                    { nameof(identifier), identifier },
                    { nameof(content), content },
                    { nameof(received), "0" }
                })
                .ExecuteVoid();
        }
        public List<string> GetNewProcesses()
        {
            DataTable dt = StartCommandToMyPort()
                .Select()
                .WhereEqual(nameof(received), "0")
                .Execute();           
            List<string> result = new();
            if (dt.Rows.Count > 0)
            {
                SetReceived();
                foreach (DataRow dr in dt.Rows)
                {
                    result.Add(dr["content"].ToString());
                }
            }
            return result;
        }
        private void SetReceived()
        {
            StartCommandToMyPort()
                .Update(nameof(received), "1")
                .ExecuteVoid();
        }
    }
}
