namespace ArduinoDMX
{
    partial class UserInterface
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
            this.colorWheel1 = new Unclassified.UI.ColorWheel();
            this.SuspendLayout();
            // 
            // colorWheel1
            // 
            this.colorWheel1.Hue = ((byte)(0));
            this.colorWheel1.Lightness = ((byte)(0));
            this.colorWheel1.Location = new System.Drawing.Point(12, 12);
            this.colorWheel1.Name = "colorWheel1";
            this.colorWheel1.Saturation = ((byte)(0));
            this.colorWheel1.SecondaryHues = null;
            this.colorWheel1.Size = new System.Drawing.Size(200, 200);
            this.colorWheel1.TabIndex = 0;
            this.colorWheel1.Text = "colorWheel1";
            this.colorWheel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.colorWheel1_MouseUp);
            // 
            // UserInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 227);
            this.Controls.Add(this.colorWheel1);
            this.Name = "UserInterface";
            this.Text = "UserInterface";
            this.ResumeLayout(false);

        }

        #endregion

        private Unclassified.UI.ColorWheel colorWheel1;

    }
}