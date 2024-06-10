using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IncasEngine
{
    public static class Cryptographer
    {
        private static string defaultKey = "b14ca5898a4e4133bbce2ea2315a1916"; // b14ca5898a4e4133bbce2ea2315a1916

        public static string GenerateKey(string input)
        {
            string result = string.Join("е", input.ToCharArray().Reverse()) + "b";
            byte[] bytes = Encoding.UTF8.GetBytes(result);
            result = Convert.ToHexString(bytes);
            if (result.Length < 32)
            {
                int multiplier = 32 - result.Length;
                result += Enumerable.Repeat("b", multiplier);
            }

            return result.Substring(0, 32);
        }

        public static string EncryptString(string input)
        {
            return EncryptString(defaultKey, input);
        }
        public static string DecryptString(string input)
        {
            return DecryptString(defaultKey, input);
        }
        public static string EncryptString(string key, string input)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using MemoryStream memoryStream = new MemoryStream();
                using CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
                using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                {
                    streamWriter.Write(input);
                }

                array = memoryStream.ToArray();
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string input)
        {
            try
            {
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(input);

                using Aes aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using MemoryStream memoryStream = new MemoryStream(buffer);
                using CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
                using StreamReader streamReader = new StreamReader((Stream)cryptoStream);
                return streamReader.ReadToEnd();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
