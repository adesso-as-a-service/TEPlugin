using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace TEPlugin
{
    [Serializable]
    sealed class EncryptedData
    {
        private byte[] encryptedData;
        public byte[] encryptedKey
        {
            get;
            internal set;
        }


        public EncryptedData(byte[] Data, byte[] Key, byte[] EncryptedKey)
        {
            this.encryptedKey = EncryptedKey;
            using (MemoryStream ms = new MemoryStream())
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();
                aes.Key = Key;
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(Data, 0, Data.Length);
                }
                byte[] encryptedContent = ms.ToArray();

                //Create new byte array that should contain both unencrypted iv and encrypted data
                encryptedData = new byte[aes.IV.Length + encryptedContent.Length];

                //copy our 2 array into one
                System.Buffer.BlockCopy(aes.IV, 0, encryptedData, 0, aes.IV.Length);
                System.Buffer.BlockCopy(encryptedContent, 0, encryptedData, aes.IV.Length, encryptedContent.Length);
                Array.Clear(Key,0,Key.Length);
            }
        }

        public byte[] Decrypt(byte[] key)
        {
            byte[] iv = new byte[128/8]; //initial vector is 16 bytes
            byte[] encryptedContent = new byte[encryptedData.Length - 128/8]; //the rest should be encryptedcontent

            //Copy data to byte array
            System.Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
            System.Buffer.BlockCopy(encryptedData, iv.Length, encryptedContent, 0, encryptedContent.Length);

            using (MemoryStream ms = new MemoryStream())
            {
                using (AesManaged cryptor = new AesManaged())
                {
                    cryptor.Mode = CipherMode.CBC;
                    cryptor.Padding = PaddingMode.PKCS7;
                    cryptor.KeySize = 256;
                    cryptor.BlockSize = 128;

                    using (CryptoStream cs = new CryptoStream(ms, cryptor.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedContent, 0, encryptedContent.Length);

                    }
                    Array.Clear(key,0,key.Length);
                    return ms.ToArray();
                }
            }


        }
    }
}
