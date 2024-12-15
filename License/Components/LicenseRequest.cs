using Incas.Core.Classes;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Incas.License
{
    class LicenseRequest
    {
        internal const string ColonReplacer = "`3µ65D#";
        internal const string CommaReplacer = "#8µœD`";
        internal const string MarkReplacer = "qΘWΘ";
        internal const string Extension = ".relinc";
        [JsonProperty("rid")]
        public Guid RequestID { get; set; }
        [JsonProperty("rd")]
        public DateTime RequestDate { get; set; }
        [JsonProperty("pn")]
        public string PersonName { get; set; }
        [JsonProperty("org")]
        public string Organization { get; set; }
        [JsonProperty("eml")]
        public string Email { get; set; }
        [JsonProperty("mn")]
        public string MachineName { get; set; }
        [JsonProperty("udn")]
        public string UserDomainName { get; set; }
        [JsonProperty("ed")]
        public DateTime ExpirationDate { get; set; }
        [JsonProperty("lt")]
        public LicenseType LicenseType { get; set; }
        private void EncryptFields()
        {
            string key = Cryptographer.GenerateKey(this.RequestID.ToString());
            this.Email = Cryptographer.EncryptString(key, this.Email);
            this.MachineName = Cryptographer.EncryptString(key, this.MachineName);
            this.UserDomainName = Cryptographer.EncryptString(key, this.UserDomainName);
        }
        private void DecryptFields()
        {
            string key = Cryptographer.GenerateKey(this.RequestID.ToString());
            this.Email = Cryptographer.DecryptString(key, this.Email);
            this.MachineName = Cryptographer.DecryptString(key, this.MachineName);
            this.UserDomainName = Cryptographer.DecryptString(key, this.UserDomainName);
        }
        internal void WriteRequest(string path)
        {
            this.EncryptFields();
            string result = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            result = result.Replace(",", CommaReplacer).Replace(":", ColonReplacer).Replace("\"", MarkReplacer);
            byte[] resultBytes = Encoding.UTF8.GetBytes(result);
            Array.Reverse(resultBytes);
            if (!path.EndsWith(Extension))
            {
                path += Extension;
            }
            result = Convert.ToHexString(resultBytes);
            File.WriteAllText(path, result);
        }
        internal static LicenseRequest ReadRequest(string path)
        {
            string text = File.ReadAllText(path);
            byte[] b = Convert.FromHexString(text);
            Array.Reverse(b);
            string result = System.Text.Encoding.UTF8.GetString(b);
            result = result.Replace(CommaReplacer, ",").Replace(ColonReplacer, ":").Replace(MarkReplacer, "\"");
            LicenseRequest license = Newtonsoft.Json.JsonConvert.DeserializeObject<LicenseRequest>(result);
            if (license == null)
            {
                return new();
            }
            license.DecryptFields();
            return license;
        }
        internal static LicenseRequest Get()
        {
            LicenseRequest lr = new();
            lr.MachineName = Environment.MachineName;
            lr.UserDomainName = Environment.UserDomainName;
            return lr;
        }
    }
}
