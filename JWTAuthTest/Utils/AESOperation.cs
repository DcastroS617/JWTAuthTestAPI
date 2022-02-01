using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuthTest.Utils
{
    public class AESOperation
    {
        public AESOperation()
        {
        }

        public string Encrypt(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using(Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using(MemoryStream ms = new MemoryStream())
                {
                    using(CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using(StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        array = ms.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }
        public string Decrypt(string key, string cypherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cypherText);
            using(Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using(MemoryStream ms = new MemoryStream(buffer))
                {
                    using(CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using(StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
