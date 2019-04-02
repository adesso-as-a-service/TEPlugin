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
    public partial class PW : Form
    {
        public PW(string prompt)
        {
            InitializeComponent();
            this.ControlBox = false;
            this.prompt.Text = prompt;
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (this.pwBox.Text.Length == 0)
            {
                MessageBox.Show("Not a valid entry");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public string GetReturnVal()
        {
            return this.pwBox.Text;
        }
    }
}
