using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Incas.Core.Classes
{
    public static class Cryptographer
    {
        private static string defaultKey = "b14ca5898a4e4133bbce2ea2315a1916"; // b14ca5898a4e4133bbce2ea2315a1916
        private static Dictionary<string, string> hexReplacements = new()
        {
            {"1", "I" },
            {"2", "Q" },
            {"3", "W" },
            {"4", "Z" },
            {"5", "S" },
            {"6", "Y" },
            {"7", "P" },
            {"8", "U" },
            {"9", "N" },
        };
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
        internal static string ToHex(string source)
        {
            byte[] resultBytes = Encoding.UTF8.GetBytes(source);
            Array.Reverse(resultBytes);
            return Convert.ToHexString(resultBytes);
        }
        internal static string FromHex(string text)
        {
            byte[] b = Convert.FromHexString(text);
            Array.Reverse(b);
            return System.Text.Encoding.UTF8.GetString(b);
        }
        internal static string ToDifficultHex(string source)
        {
            byte[] resultBytes = Encoding.UTF8.GetBytes(source);
            Array.Reverse(resultBytes);
            string result = Convert.ToHexString(resultBytes);
            foreach (KeyValuePair<string, string> pair in hexReplacements)
            {
                result = result.Replace(pair.Key, pair.Value);
            }
            return result;
        }
        internal static string FromDifficultHex(string text)
        {
            foreach (KeyValuePair<string, string> pair in hexReplacements)
            {
                text = text.Replace(pair.Value, pair.Key);
            }
            byte[] b = Convert.FromHexString(text);
            Array.Reverse(b);
            string result = System.Text.Encoding.UTF8.GetString(b);
            return result;
        }
    }
}
