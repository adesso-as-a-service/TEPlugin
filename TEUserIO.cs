using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCCrypto;
using TEPlugin.Forms;
using System.Windows.Forms;

namespace TEPlugin
{
    class TEUserIO : IUserIO
    {
        public TEBasicForm Log = new TEBasicForm();

        public void init()
        {
            Log.Show();
        }

        public void close()
        {
            Log.Close();
        }

        public int selectFromList(List<string> list)
        {
            TEListSelect listForm = new TEListSelect(list);
            listForm.ShowDialog();
            if (listForm.DialogResult == DialogResult.Cancel) throw new OperationCanceledException("List Selection cancelled");
            return listForm.ReadSelection();
        }

        public void outputText(string text)
        {
            Log.logBox.AppendText(text);
            Log.logBox.AppendText("\n");
        }

        public byte[] ReadPW(string Prompt)
        {
            PW pwForm = new PW(Prompt);
            pwForm.ShowDialog();
            if (pwForm.DialogResult == DialogResult.Cancel) throw new OperationCanceledException("Password entry cancelled");
            var pw = pwForm.GetReturnVal();
            return Encoding.UTF8.GetBytes(pw);

        }

        public void outputListAbort(List<string> list)
        {
            string delimiter = Environment.NewLine;
            MessageBox.Show((list.Aggregate((i, j) => i + delimiter + j)),"Info",MessageBoxButtons.OK);
        }

        public Tuple<bool,bool,int,int> getSettings()
        {
            TESetting settings = new TESetting();
            settings.ShowDialog();
            if (settings.DialogResult == DialogResult.Cancel)  throw new OperationCanceledException("Settings entry was cancelled");
            return settings.ReadReturnValue();
        }
    }
}
