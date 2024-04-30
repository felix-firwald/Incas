using Common;

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
            this.tableName = "Profiles";
        }

    }
}
