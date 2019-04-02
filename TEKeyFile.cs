using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using shamirsSecretSharing;
using System.IO;
using SCCrypto;
using System.Security.Cryptography;

namespace TEPlugin
{
    [Serializable]
    public class TEKeyFile
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public PublicKey key;

        public List<Tuple<string, byte[], byte[]>> encryptedShares;





        public TEKeyFile(PublicKey key)
        {
            this.key = key;
            encryptedShares = new List<Tuple<string, byte[], byte[]>>();
        }


        //----------------------
        public static byte[] OpenKeyfile(string filename, SmartCard smartCard)
        {
            TEUserIO io =(TEUserIO) smartCard.settings.userIO;
            io.init();
            TEKeyFile keyFile;
            if (filename.Length == 0)
                throw new FileNotFoundException("Keyfile : Filename not set.");
            try
            {
                using (Stream stream = File.Open(filename, FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    keyFile =  (TEKeyFile) binaryFormatter.Deserialize(stream);
                }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Unable to load keyfile.");
            }

            // DECRYPTION
            // Set io
            int dec = 0;
            int remaining;

            Share share;
            Tuple<int, byte[], byte[]> retVal;

            Share[] shares = new Share[keyFile.key.N];

            Dictionary<byte[], EncryptedData> encData = new Dictionary<byte[], EncryptedData>(new ArrayEqualityCompare());

            EncryptedData temp;

            List<Tuple<string, byte[], byte[]>> dataToDecrypt = new List<Tuple<string, byte[], byte[]>>();

            byte[] key, encKey;

            for (int i = 0; i < keyFile.encryptedShares.Count; i++)
            {
                temp = (EncryptedData)ByteArrayToObject(keyFile.encryptedShares[i].Item3);
                encData.Add(temp.encryptedKey, temp);
                dataToDecrypt.Add(new Tuple<string, byte[], byte[]>(keyFile.encryptedShares[i].Item1, keyFile.encryptedShares[i].Item2, temp.encryptedKey));
            }

            // Decrypt with scc
            int remainingOld = keyFile.encryptedShares.Count;
            Decryption decrypt = new Decryption(smartCard, dataToDecrypt);
            while (dec < keyFile.key.N)
            {
                retVal = decrypt.Do();
                remaining = retVal.Item1;
                if (remaining < 0) continue;
                if (remainingOld > remaining)
                {
                    remainingOld = remaining;
                    encKey = retVal.Item2;
                    key = retVal.Item3;
                    shares[dec] = (Share)ByteArrayToObject(encData[encKey].Decrypt(key));
                    dec++;
                }
                io.outputText(String.Format("Remaining: {0}", keyFile.key.N - dec));
            }
            io.close();
            return SssEngine.Decrypt(keyFile.key, shares);
        }


        //----------------------
        public static byte[] CreateKeyfile(string filename, SmartCard smartCard)
        {
            uint n, m;
            byte[] secret;
            TEUserIO io = (TEUserIO) smartCard.settings.userIO;
            io.init();
            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                byte[] plain, cipher, pubkeyhash;
                EncryptedData encData;
                bool allowDoubleOwners, allowDoubleKeys;
                // ConsoleIO io = new ConsoleIO();
                // read n and m, doubleKeys, doubleOwners
                var settings = io.getSettings();
                n = (uint) settings.Item3;
                m = (uint) settings.Item4;
                allowDoubleKeys = settings.Item1;
                allowDoubleOwners = settings.Item2;
                SssEngine engine = new SssEngine(n, m, 2048);

                secret = getRandomBytes(256 / 8);
                // shamir encrypt
                Tuple<PublicKey, Share[]> tuple = engine.Encrypt(secret);
                TEKeyFile store = new TEKeyFile(tuple.Item1);
                // encrypt with SCC
                LinkedList<byte[]> dataToEncrypt = new LinkedList<byte[]>();
                for (int i = 0; i < m; i++)
                {
                    dataToEncrypt.AddLast(getRandomBytes(256/8));
                }
                Encryption enc = new Encryption(smartCard, dataToEncrypt,allowDoubleOwners,allowDoubleKeys);

                int remaining = dataToEncrypt.Count;
                Tuple<int, byte[], byte[], byte[], string> retVal;
                Tuple<string, byte[], byte[]> storeVal;
                while (remaining > 0)
                {
                    retVal = enc.Do();
                    remaining = retVal.Item1;
                    plain = retVal.Item2;
                    cipher = retVal.Item3;
                    pubkeyhash = retVal.Item4;
                    encData = new EncryptedData(ObjectToByteArray(tuple.Item2[remaining]), plain, cipher);
                    storeVal = new Tuple<string, byte[], byte[]>(retVal.Item5, pubkeyhash, ObjectToByteArray(encData));
                    store.encryptedShares.Add(storeVal);
                    io.outputText(String.Format("Remaining: {0}", remaining));
                }
            
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, store);
            }
            io.close();
            return secret;
        }

        private static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        // Convert a byte array to an Object
        private static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        private static byte[] getRandomBytes(uint len)
        {
            byte[] retVal = new byte[len];
            rng.GetBytes(retVal);
            return retVal;
        }
    }

}
