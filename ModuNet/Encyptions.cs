using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
namespace ModuNet
{
    internal class Encyptions
    {
        public Encyptions() { 

            string originalText = "Hello, World!";
            using (Aes aes = Aes.Create())
            {
                byte[] encrypted = EncryptStringToBytes_AES(originalText, aes.Key, aes.IV);
                Console.WriteLine(encrypted[0]);
                Console.WriteLine("Decrpt:");
                string text = DecryptStringFromBytes_AES(encrypted, aes.Key, aes.IV);
                Console.WriteLine(text);
            }
        }
        byte[] EncryptStringToBytes_AES(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                return null;
            if (Key == null || Key.Length <= 0) return null;
            if (IV == null || IV.Length <= 0) return null;
            byte[] encrpyted;


            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform ecryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypted = new CryptoStream(msEncrypt, ecryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypted))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    encrpyted = msEncrypt.ToArray();
                }
            }
            return encrpyted;
        }
        string DecryptStringFromBytes_AES(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0) { return null; }
            if (Key == null || Key.Length <= 0) return null;
            if (IV == null || IV.Length <= 0)
                { return null; }
            string plainText = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plainText;
        }
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
