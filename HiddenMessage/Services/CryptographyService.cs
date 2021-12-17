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

                string keyHash = MD5Hash(key);
                byte[] byteHash = Convert.FromBase64String(keyHash);
                byte[] byteBuff = Encoding.UTF8.GetBytes(source);

                desCryptoProvider.Key = byteHash;
                desCryptoProvider.Mode = CipherMode.ECB;

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

                string keyHash = MD5Hash(key);
                byte[] byteHash = Convert.FromBase64String(keyHash);
                byte[] byteBuff = Convert.FromBase64String(encodedText);

                desCryptoProvider.Key = byteHash;
                desCryptoProvider.Mode = CipherMode.ECB;

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
