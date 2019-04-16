using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using shamirsSecretSharing;
using System.IO;
using SCCrypto;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace TEPlugin
{
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
                byte[] fileBin = File.ReadAllBytes(filename);    
                keyFile =  TEKeyFile.FromBinary(getSubarry(fileBin,0,fileBin.Length));
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Unable to load keyfile.");
            }

            // DECRYPTION
            // Set io
            int dec = 0;
            int remaining;

            Tuple<int, byte[], byte[]> retVal;

            Share[] shares = new Share[keyFile.key.N];

            Dictionary<byte[], EncryptedData> encData = new Dictionary<byte[], EncryptedData>(new ArrayEqualityCompare());

            EncryptedData temp;

            List<Tuple<string, byte[], byte[]>> dataToDecrypt = new List<Tuple<string, byte[], byte[]>>();

            byte[] key, encKey;

            for (int i = 0; i < keyFile.encryptedShares.Count; i++)
            {
                temp = EncryptedData.FromBinary(keyFile.encryptedShares[i].Item3);
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
                    shares[dec] = Share.FromBinary(encData[encKey].Decrypt(key));
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
                    encData = new EncryptedData(tuple.Item2[remaining].ToBinary(), plain, cipher);
                    storeVal = new Tuple<string, byte[], byte[]>(retVal.Item5, pubkeyhash,encData.ToBinary());
                    store.encryptedShares.Add(storeVal);
                    io.outputText(String.Format("Remaining: {0}", remaining));
                }

                byte[] binForm = store.ToBinary();
                stream.Write(binForm, 0, binForm.Length);
            }
            io.close();
            return secret;
        }


        public byte [] ToBinary()
        {
            List<byte> retVal = new List<byte> { };
            byte[] help, len;

            // add pubkey
            retVal.Add(0x01);
            help = key.ToBinary();
            len = BitConverter.GetBytes(help.Length);
            retVal.AddRange(len);
            retVal.AddRange(help);

            // add encrypted Shares

            retVal.Add(0x02);
            help = encryptedSharesToBinary();
            len = BitConverter.GetBytes(help.Length);
            retVal.AddRange(len);
            retVal.AddRange(help);

            return retVal.ToArray();
        }

        private byte[] encryptedSharesToBinary()
        {
            List<byte> retVal = new List<byte> { };
            byte[] help, len;
            Tuple<string, byte[], byte[]> element;

            for (int i = 0; i < encryptedShares.Count; i++)
            {
                element = encryptedShares[i];

                //add string
                retVal.Add(0x01);

                help = Encoding.UTF8.GetBytes(element.Item1);
                len = BitConverter.GetBytes(help.Length);

                retVal.AddRange(len);
                retVal.AddRange(help);

                // add byte1
                retVal.Add(0x02);

                help = new byte[element.Item2.Length];
                Array.Copy(element.Item2, help, help.Length);
                len = BitConverter.GetBytes(help.Length);

                retVal.AddRange(len);
                retVal.AddRange(help);

                // add byte2
                retVal.Add(0x03);

                help = new byte[element.Item3.Length];
                Array.Copy(element.Item3, help, help.Length);
                len = BitConverter.GetBytes(help.Length);

                retVal.AddRange(len);
                retVal.AddRange(help);
            }

            return retVal.ToArray();
        }


        public static TEKeyFile FromBinary(byte[] array)
        {
            TEKeyFile kf;
            PublicKey pub = new PublicKey(2, 2, 1024);
            List<Tuple<string, byte[], byte[]>> encryptedShares = new List<Tuple<string, byte[], byte[]>> { };
            byte[] help;
            int len;
            for (int i = 0; i < array.Length; i++)
            {
                switch (array[i])
                {
                    case 0x01:
                        len = BitConverter.ToInt32(array, i + 1);
                        i = i + 4;
                        help = getSubarry(array, i + 1, i + 1 + len);
                        pub = PublicKey.ReadFromBinary(help);
                        i = i + len;
                        break;
                    case 0x02:
                        len = BitConverter.ToInt32(array, i + 1);
                        i = i + 4;
                        help = getSubarry(array, i + 1, i + 1 + len);
                        encryptedShares = esFromBinary(help);
                        i = i + len;
                        break;
                    default:
                        throw new FormatException("TEKeyFile Binary Format is incorrect");
                }
            }

            kf = new TEKeyFile(pub);
            kf.encryptedShares = encryptedShares;
            return kf;
        }


        private static byte[] getRandomBytes(uint len)
        {
            byte[] retVal = new byte[len];
            rng.GetBytes(retVal);
            return retVal;
        }

        public static byte[] getSubarry(byte[] array, int start, int stop)
        {
            if (stop > array.Length) throw new ArgumentException("Stop is out of bounds");
            if (start < 0) throw new ArgumentException("Start is out of bounds");
            if (start >= stop) throw new ArgumentException("Start can't be bigger or as big as stop");

            byte[] retVal = new byte[stop - start];
            Array.Copy(array, start, retVal, 0, stop - start);
            return retVal;
        }

        private static List<Tuple<string, byte[], byte[]>> esFromBinary(byte[] array)
        {
            List<Tuple<string, byte[], byte[]>> encryptedShares = new List<Tuple<string, byte[], byte[]>> { };
            
            int len;
            byte[] help, byte1, byte2;
            string str;
            for (int i = 0; i < array.Length; i++)
            {
                if ( array[i] != 0x01)
                throw new FormatException("Encrypted Share Binary Format is incorrect" + i.ToString() +" " + array[i].ToString());


                len = BitConverter.ToInt32(array, i + 1);
                i = i + 4;

                help = getSubarry(array, i + 1, i + 1 + len);
                i = i + len;

                str = Encoding.UTF8.GetString(help);
                i++;

                if (array[i] != 0x02)
                    throw new FormatException("Encrypted Share Binary Format is incorrect2");


                len = BitConverter.ToInt32(array, i + 1);
                i = i + 4;

                byte1 = getSubarry(array, i + 1, i + 1 + len);
                i = i + len;
                i++;

                if (array[i] != 0x03)
                    throw new FormatException("Encrypted Share Binary Format is incorrect3");


                len = BitConverter.ToInt32(array, i + 1);
                i = i + 4;

                byte2 = getSubarry(array, i + 1, i + 1 + len);
                i = i + len;

                encryptedShares.Add(new Tuple<string, byte[], byte[]> (str, byte1, byte2 ));
            }

            return encryptedShares;
        }
    }

}
