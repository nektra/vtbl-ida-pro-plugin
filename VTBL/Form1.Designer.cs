namespace DeviareTest
{
    partial class Form1
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
            this.textOutput = new System.Windows.Forms.TextBox();
            this.listBoxVTBL = new System.Windows.Forms.ListBox();
            this.btnHook = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.comboBoxModules = new System.Windows.Forms.ComboBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnProcess = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.checkSuspended = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textOutput
            // 
            this.textOutput.Location = new System.Drawing.Point(12, 325);
            this.textOutput.Multiline = true;
            this.textOutput.Name = "textOutput";
            this.textOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textOutput.Size = new System.Drawing.Size(337, 132);
            this.textOutput.TabIndex = 0;
            this.textOutput.WordWrap = false;
            // 
            // listBoxVTBL
            // 
            this.listBoxVTBL.FormattingEnabled = true;
            this.listBoxVTBL.Location = new System.Drawing.Point(12, 115);
            this.listBoxVTBL.Name = "listBoxVTBL";
            this.listBoxVTBL.Size = new System.Drawing.Size(337, 147);
            this.listBoxVTBL.TabIndex = 1;
            // 
            // btnHook
            // 
            this.btnHook.Location = new System.Drawing.Point(12, 277);
            this.btnHook.Name = "btnHook";
            this.btnHook.Size = new System.Drawing.Size(75, 23);
            this.btnHook.TabIndex = 2;
            this.btnHook.Text = "Hook";
            this.btnHook.UseVisualStyleBackColor = true;
            this.btnHook.Click += new System.EventHandler(this.btnHook_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(12, 475);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // comboBoxModules
            // 
            this.comboBoxModules.FormattingEnabled = true;
            this.comboBoxModules.Location = new System.Drawing.Point(152, 22);
            this.comboBoxModules.Name = "comboBoxModules";
            this.comboBoxModules.Size = new System.Drawing.Size(197, 21);
            this.comboBoxModules.TabIndex = 4;
            this.comboBoxModules.SelectedIndexChanged += new System.EventHandler(this.comboBoxModules_SelectedIndexChanged);
            this.comboBoxModules.DropDown += new System.EventHandler(this.comboBoxModules_DropDown);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(152, 63);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(197, 23);
            this.progressBar.TabIndex = 5;
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(12, 22);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(123, 21);
            this.btnProcess.TabIndex = 6;
            this.btnProcess.Text = "Create Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // checkSuspended
            // 
            this.checkSuspended.AutoSize = true;
            this.checkSuspended.Location = new System.Drawing.Point(34, 69);
            this.checkSuspended.Name = "checkSuspended";
            this.checkSuspended.Size = new System.Drawing.Size(80, 17);
            this.checkSuspended.TabIndex = 7;
            this.checkSuspended.Text = "Suspended";
            this.checkSuspended.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 510);
            this.Controls.Add(this.checkSuspended);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.comboBoxModules);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnHook);
            this.Controls.Add(this.listBoxVTBL);
            this.Controls.Add(this.textOutput);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textOutput;
        private System.Windows.Forms.ListBox listBoxVTBL;
        private System.Windows.Forms.Button btnHook;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ComboBox comboBoxModules;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox checkSuspended;

    }
}

