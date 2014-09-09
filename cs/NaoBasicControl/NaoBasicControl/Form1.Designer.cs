namespace NaoBasicControl
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
            this.btnSay = new System.Windows.Forms.Button();
            this.textBoxIp = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxText = new System.Windows.Forms.TextBox();
            this.btnStand = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.pageSetupDialog1 = new System.Windows.Forms.PageSetupDialog();
            this.btnStandInit = new System.Windows.Forms.Button();
            this.btnStandZero = new System.Windows.Forms.Button();
            this.btnCrouch = new System.Windows.Forms.Button();
            this.btnSit = new System.Windows.Forms.Button();
            this.btnSitRelax = new System.Windows.Forms.Button();
            this.btnLyingBelly = new System.Windows.Forms.Button();
            this.btnLyingBack = new System.Windows.Forms.Button();
            this.btnStiffOff = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSay
            // 
            this.btnSay.Location = new System.Drawing.Point(103, 70);
            this.btnSay.Name = "btnSay";
            this.btnSay.Size = new System.Drawing.Size(75, 23);
            this.btnSay.TabIndex = 0;
            this.btnSay.Text = "Say";
            this.btnSay.UseVisualStyleBackColor = true;
            this.btnSay.Click += new System.EventHandler(this.btnSay_Click);
            // 
            // textBoxIp
            // 
            this.textBoxIp.Location = new System.Drawing.Point(92, 13);
            this.textBoxIp.Name = "textBoxIp";
            this.textBoxIp.Size = new System.Drawing.Size(87, 20);
            this.textBoxIp.TabIndex = 1;
            this.textBoxIp.Text = "167.205.66.109";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Text";
            // 
            // textBoxText
            // 
            this.textBoxText.Location = new System.Drawing.Point(92, 44);
            this.textBoxText.Name = "textBoxText";
            this.textBoxText.Size = new System.Drawing.Size(90, 20);
            this.textBoxText.TabIndex = 4;
            this.textBoxText.Text = "Hello";
            // 
            // btnStand
            // 
            this.btnStand.Location = new System.Drawing.Point(15, 145);
            this.btnStand.Name = "btnStand";
            this.btnStand.Size = new System.Drawing.Size(75, 23);
            this.btnStand.TabIndex = 5;
            this.btnStand.Text = "Stand";
            this.btnStand.UseVisualStyleBackColor = true;
            this.btnStand.Click += new System.EventHandler(this.btnStand_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(98, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Action Button";
            // 
            // btnStandInit
            // 
            this.btnStandInit.Location = new System.Drawing.Point(104, 145);
            this.btnStandInit.Name = "btnStandInit";
            this.btnStandInit.Size = new System.Drawing.Size(75, 23);
            this.btnStandInit.TabIndex = 7;
            this.btnStandInit.Text = "StandInit";
            this.btnStandInit.UseVisualStyleBackColor = true;
            this.btnStandInit.Click += new System.EventHandler(this.btnStandInit_Click);
            // 
            // btnStandZero
            // 
            this.btnStandZero.Location = new System.Drawing.Point(197, 145);
            this.btnStandZero.Name = "btnStandZero";
            this.btnStandZero.Size = new System.Drawing.Size(75, 23);
            this.btnStandZero.TabIndex = 8;
            this.btnStandZero.Text = "StandZero";
            this.btnStandZero.UseVisualStyleBackColor = true;
            this.btnStandZero.Click += new System.EventHandler(this.btnStandZero_Click);
            // 
            // btnCrouch
            // 
            this.btnCrouch.Location = new System.Drawing.Point(15, 187);
            this.btnCrouch.Name = "btnCrouch";
            this.btnCrouch.Size = new System.Drawing.Size(75, 23);
            this.btnCrouch.TabIndex = 9;
            this.btnCrouch.Text = "Crouch";
            this.btnCrouch.UseVisualStyleBackColor = true;
            this.btnCrouch.Click += new System.EventHandler(this.btnCrouch_Click);
            // 
            // btnSit
            // 
            this.btnSit.Location = new System.Drawing.Point(104, 186);
            this.btnSit.Name = "btnSit";
            this.btnSit.Size = new System.Drawing.Size(75, 23);
            this.btnSit.TabIndex = 10;
            this.btnSit.Text = "Sit";
            this.btnSit.UseVisualStyleBackColor = true;
            this.btnSit.Click += new System.EventHandler(this.btnSit_Click);
            // 
            // btnSitRelax
            // 
            this.btnSitRelax.Location = new System.Drawing.Point(197, 186);
            this.btnSitRelax.Name = "btnSitRelax";
            this.btnSitRelax.Size = new System.Drawing.Size(75, 23);
            this.btnSitRelax.TabIndex = 11;
            this.btnSitRelax.Text = "SitRelax";
            this.btnSitRelax.UseVisualStyleBackColor = true;
            this.btnSitRelax.Click += new System.EventHandler(this.btnSitRelax_Click);
            // 
            // btnLyingBelly
            // 
            this.btnLyingBelly.Location = new System.Drawing.Point(15, 227);
            this.btnLyingBelly.Name = "btnLyingBelly";
            this.btnLyingBelly.Size = new System.Drawing.Size(75, 23);
            this.btnLyingBelly.TabIndex = 12;
            this.btnLyingBelly.Text = "LyingBelly";
            this.btnLyingBelly.UseVisualStyleBackColor = true;
            this.btnLyingBelly.Click += new System.EventHandler(this.btnLyingBelly_Click);
            // 
            // btnLyingBack
            // 
            this.btnLyingBack.Location = new System.Drawing.Point(104, 227);
            this.btnLyingBack.Name = "btnLyingBack";
            this.btnLyingBack.Size = new System.Drawing.Size(75, 23);
            this.btnLyingBack.TabIndex = 13;
            this.btnLyingBack.Text = "LyingBack";
            this.btnLyingBack.UseVisualStyleBackColor = true;
            this.btnLyingBack.Click += new System.EventHandler(this.btnLyingBack_Click);
            // 
            // btnStiffOff
            // 
            this.btnStiffOff.BackColor = System.Drawing.Color.Transparent;
            this.btnStiffOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStiffOff.ForeColor = System.Drawing.Color.Red;
            this.btnStiffOff.Location = new System.Drawing.Point(197, 227);
            this.btnStiffOff.Name = "btnStiffOff";
            this.btnStiffOff.Size = new System.Drawing.Size(75, 23);
            this.btnStiffOff.TabIndex = 14;
            this.btnStiffOff.Text = "StiffOff";
            this.btnStiffOff.UseVisualStyleBackColor = false;
            this.btnStiffOff.Click += new System.EventHandler(this.btnStiffOff_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 273);
            this.Controls.Add(this.btnStiffOff);
            this.Controls.Add(this.btnLyingBack);
            this.Controls.Add(this.btnLyingBelly);
            this.Controls.Add(this.btnSitRelax);
            this.Controls.Add(this.btnSit);
            this.Controls.Add(this.btnCrouch);
            this.Controls.Add(this.btnStandZero);
            this.Controls.Add(this.btnStandInit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnStand);
            this.Controls.Add(this.textBoxText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxIp);
            this.Controls.Add(this.btnSay);
            this.Name = "Form1";
            this.Text = "Nao Basic Control";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSay;
        private System.Windows.Forms.TextBox textBoxIp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxText;
        private System.Windows.Forms.Button btnStand;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PageSetupDialog pageSetupDialog1;
        private System.Windows.Forms.Button btnStandInit;
        private System.Windows.Forms.Button btnStandZero;
        private System.Windows.Forms.Button btnCrouch;
        private System.Windows.Forms.Button btnSit;
        private System.Windows.Forms.Button btnSitRelax;
        private System.Windows.Forms.Button btnLyingBelly;
        private System.Windows.Forms.Button btnLyingBack;
        private System.Windows.Forms.Button btnStiffOff;
    }
}

