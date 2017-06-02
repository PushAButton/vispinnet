namespace VPTableDoctor
{
    partial class MainForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.FileButton = new System.Windows.Forms.Button();
            this.FolderButton = new System.Windows.Forms.Button();
            this.FileListBox = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.FS = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DMDOption = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ScriptClean = new System.Windows.Forms.CheckBox();
            this.Mechanical = new System.Windows.Forms.CheckBox();
            this.B2S = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.FileButton);
            this.splitContainer1.Panel1.Controls.Add(this.FolderButton);
            this.splitContainer1.Panel1.Controls.Add(this.FileListBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox3);
            this.splitContainer1.Panel2.Controls.Add(this.button1);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(452, 385);
            this.splitContainer1.SplitterDistance = 187;
            this.splitContainer1.TabIndex = 0;
            // 
            // FileButton
            // 
            this.FileButton.Location = new System.Drawing.Point(12, 350);
            this.FileButton.Name = "FileButton";
            this.FileButton.Size = new System.Drawing.Size(162, 23);
            this.FileButton.TabIndex = 1;
            this.FileButton.Text = "Add File";
            this.FileButton.UseVisualStyleBackColor = true;
            this.FileButton.Click += new System.EventHandler(this.FileButton_Click);
            // 
            // FolderButton
            // 
            this.FolderButton.Location = new System.Drawing.Point(12, 321);
            this.FolderButton.Name = "FolderButton";
            this.FolderButton.Size = new System.Drawing.Size(162, 23);
            this.FolderButton.TabIndex = 0;
            this.FolderButton.Text = "Add Folder";
            this.FolderButton.UseVisualStyleBackColor = true;
            this.FolderButton.Click += new System.EventHandler(this.FolderButton_Click);
            // 
            // FileListBox
            // 
            this.FileListBox.FormattingEnabled = true;
            this.FileListBox.Location = new System.Drawing.Point(12, 12);
            this.FileListBox.Name = "FileListBox";
            this.FileListBox.Size = new System.Drawing.Size(162, 303);
            this.FileListBox.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.FS);
            this.groupBox3.Location = new System.Drawing.Point(13, 221);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(236, 59);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Experimental Options (DO NOT USE)";
            this.groupBox3.Visible = false;
            // 
            // FS
            // 
            this.FS.AutoSize = true;
            this.FS.Location = new System.Drawing.Point(14, 25);
            this.FS.Name = "FS";
            this.FS.Size = new System.Drawing.Size(131, 17);
            this.FS.TabIndex = 1;
            this.FS.Text = "Convert to Full-Screen";
            this.FS.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 350);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(236, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Process Tables";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.DMDOption);
            this.groupBox2.Location = new System.Drawing.Point(13, 122);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(236, 93);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Advanced Options";
            // 
            // DMDOption
            // 
            this.DMDOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DMDOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DMDOption.FormattingEnabled = true;
            this.DMDOption.Items.AddRange(new object[] {
            "Remove DMD Position & Rotation",
            "Remove DMD Rotation",
            "Don\'t Change DMD",
            ""});
            this.DMDOption.Location = new System.Drawing.Point(14, 26);
            this.DMDOption.Name = "DMDOption";
            this.DMDOption.Size = new System.Drawing.Size(204, 21);
            this.DMDOption.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ScriptClean);
            this.groupBox1.Controls.Add(this.Mechanical);
            this.groupBox1.Controls.Add(this.B2S);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 104);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Basic Options";
            // 
            // ScriptClean
            // 
            this.ScriptClean.AutoSize = true;
            this.ScriptClean.Checked = true;
            this.ScriptClean.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ScriptClean.Location = new System.Drawing.Point(14, 69);
            this.ScriptClean.Name = "ScriptClean";
            this.ScriptClean.Size = new System.Drawing.Size(95, 17);
            this.ScriptClean.TabIndex = 2;
            this.ScriptClean.Text = "Cleanup Script";
            this.ScriptClean.UseVisualStyleBackColor = true;
            // 
            // Mechanical
            // 
            this.Mechanical.AutoSize = true;
            this.Mechanical.Location = new System.Drawing.Point(14, 46);
            this.Mechanical.Name = "Mechanical";
            this.Mechanical.Size = new System.Drawing.Size(156, 17);
            this.Mechanical.TabIndex = 1;
            this.Mechanical.Text = "Enable Mechanical Plunger";
            this.Mechanical.UseVisualStyleBackColor = true;
            // 
            // B2S
            // 
            this.B2S.AutoSize = true;
            this.B2S.Checked = true;
            this.B2S.CheckState = System.Windows.Forms.CheckState.Checked;
            this.B2S.Location = new System.Drawing.Point(14, 23);
            this.B2S.Name = "B2S";
            this.B2S.Size = new System.Drawing.Size(134, 17);
            this.B2S.TabIndex = 0;
            this.B2S.Text = "Enable B2S Backglass";
            this.B2S.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 385);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "VP Table Doctor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button FileButton;
        private System.Windows.Forms.Button FolderButton;
        private System.Windows.Forms.ListBox FileListBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        public System.Windows.Forms.ComboBox DMDOption;
        public System.Windows.Forms.CheckBox ScriptClean;
        public System.Windows.Forms.CheckBox Mechanical;
        public System.Windows.Forms.CheckBox B2S;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.CheckBox FS;
    }
}

