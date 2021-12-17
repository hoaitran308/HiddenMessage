using System;
using System.Text;
using System.Security.Cryptography;

namespace HiddenMessage.Services
{
    public class CryptographyService
    {
        public string MD5Hash(string source)
        {
            using MD5 hashMD5Provider = MD5.Create();
            byte[] byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(source));
            return Convert.ToBase64String(byteHash);
        }

        public string TripleDESEncrypt(string source, string key)
        {
            try
            {
                using TripleDES desCryptoProvider = TripleDES.Create();

                byte[] byteHash = Convert.FromBase64String(key);
                desCryptoProvider.Key = byteHash;
                desCryptoProvider.Mode = CipherMode.ECB;
                byte[] byteBuff = Encoding.UTF8.GetBytes(source);

                string encoded = Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                return encoded;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string TripleDESDecrypt(string encodedText, string key)
        {
            try
            {
                using TripleDES desCryptoProvider = TripleDES.Create();
                using MD5 hashMD5Provider = MD5.Create();

                byte[] byteHash = Convert.FromBase64String(key);
                desCryptoProvider.Key = byteHash;
                desCryptoProvider.Mode = CipherMode.ECB;
                byte[] byteBuff = Convert.FromBase64String(encodedText);

                string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                return plaintext;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
