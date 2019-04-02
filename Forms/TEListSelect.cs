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
    public partial class TEListSelect : Form
    {
        public TEListSelect(List<string> list)
        {
            InitializeComponent();
            this.ControlBox = false;
            this.keyList.DataSource = list;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public int ReadSelection() { return this.keyList.SelectedIndex; }

        private void selectBtn_Click(object sender, EventArgs e)
        {
            if (keyList.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a key!");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            

        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            keyList.SelectedIndex = -1;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
