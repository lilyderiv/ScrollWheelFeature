namespace Scroll_Feature
{
    partial class Form1
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.telemetryWheel = new Scroll_Feature.TelemetryWheel();
            this.SuspendLayout();
            // 
            // telemetryWheel
            // 
            this.telemetryWheel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.telemetryWheel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.telemetryWheel.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.telemetryWheel.Location = new System.Drawing.Point(30, 63);
            this.telemetryWheel.MaxItemsToKeep = 30;
            this.telemetryWheel.Name = "telemetryWheel";
            this.telemetryWheel.Size = new System.Drawing.Size(1425, 284);
            this.telemetryWheel.TabIndex = 0;
            this.telemetryWheel.Text = "telemetryWheel1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1622, 866);
            this.Controls.Add(this.telemetryWheel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private TelemetryWheel telemetryWheel;
    }
}

