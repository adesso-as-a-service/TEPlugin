namespace TEPlugin.Forms
{
    partial class TESetting
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
            this.doubleKey = new System.Windows.Forms.CheckBox();
            this.doubleOwner = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.nVal = new System.Windows.Forms.NumericUpDown();
            this.mVal = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nVal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mVal)).BeginInit();
            this.SuspendLayout();
            // 
            // doubleKey
            // 
            this.doubleKey.AutoSize = true;
            this.doubleKey.Location = new System.Drawing.Point(205, 12);
            this.doubleKey.Name = "doubleKey";
            this.doubleKey.Size = new System.Drawing.Size(138, 17);
            this.doubleKey.TabIndex = 0;
            this.doubleKey.Text = "Allow double key usage";
            this.doubleKey.UseVisualStyleBackColor = true;
            // 
            // doubleOwner
            // 
            this.doubleOwner.AutoSize = true;
            this.doubleOwner.Location = new System.Drawing.Point(205, 35);
            this.doubleOwner.Name = "doubleOwner";
            this.doubleOwner.Size = new System.Drawing.Size(150, 17);
            this.doubleOwner.TabIndex = 1;
            this.doubleOwner.Text = "Allow double owner usage";
            this.doubleOwner.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "# of keys needed to access";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "# of keys to register";
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(163, 72);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 6;
            this.okBtn.Text = "OK";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(279, 70);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 7;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // nVal
            // 
            this.nVal.Location = new System.Drawing.Point(18, 29);
            this.nVal.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.nVal.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nVal.Name = "nVal";
            this.nVal.Size = new System.Drawing.Size(120, 20);
            this.nVal.TabIndex = 8;
            this.nVal.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // mVal
            // 
            this.mVal.Location = new System.Drawing.Point(18, 75);
            this.mVal.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.mVal.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.mVal.Name = "mVal";
            this.mVal.Size = new System.Drawing.Size(120, 20);
            this.mVal.TabIndex = 9;
            this.mVal.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // TESetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(366, 107);
            this.Controls.Add(this.mVal);
            this.Controls.Add(this.nVal);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.doubleOwner);
            this.Controls.Add(this.doubleKey);
            this.Name = "TESetting";
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.nVal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mVal)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox doubleKey;
        private System.Windows.Forms.CheckBox doubleOwner;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.NumericUpDown nVal;
        private System.Windows.Forms.NumericUpDown mVal;
    }
}