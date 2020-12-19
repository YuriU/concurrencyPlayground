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
            this._statusAnimationControl = new NamedMutexDemo.StatusAnimationControl();
            this.SuspendLayout();
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
            this.ClientSize = new System.Drawing.Size(343, 382);
            this.Controls.Add(this._statusAnimationControl);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private StatusAnimationControl _statusAnimationControl;
    }
}

