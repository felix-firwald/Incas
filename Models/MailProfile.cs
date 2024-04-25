using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incubator_2.Models
{
    public class MailProfile : Model
    {
        public int id;
        public string name;
        public string email;
        public string password;
        public string host;
        public int port;
        public MailProfile()
        {
            tableName = "Profiles";
        }

    }
}
