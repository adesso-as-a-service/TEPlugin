using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace TEPlugin
{
    sealed class EncryptedData
    {
        private byte[] encryptedData;
        public byte[] encryptedKey
        {
            get;
            internal set;
        }

        private EncryptedData()
        {

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

        public byte[] ToBinary()
        {
            List<byte> retVal = new List<byte> { };
            byte[] help, len;

            // add encryptedData
            retVal.Add(0x01);
            help = TEKeyFile.getSubarry(encryptedData, 0, encryptedData.Length);
            len = BitConverter.GetBytes(help.Length);
            retVal.AddRange(len);
            retVal.AddRange(help);

            // add encryptedKey

            retVal.Add(0x02);
            help = TEKeyFile.getSubarry(encryptedKey, 0, encryptedKey.Length);
            len = BitConverter.GetBytes(help.Length);
            retVal.AddRange(len);
            retVal.AddRange(help);

            return retVal.ToArray();
        }

        public static EncryptedData FromBinary(byte[] array)
        {
            EncryptedData ec;
            byte[]  ed ,ek;
            int len;
            ed = ek = new byte[0];
            for (int i = 0; i < array.Length; i++)
            {
                switch (array[i])
                {
                    case 0x01:
                        len = BitConverter.ToInt32(array, i + 1);
                        i = i + 4;
                        ed = TEKeyFile.getSubarry(array, i + 1, i + 1 + len);
                        i = i + len;
                        break;
                    case 0x02:
                        len = BitConverter.ToInt32(array, i + 1);
                        i = i + 4;
                        ek = TEKeyFile.getSubarry(array, i + 1, i + 1 + len);
                        i = i + len;
                        break;
                    default:
                        throw new FormatException("EncryptedData Binary Format is incorrect");
                }
            }

            ec = new EncryptedData();
            ec.encryptedKey = ek;
            ec.encryptedData = ed;
            return ec;
        }

    }
}
