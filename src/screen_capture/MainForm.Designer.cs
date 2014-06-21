namespace btnet
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonCapture = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonSend = new System.Windows.Forms.Button();
            this.buttonConfigure = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.buttonExit = new System.Windows.Forms.Button();
            this.radioButtonArea = new System.Windows.Forms.RadioButton();
            this.radioButtonForeground = new System.Windows.Forms.RadioButton();
            this.radioButtonDesktop = new System.Windows.Forms.RadioButton();
            this.numericUpDownDelay = new System.Windows.Forms.NumericUpDown();
            this.labelDelay = new System.Windows.Forms.Label();
            this.labelBugId = new System.Windows.Forms.Label();
            this.radioButtonUpdateExisting = new System.Windows.Forms.RadioButton();
            this.radioButtonCreateNew = new System.Windows.Forms.RadioButton();
            this.textBoxShortDescription = new System.Windows.Forms.TextBox();
            this.labelDescription = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCopy = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBoxPenType = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripButtonUndo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAbout = new System.Windows.Forms.ToolStripButton();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.textBoxBugId = new btnet.NumericTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDelay)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Gray;
            this.pictureBox1.Location = new System.Drawing.Point(-1, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(138, 125);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // buttonCapture
            // 
            this.buttonCapture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCapture.ForeColor = System.Drawing.Color.Blue;
            this.buttonCapture.Location = new System.Drawing.Point(621, 25);
            this.buttonCapture.Name = "buttonCapture";
            this.buttonCapture.Size = new System.Drawing.Size(130, 39);
            this.buttonCapture.TabIndex = 0;
            this.buttonCapture.Text = "Capture";
            this.buttonCapture.UseVisualStyleBackColor = true;
            this.buttonCapture.Click += new System.EventHandler(this.buttonCapture_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "BugTracker.NET Screen Capture";
            this.notifyIcon1.Visible = true;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoScroll = true;
            this.panel2.AutoScrollMinSize = new System.Drawing.Size(48, 48);
            this.panel2.BackColor = System.Drawing.Color.Gray;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Location = new System.Drawing.Point(9, 23);
            this.panel2.MinimumSize = new System.Drawing.Size(600, 480);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(600, 480);
            this.panel2.TabIndex = 4;
            // 
            // buttonSend
            // 
            this.buttonSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSend.ForeColor = System.Drawing.Color.Blue;
            this.buttonSend.Location = new System.Drawing.Point(621, 190);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(130, 39);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Send";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // buttonConfigure
            // 
            this.buttonConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConfigure.AutoSize = true;
            this.buttonConfigure.ForeColor = System.Drawing.Color.Blue;
            this.buttonConfigure.Location = new System.Drawing.Point(621, 429);
            this.buttonConfigure.Name = "buttonConfigure";
            this.buttonConfigure.Size = new System.Drawing.Size(130, 31);
            this.buttonConfigure.TabIndex = 4;
            this.buttonConfigure.Text = "Configure...";
            this.buttonConfigure.UseVisualStyleBackColor = true;
            this.buttonConfigure.Click += new System.EventHandler(this.buttonConfigure_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonExit.ForeColor = System.Drawing.Color.Blue;
            this.buttonExit.Location = new System.Drawing.Point(621, 479);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(75, 23);
            this.buttonExit.TabIndex = 7;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // radioButtonArea
            // 
            this.radioButtonArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonArea.AutoSize = true;
            this.radioButtonArea.Checked = true;
            this.radioButtonArea.Location = new System.Drawing.Point(6, 5);
            this.radioButtonArea.Name = "radioButtonArea";
            this.radioButtonArea.Size = new System.Drawing.Size(79, 17);
            this.radioButtonArea.TabIndex = 0;
            this.radioButtonArea.TabStop = true;
            this.radioButtonArea.Text = "Select area";
            this.radioButtonArea.UseVisualStyleBackColor = true;
            this.radioButtonArea.CheckedChanged += new System.EventHandler(this.radioButtonArea_CheckedChanged);
            // 
            // radioButtonForeground
            // 
            this.radioButtonForeground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonForeground.AutoSize = true;
            this.radioButtonForeground.Location = new System.Drawing.Point(6, 25);
            this.radioButtonForeground.Name = "radioButtonForeground";
            this.radioButtonForeground.Size = new System.Drawing.Size(118, 17);
            this.radioButtonForeground.TabIndex = 1;
            this.radioButtonForeground.TabStop = true;
            this.radioButtonForeground.Text = "Foreground window";
            this.radioButtonForeground.UseVisualStyleBackColor = true;
            this.radioButtonForeground.CheckedChanged += new System.EventHandler(this.radioButtonForeground_CheckedChanged);
            // 
            // radioButtonDesktop
            // 
            this.radioButtonDesktop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonDesktop.AutoSize = true;
            this.radioButtonDesktop.Location = new System.Drawing.Point(6, 45);
            this.radioButtonDesktop.Name = "radioButtonDesktop";
            this.radioButtonDesktop.Size = new System.Drawing.Size(94, 17);
            this.radioButtonDesktop.TabIndex = 2;
            this.radioButtonDesktop.TabStop = true;
            this.radioButtonDesktop.Text = "Current screen";
            this.radioButtonDesktop.UseVisualStyleBackColor = true;
            this.radioButtonDesktop.CheckedChanged += new System.EventHandler(this.radioButtonDesktop_CheckedChanged);
            // 
            // numericUpDownDelay
            // 
            this.numericUpDownDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownDelay.Location = new System.Drawing.Point(39, 69);
            this.numericUpDownDelay.Name = "numericUpDownDelay";
            this.numericUpDownDelay.Size = new System.Drawing.Size(41, 20);
            this.numericUpDownDelay.TabIndex = 3;
            // 
            // labelDelay
            // 
            this.labelDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDelay.AutoSize = true;
            this.labelDelay.Location = new System.Drawing.Point(3, 71);
            this.labelDelay.Name = "labelDelay";
            this.labelDelay.Size = new System.Drawing.Size(130, 13);
            this.labelDelay.TabIndex = 16;
            this.labelDelay.Text = "Delay                  Seconds";
            // 
            // labelBugId
            // 
            this.labelBugId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelBugId.AutoSize = true;
            this.labelBugId.Location = new System.Drawing.Point(3, 46);
            this.labelBugId.Name = "labelBugId";
            this.labelBugId.Size = new System.Drawing.Size(50, 13);
            this.labelBugId.TabIndex = 19;
            this.labelBugId.Text = "Bug ID#:";
            // 
            // radioButtonUpdateExisting
            // 
            this.radioButtonUpdateExisting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonUpdateExisting.AutoSize = true;
            this.radioButtonUpdateExisting.Location = new System.Drawing.Point(6, 23);
            this.radioButtonUpdateExisting.Name = "radioButtonUpdateExisting";
            this.radioButtonUpdateExisting.Size = new System.Drawing.Size(119, 17);
            this.radioButtonUpdateExisting.TabIndex = 1;
            this.radioButtonUpdateExisting.Text = "Update existing bug";
            this.radioButtonUpdateExisting.UseVisualStyleBackColor = true;
            this.radioButtonUpdateExisting.CheckedChanged += new System.EventHandler(this.radioButtonUpdateExisting_CheckedChanged);
            // 
            // radioButtonCreateNew
            // 
            this.radioButtonCreateNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonCreateNew.AutoSize = true;
            this.radioButtonCreateNew.Checked = true;
            this.radioButtonCreateNew.Location = new System.Drawing.Point(6, 3);
            this.radioButtonCreateNew.Name = "radioButtonCreateNew";
            this.radioButtonCreateNew.Size = new System.Drawing.Size(100, 17);
            this.radioButtonCreateNew.TabIndex = 0;
            this.radioButtonCreateNew.TabStop = true;
            this.radioButtonCreateNew.Text = "Create new bug";
            this.radioButtonCreateNew.UseVisualStyleBackColor = true;
            this.radioButtonCreateNew.CheckedChanged += new System.EventHandler(this.radioButtonCreateNew_CheckedChanged);
            // 
            // textBoxShortDescription
            // 
            this.textBoxShortDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxShortDescription.Location = new System.Drawing.Point(621, 248);
            this.textBoxShortDescription.MaxLength = 200;
            this.textBoxShortDescription.Multiline = true;
            this.textBoxShortDescription.Name = "textBoxShortDescription";
            this.textBoxShortDescription.Size = new System.Drawing.Size(152, 77);
            this.textBoxShortDescription.TabIndex = 2;
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(618, 232);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(63, 13);
            this.labelDescription.TabIndex = 22;
            this.labelDescription.Text = "Description:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.numericUpDownDelay);
            this.panel1.Controls.Add(this.labelDelay);
            this.panel1.Controls.Add(this.radioButtonForeground);
            this.panel1.Controls.Add(this.radioButtonDesktop);
            this.panel1.Controls.Add(this.radioButtonArea);
            this.panel1.Location = new System.Drawing.Point(620, 68);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(147, 101);
            this.panel1.TabIndex = 23;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.Controls.Add(this.radioButtonCreateNew);
            this.panel3.Controls.Add(this.textBoxBugId);
            this.panel3.Controls.Add(this.labelBugId);
            this.panel3.Controls.Add(this.radioButtonUpdateExisting);
            this.panel3.Location = new System.Drawing.Point(620, 331);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(136, 75);
            this.panel3.TabIndex = 24;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonSaveAs,
            this.toolStripSeparator3,
            this.toolStripButtonCopy,
            this.toolStripSeparator1,
            this.toolStripComboBoxPenType,
            this.toolStripButtonUndo,
            this.toolStripButtonAbout});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 40, 0);
            this.toolStrip1.Size = new System.Drawing.Size(784, 25);
            this.toolStrip1.TabIndex = 25;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonSaveAs
            // 
            this.toolStripButtonSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSaveAs.ForeColor = System.Drawing.Color.Blue;
            this.toolStripButtonSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveAs.Image")));
            this.toolStripButtonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveAs.Name = "toolStripButtonSaveAs";
            this.toolStripButtonSaveAs.Size = new System.Drawing.Size(58, 22);
            this.toolStripButtonSaveAs.Text = "Save as...";
            this.toolStripButtonSaveAs.Click += new System.EventHandler(this.toolStripButtonSaveAs_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCopy
            // 
            this.toolStripButtonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonCopy.ForeColor = System.Drawing.Color.Blue;
            this.toolStripButtonCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCopy.Image")));
            this.toolStripButtonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCopy.Name = "toolStripButtonCopy";
            this.toolStripButtonCopy.Size = new System.Drawing.Size(108, 22);
            this.toolStripButtonCopy.Text = "Copy to Clipboard";
            this.toolStripButtonCopy.Click += new System.EventHandler(this.toolStripButtonCopy_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripComboBoxPenType
            // 
            this.toolStripComboBoxPenType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxPenType.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.toolStripComboBoxPenType.Items.AddRange(new object[] {
            "red arrow",
            "red marker",
            "yellow highlighter"});
            this.toolStripComboBoxPenType.Name = "toolStripComboBoxPenType";
            this.toolStripComboBoxPenType.Size = new System.Drawing.Size(121, 25);
            // 
            // toolStripButtonUndo
            // 
            this.toolStripButtonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonUndo.ForeColor = System.Drawing.Color.Blue;
            this.toolStripButtonUndo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUndo.Image")));
            this.toolStripButtonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUndo.Name = "toolStripButtonUndo";
            this.toolStripButtonUndo.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripButtonUndo.Size = new System.Drawing.Size(75, 22);
            this.toolStripButtonUndo.Text = "Undo Line";
            this.toolStripButtonUndo.Click += new System.EventHandler(this.toolStripButtonUndo_Click);
            // 
            // toolStripButtonAbout
            // 
            this.toolStripButtonAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonAbout.ForeColor = System.Drawing.Color.Blue;
            this.toolStripButtonAbout.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAbout.Image")));
            this.toolStripButtonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAbout.Name = "toolStripButtonAbout";
            this.toolStripButtonAbout.Padding = new System.Windows.Forms.Padding(70, 0, 0, 0);
            this.toolStripButtonAbout.Size = new System.Drawing.Size(123, 22);
            this.toolStripButtonAbout.Text = "About...";
            this.toolStripButtonAbout.Click += new System.EventHandler(this.toolStripButtonAbout_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.BackColor = System.Drawing.Color.Orange;
            this.linkLabel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.linkLabel1.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel1.Location = new System.Drawing.Point(703, 479);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(2);
            this.linkLabel1.Size = new System.Drawing.Size(75, 23);
            this.linkLabel1.TabIndex = 26;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Donate!";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // textBoxBugId
            // 
            this.textBoxBugId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBugId.Location = new System.Drawing.Point(56, 43);
            this.textBoxBugId.Name = "textBoxBugId";
            this.textBoxBugId.Size = new System.Drawing.Size(64, 20);
            this.textBoxBugId.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonCapture;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonExit;
            this.ClientSize = new System.Drawing.Size(784, 512);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.textBoxShortDescription);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonConfigure);
            this.Controls.Add(this.buttonSend);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.buttonCapture);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 550);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "BugTracker.NET Screen Capture";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDelay)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonCapture;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.Button buttonConfigure;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.RadioButton radioButtonArea;
        private System.Windows.Forms.RadioButton radioButtonForeground;
        private System.Windows.Forms.RadioButton radioButtonDesktop;
        private System.Windows.Forms.NumericUpDown numericUpDownDelay;
        private System.Windows.Forms.Label labelDelay;
        private NumericTextBox textBoxBugId;
        private System.Windows.Forms.Label labelBugId;
        private System.Windows.Forms.RadioButton radioButtonUpdateExisting;
        private System.Windows.Forms.RadioButton radioButtonCreateNew;
        private System.Windows.Forms.TextBox textBoxShortDescription;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveAs;
        private System.Windows.Forms.ToolStripButton toolStripButtonCopy;
        private System.Windows.Forms.ToolStripButton toolStripButtonUndo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxPenType;
        private System.Windows.Forms.ToolStripButton toolStripButtonAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

