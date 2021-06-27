namespace NamedMutexDemo
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
            this._btnPass = new System.Windows.Forms.Button();
            this._statusAnimationControl = new NamedMutexDemo.StatusAnimationControl();
            this.SuspendLayout();
            // 
            // _btnPass
            // 
            this._btnPass.Location = new System.Drawing.Point(12, 218);
            this._btnPass.Name = "_btnPass";
            this._btnPass.Size = new System.Drawing.Size(75, 23);
            this._btnPass.TabIndex = 1;
            this._btnPass.Text = "Pass >>>";
            this._btnPass.UseVisualStyleBackColor = true;
            this._btnPass.Click += new System.EventHandler(this._btnPass_Click);
            // 
            // _statusAnimationControl
            // 
            this._statusAnimationControl.Location = new System.Drawing.Point(12, 12);
            this._statusAnimationControl.Name = "_statusAnimationControl";
            this._statusAnimationControl.Size = new System.Drawing.Size(320, 200);
            this._statusAnimationControl.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 246);
            this.Controls.Add(this._btnPass);
            this.Controls.Add(this._statusAnimationControl);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(359, 285);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(359, 285);
            this.Name = "MainForm";
            this.Text = "Named Mutex Demo";
            this.ResumeLayout(false);

        }

        #endregion

        private StatusAnimationControl _statusAnimationControl;
        private System.Windows.Forms.Button _btnPass;
    }
}

