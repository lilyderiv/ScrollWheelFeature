using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace Scroll_Feature
{
    [ToolboxItem(true)]
    public class TelemetryWheel : Control
    {
        private List<string> packets = new List<string>();
        public int MaxItemsToKeep { get; set; } = 30;
        public int ItemHeight { get; set; } = 30;


        public TelemetryWheel()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        }

        // Form1'den çağırılacak ana metod (ham string buraya gelir)
        public void AddPacket(string rawTelemetryData)
        {
            string formattedData = FormatTelemetry(rawTelemetryData);

            // Eğer formatlama başarısız olduysa (eksik veri vb.) listeye ekleme
            if (string.IsNullOrEmpty(formattedData)) return;

            packets.Add(formattedData);

            if (packets.Count > MaxItemsToKeep)
            {
                packets.RemoveAt(0);
            }

            this.Invalidate();
        }

        // Senin 17 parçalık "<...>, <...>" formatını çözen metod
        private string FormatTelemetry(string rawData)
        {
            try
            {
                // Form1'deki mantığın aynısı: Virgül, < ve > işaretlerini atarak parçala
                string[] parcalar = rawData.Split(new char[] { ',', '<', '>', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Eğer eksik paket geldiyse (senin sisteminde 17 olması gerekiyor)
                if (parcalar.Length < 17)
                {
                    return null; // Çizilmesi için null döndür, ekrana bozuk satır basmasın
                }

                double inisHizi = ParseDoubleSafe(parcalar[6].Trim());
                double latitude = ParseDoubleSafe(parcalar[9].Trim());
                double longitude = ParseDoubleSafe(parcalar[10].Trim());

                return $"Hız: {inisHizi:F2} m/s | Enlem: {latitude:F4} | Boylam: {longitude:F4}";
            }
            catch
            {
                return null;
            }
        }

        // Nokta/virgül karmaşasını çözen güvenli dönüştürücü (Noktalı veya virgüllü gelse de çevirir)
        private double ParseDoubleSafe(string val)
        {
            if (double.TryParse(val.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
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

                float itemY = centerY - (age * ItemHeight);

                if (itemY < -ItemHeight) break;

                float scale = Math.Max(0.4f, 1.0f - (age * 0.15f));

                // Opaklık hızını düşürdük (35) ki daha fazla geçmiş satır ekranda kalabilsin
                int opacity = (int)Math.Max(0, 255 - (age * 35));

                if (opacity <= 0) continue;

                using (Font scaledFont = new Font(this.Font.FontFamily, this.Font.Size * scale, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.FromArgb(opacity, this.ForeColor)))
                {
                    SizeF textSize = g.MeasureString(packets[i], scaledFont);

                    float textX = (this.Width - textSize.Width) / 2;
                    float textY = itemY - (textSize.Height / 2);

                    g.DrawString(packets[i], scaledFont, brush, textX, textY);
                }
            }
        }

    }
}