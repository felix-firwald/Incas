using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    public class Message : Model
    {
        private string path;
        public int id { get; set; }
        public DateTime date { get; set; }
        public int sender { get; set; }
        public int recipient { get; set; }
        public string content { get; set; }
        public Message()
        {
            tableName = "Messages";
        }

        public List<Message> GetMessages()
        {
            DataTable dt = StartCommandToChat(this.path)
                .Select()
                .Execute();
            List<Message> messages = new();
            foreach (DataRow row in dt.Rows)
            {
                Message m = new();
                m.Serialize(row);
                messages.Add(m);
            }
            return messages;
        }
        public void SendMessage()
        {

        }

    }
}
