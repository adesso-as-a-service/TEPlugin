using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TEPlugin.Forms
{
    public partial class TESetting : Form
    {
        public TESetting()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            // Perform sanity checks:
            if (nVal.Value > mVal.Value)
            {
                MessageBox.Show("Please choose a valid set of numbers (Register at least enough keys to decrypt)!");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public Tuple<bool, bool, int, int> ReadReturnValue()
        {
            return new Tuple<bool, bool, int, int>(this.doubleKey.Checked, this.doubleOwner.Checked, Decimal.ToInt32(this.nVal.Value), Decimal.ToInt32(this.mVal.Value));
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
