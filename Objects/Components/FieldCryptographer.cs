using Incas.Core.Classes;
using System;

namespace Incas.Objects.Components
{
    internal class FieldCryptographer
    {
        internal static string EncryptFieldValue(Guid classId, Guid columnId, Guid objectId, string value)
        {
            string key = Cryptographer.GenerateKey(FieldCryptographer.GenerateKey(ref classId, ref columnId, ref objectId));
            return Cryptographer.EncryptString(key, value);
        }
        internal static string DecryptFieldValue(Guid classId, Guid columnId, Guid objectId, string value)
        {
            string key = Cryptographer.GenerateKey(FieldCryptographer.GenerateKey(ref classId, ref columnId, ref objectId));
            return Cryptographer.DecryptString(key, value);
        }
        private static string GenerateKey(ref Guid classId, ref Guid columnId, ref Guid objectId)
        {
            string result = "";
            string[] cid = columnId.ToString().Split('-'); // always 4 parts
            string[] oid = objectId.ToString().Split('-'); // always 4 parts
            string temp = classId.ToString();
            int firstSwitcher = Char.IsDigit(temp[0]) ? 1 : 0;
            int secondSwitcher = Char.IsDigit(temp[1]) ? 1 : 0;
            switch (firstSwitcher)
            {
                case 0:
                    switch (secondSwitcher)
                    {
                        case 0:
                            result = $"{cid[0]}{oid[1]}{cid[2]}{oid[3]}";
                            break;
                        case 1:
                        default:
                            result = $"{oid[0]}{cid[1]}{oid[2]}{cid[3]}";
                            break;
                    }
                    break;
                case 1:
                default:
                    switch (secondSwitcher)
                    {
                        case 0:
                            result = $"{cid[0]}{cid[1]}{oid[2]}{oid[3]}";
                            break;
                        case 1:
                        default:
                            result = $"{oid[0]}{oid[1]}{cid[2]}{cid[3]}";
                            break;
                    }
                    break;
            }
            return result;
        }
    }
}
