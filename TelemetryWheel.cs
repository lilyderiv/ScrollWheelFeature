using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace Scroll_Feature
{
    [ToolboxItem(true)]
    public class TelemetryWheel : Control
    {
        private List<string> packets = new List<string>();
        public int MaxItemsToKeep { get; set; } = 30;

        // Yarıçapı biraz küçülttük ki yazılar ekrana sığsın
        public float WheelRadius { get; set; } = 60f;

        // Açıyı 15'ten 25'e çıkardık. Böylece 2. ve 3. satırlar silindirin arkasına doğru çok daha sert yatacak.
        public float AnglePerItem { get; set; } = 25f;

        public TelemetryWheel()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.DeepSkyBlue;
            this.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        }

        public void AddPacket(string rawTelemetryData)
        {
            string formattedData = FormatTelemetry(rawTelemetryData);
            if (string.IsNullOrEmpty(formattedData)) return;

            packets.Add(formattedData);
            if (packets.Count > MaxItemsToKeep) packets.RemoveAt(0);

            this.Invalidate();
        }

        private string FormatTelemetry(string rawData)
        {
            try
            {
                string[] parcalar = rawData.Split(new char[] { ',', '<', '>', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parcalar.Length < 17) return null;

                double inisHizi = ParseDoubleSafe(parcalar[6].Trim());
                double latitude = ParseDoubleSafe(parcalar[9].Trim());
                double longitude = ParseDoubleSafe(parcalar[10].Trim());

                return $"Hız: {inisHizi:F2} m/s | Enlem: {latitude:F4} | Boylam: {longitude:F4}";
            }
            catch { return null; }
        }

        private double ParseDoubleSafe(string val)
        {
            if (double.TryParse(val.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                return result;
            return 0.0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            if (packets.Count == 0) return;

            float centerY = this.Height / 2f;

            for (int i = packets.Count - 1; i >= 0; i--)
            {
                int age = (packets.Count - 1) - i;

                double angleDegrees = age * AnglePerItem;

                if (angleDegrees >= 90) break;

                double angleRadians = angleDegrees * Math.PI / 180.0;

                float itemY = centerY - (float)(WheelRadius * Math.Sin(angleRadians));

                float perspectiveScale = (float)Math.Cos(angleRadians);

                // Yatayda biraz daha yavaş, dikeyde ise kosinüs kadar (perspektif) küçült
                float scaleX = Math.Max(0.7f, 1.0f - (age * 0.04f));
                float scaleY = Math.Max(0.1f, perspectiveScale);

                int opacity = (int)(255 * perspectiveScale);
                if (opacity <= 5) continue;

                using (Font scaledFont = new Font(this.Font.FontFamily, this.Font.Size * scaleX, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.FromArgb(opacity, this.ForeColor)))
                {
                    SizeF textSize = g.MeasureString(packets[i], scaledFont);

                    float textX = (this.Width - textSize.Width) / 2;
                    float textY = itemY - (textSize.Height / 2);

                    // --- DÜZELTİLMİŞ 3D EZİLME (SQUASH) EFEKTİ ---
                    // 1. Graphics'in mevcut durumunu kaydet (Bu yöntem new Matrix() ten çok daha sağlıklıdır)
                    GraphicsState state = g.Save();

                    // 2. Dönüşüm merkezini yazının 'Tam Ortasına' taşı
                    float textCenterX = textX + (textSize.Width / 2);
                    float textCenterY = textY + (textSize.Height / 2);

                    g.TranslateTransform(textCenterX, textCenterY);

                    // 3. Y ekseninde basıklık ver (Geriye yatma hissi)
                    g.ScaleTransform(1.0f, scaleY);

                    // 4. Dönüşüm merkezini eski yerine al
                    g.TranslateTransform(-textCenterX, -textCenterY);

                    // 5. Çizimi yap
                    g.DrawString(packets[i], scaledFont, brush, textX, textY);

                    // 6. Graphics'i bir sonraki çizim için temiz duruma (eski haline) döndür
                    g.Restore(state);
                }
            }
        }
    }
}