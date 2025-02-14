using Incas.Core.Classes;
using IncasEngine.Core;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Incas.License
{
    internal class License
    {
        private const string ColonReplacer = "`3µ65D#";
        private const string CommaReplacer = "#8µœD`";
        private const string MarkReplacer = "qΘWΘ";
        internal const string Extension = ".licensinc";
        [JsonProperty("lid")]
        public Guid LicenseID { get; set; }
        [JsonProperty("lac")]
        public string LicenseAuthorComputer { get; set; }
        [JsonProperty("ld")]
        public DateTime LicenseDate { get; set; }
        [JsonProperty("ed")]
        public DateTime ExpirationDate { get; set; }
        [JsonProperty("et")]
        public string ExpirationText { get; set; }
        [JsonProperty("wd")]
        public DateTime WarningDate { get; set; }
        [JsonProperty("wt")]
        public string WarningText { get; set; }
        [JsonProperty("ln")]
        public string LicenseName { get; set; }
        [JsonProperty("lns")]
        public string LicenseNamespace { get; set; }
        [JsonProperty("eml")]
        public string Email { get; set; }
        [JsonProperty("mn")]
        public string MachineName { get; set; }
        [JsonProperty("udn")]
        public string UserDomainName { get; set; }
        [JsonProperty("umi")]
        public Guid UniqueMachineId { get; set; }
        [JsonProperty("lt")]
        public LicenseType LicenseType { get; set; }

        [JsonProperty("com")]
        public string Commentary { get; set; }

        // signature verification version, 0 by default
        public int SVV { get; set; }
        // verification signatures
        public int VSI { get; set; }
        public int VSII { get; set; }
        public int VSIII { get; set; }
        public int VSIV { get; set; }
        public long VSV { get; set; }
        private int CalculateVSI()
        {
            this.MachineName ??= "";
            return this.ExpirationDate.Year - (this.LicenseDate.Month + this.LicenseDate.Day) + this.MachineName.Length;
        }
        private int CalculateVSII()
        {
            this.Email ??= "";
            return (this.LicenseDate.Month * this.WarningDate.Month) + this.Email.Length;
        }
        private int CalculateVSIII()
        {
            this.UserDomainName ??= "";
            return (this.LicenseDate.Day * this.ExpirationDate.Day) + this.UserDomainName.Length;
        }
        private int CalculateVSIV(int licType)
        {
            this.LicenseName ??= "";
            this.LicenseNamespace ??= "";
            return this.LicenseName.Length + this.LicenseNamespace.Length + (licType * licType);
        }
        private long CalculateVSV(int licType)
        {
            DateTimeOffset startDate = new(this.LicenseDate);
            DateTimeOffset endDate = new(this.ExpirationDate);
            return endDate.ToUnixTimeSeconds() - startDate.ToUnixTimeSeconds() - licType;
        }

        internal void WriteVerificationSignatures()
        {
            int licType = (int)this.LicenseType;
            this.VSI = this.CalculateVSI();
            this.VSII = this.CalculateVSII();
            this.VSIII = this.CalculateVSIII();
            this.VSIV = this.CalculateVSIV(licType);
            this.VSV = this.CalculateVSV(licType);
        }
        internal bool CheckVerificationSignatures()
        {
            int licType = (int)this.LicenseType;
            if (this.VSI == this.CalculateVSI() 
                && this.VSII == this.CalculateVSII() 
                && this.VSIII == this.CalculateVSIII()
                && this.VSIV == this.CalculateVSIV(licType)
                && this.VSV == this.CalculateVSV(licType))
            {
                return true;
            }
            return false;
        }
        private void EncryptFields()
        {
            string key = Cryptographer.GenerateKey(this.LicenseID.ToString());
            this.LicenseAuthorComputer = Cryptographer.EncryptString(key, this.LicenseAuthorComputer);
            this.Email = Cryptographer.EncryptString(key, this.Email);
            this.MachineName = Cryptographer.EncryptString(key, this.MachineName);
            this.UserDomainName = Cryptographer.EncryptString(key, this.UserDomainName);
        }
        private void DecryptFields()
        {
            string key = Cryptographer.GenerateKey(this.LicenseID.ToString());
            this.LicenseAuthorComputer = Cryptographer.DecryptString(key, this.LicenseAuthorComputer);
            this.Email = Cryptographer.DecryptString(key, this.Email);
            this.MachineName = Cryptographer.DecryptString(key, this.MachineName);
            this.UserDomainName = Cryptographer.DecryptString(key, this.UserDomainName);
        }
        internal bool IsActual()
        {
            return DateTime.Now < this.ExpirationDate;
        }
        internal bool IsItForMe()
        {
            return (Environment.MachineName == this.MachineName && Environment.UserDomainName == this.UserDomainName && EngineGlobals.GetUMI() == this.UniqueMachineId.ToString());
        }
        internal static License ReadLicense(string path)
        {
            try
            {
                string text = File.ReadAllText(path);
                string result = Cryptographer.FromDifficultHex(text);
                result = result.Replace(CommaReplacer, ",").Replace(ColonReplacer, ":").Replace(MarkReplacer, "\"");
                License license = Newtonsoft.Json.JsonConvert.DeserializeObject<License>(result);
                if (license == null)
                {
                    return new();
                }
                license.DecryptFields();
                return license;
            }
            catch
            {
                return new();
            }
        }
    }
}
