namespace TEPlugin.Forms
{
    partial class TEListAbort
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.refreshBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.enhancedListView1 = new TEPlugin.Forms.EnhancedListView();
            this.SuspendLayout();
            // 
            // refreshBtn
            // 
            this.refreshBtn.Location = new System.Drawing.Point(136, 329);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(75, 23);
            this.refreshBtn.TabIndex = 3;
            this.refreshBtn.Text = "OK";
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(35, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Please connect one of the following keys:";
            // 
            // enhancedListView1
            // 
            this.enhancedListView1.Location = new System.Drawing.Point(12, 50);
            this.enhancedListView1.Name = "enhancedListView1";
            this.enhancedListView1.Size = new System.Drawing.Size(353, 265);
            this.enhancedListView1.Sortable = false;
            this.enhancedListView1.TabIndex = 4;
            this.enhancedListView1.UseCompatibleStateImageBehavior = false;
            // 
            // TEListAbort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 364);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.enhancedListView1);
            this.Controls.Add(this.refreshBtn);
            this.Name = "TEListAbort";
            this.Text = "Needed Keys";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button refreshBtn;
        private EnhancedListView enhancedListView1;
        private System.Windows.Forms.Label label1;
    }
}