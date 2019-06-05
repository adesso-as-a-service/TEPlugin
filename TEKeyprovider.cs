using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeePassLib.Keys;
using KeePassLib.Utility;
using SCCrypto;
using System.Windows.Forms;

namespace TEPlugin
{
    class TEKeyprovider : KeyProvider
    {
        private string pkcs11Library;
      
        public TEKeyprovider(string pkcs11Library)
        {
            this.pkcs11Library = pkcs11Library;
        }

        public override string Name
        {
            get { return "Smartcard Threshold Encryption Key Provider"; }
        }

        public override byte[] GetKey(KeyProviderQueryContext ctx)
        {
            if (pkcs11Library.Length == 0)
            {
                MessageBox.Show("Please select the pkcs11.dll in the settings first!");
                return null;
            }

            SmartCard smartCard = new SmartCard(new Settings(pkcs11Library, new TEUserIO()));
            

            try
            {
                // Open Keyfile
                string filename = UrlUtil.StripExtension(ctx.DatabaseIOInfo.Path) + ".eks";

                if (!ctx.CreatingNewKey)
                {
                    return TEKeyFile.OpenKeyfile(filename, smartCard);
                }
                else
                    return TEKeyFile.CreateKeyfile(filename, smartCard);

            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    return null;
                }
                MessageBox.Show("Message: " + ex.Message + "\nSource: "+ex.Source + "\nTrace:" + ex.StackTrace);
                return null;
            }
        }
    }
}
