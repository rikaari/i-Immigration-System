
namespace eVerification
{
    partial class PopupForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.m_labelResults = new System.Windows.Forms.Label();
            this.img_status = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.img_status)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.button1.Location = new System.Drawing.Point(168, 192);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 37);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // m_labelResults
            // 
            this.m_labelResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_labelResults.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_labelResults.Location = new System.Drawing.Point(0, 0);
            this.m_labelResults.Name = "m_labelResults";
            this.m_labelResults.Size = new System.Drawing.Size(453, 241);
            this.m_labelResults.TabIndex = 3;
            this.m_labelResults.Text = "Message Box";
            this.m_labelResults.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // img_status
            // 
            this.img_status.Location = new System.Drawing.Point(168, 12);
            this.img_status.Name = "img_status";
            this.img_status.Size = new System.Drawing.Size(112, 76);
            this.img_status.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.img_status.TabIndex = 4;
            this.img_status.TabStop = false;
            // 
            // PopupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(453, 241);
            this.Controls.Add(this.img_status);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.m_labelResults);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Results";
            this.Load += new System.EventHandler(this.PopupForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.img_status)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label m_labelResults;
        private System.Windows.Forms.PictureBox img_status;
    }
}