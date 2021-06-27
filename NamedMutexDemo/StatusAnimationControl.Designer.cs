namespace NamedMutexDemo
{
    partial class StatusAnimationControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._animationTimer = new System.Windows.Forms.Timer(this.components);
            this._lblIteration = new System.Windows.Forms.Label();
            this._lblIterationValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _animationTimer
            // 
            this._animationTimer.Enabled = true;
            this._animationTimer.Tick += new System.EventHandler(this._animationTimer_Tick);
            // 
            // _lblIteration
            // 
            this._lblIteration.AutoSize = true;
            this._lblIteration.Location = new System.Drawing.Point(210, 20);
            this._lblIteration.Name = "_lblIteration";
            this._lblIteration.Size = new System.Drawing.Size(48, 13);
            this._lblIteration.TabIndex = 0;
            this._lblIteration.Text = "Iteration:";
            // 
            // _lblIterationValue
            // 
            this._lblIterationValue.AutoSize = true;
            this._lblIterationValue.Location = new System.Drawing.Point(264, 20);
            this._lblIterationValue.Name = "_lblIterationValue";
            this._lblIterationValue.Size = new System.Drawing.Size(13, 13);
            this._lblIterationValue.TabIndex = 1;
            this._lblIterationValue.Text = "0";
            // 
            // StatusAnimationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._lblIterationValue);
            this.Controls.Add(this._lblIteration);
            this.Name = "StatusAnimationControl";
            this.Size = new System.Drawing.Size(320, 200);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer _animationTimer;
        private System.Windows.Forms.Label _lblIteration;
        private System.Windows.Forms.Label _lblIterationValue;
    }
}
