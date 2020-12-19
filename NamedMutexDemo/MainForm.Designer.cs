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
            this.statusAnimationControl1 = new NamedMutexDemo.StatusAnimationControl();
            this.SuspendLayout();
            // 
            // statusAnimationControl1
            // 
            this.statusAnimationControl1.Location = new System.Drawing.Point(12, 12);
            this.statusAnimationControl1.Name = "statusAnimationControl1";
            this.statusAnimationControl1.Size = new System.Drawing.Size(200, 200);
            this.statusAnimationControl1.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 382);
            this.Controls.Add(this.statusAnimationControl1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private StatusAnimationControl statusAnimationControl1;
    }
}

