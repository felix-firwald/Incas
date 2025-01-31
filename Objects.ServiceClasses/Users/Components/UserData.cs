using Incas.Objects.Engine;
using Newtonsoft.Json;

namespace Incas.Objects.ServiceClasses.Users.Components
{
    public class UserData : IObjectData
    {
        [JsonProperty("u_pwd")]
        public string Password { get; set; }

        /// <summary>
        /// Means user (including super admin) can`t remove or edit this user
        /// <para>For the first user INCAS sets it to True, otherwise False</para>
        /// </summary>
        [JsonProperty("u_i")]
        public bool Indestructible { get; set; }
    }
}
