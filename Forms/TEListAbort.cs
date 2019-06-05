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
    public partial class TEListAbort : Form
    {

        public TEListAbort(List<string> list)
        {
            InitializeComponent();
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.HandledEnhancedListView(list);
        }


        private void HandledEnhancedListView(List<string> list)
        {
            ColumnHeader[] headers = new ColumnHeader[2];
            for (int i = 0; i < headers.Length; i++)
            {
                headers[i] = new ColumnHeader();
            }
            this.enhancedListView1.BeginUpdate();
            this.enhancedListView1.Dock = System.Windows.Forms.DockStyle.None;
            this.enhancedListView1.Sortable = true;
            this.enhancedListView1.FullRowSelect = true;
            this.enhancedListView1.ItemSelectionChanged += noSelect;
            this.enhancedListView1.HideSelection = false;
            this.enhancedListView1.MultiSelect = false;
            this.enhancedListView1.Columns.AddRange(headers);
            this.enhancedListView1.View = System.Windows.Forms.View.Details;
            headers[0].Text = "Cert. Subject";
            headers[1].Text = "Public Key Hash (Base64)";




            this.enhancedListView1.HeaderStyle = ColumnHeaderStyle.Clickable;
            for (int i = 0; i < list.Count; i++)
            {
                string entry = list[i];
                ListViewItem listViewItem = new ListViewItem(entry.Split(new string[] { " / " }, StringSplitOptions.None));
                listViewItem.Tag = i;
                enhancedListView1.Items.Add(listViewItem);
            }
            for (int i = 0; i < headers.Length; i++)
            {
                enhancedListView1.Columns[i].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            }

            this.enhancedListView1.EndUpdate();


        }

        private void noSelect(
    object sender,
    ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
                e.Item.Selected = false;
        }




        private void refreshBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
