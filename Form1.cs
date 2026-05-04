using System;
using System.Windows.Forms;

namespace Scroll_Feature
{
    public partial class Form1 : Form
    {
        TelemetrySimulator telemetrySimulator;

        public Form1()
        {
            InitializeComponent();

            // Simülatörü saniyede 1 (1000 ms) veri üretecek şekilde başlat
            telemetrySimulator = new TelemetrySimulator(1000);

            // Simülatörden "Yeni Veri Geldi" olayı tetiklendiğinde çalışacak metodu bağlıyoruz
            telemetrySimulator.OnDataReceived += Simulator_OnDataReceived;

            // SİMÜLATÖRÜ BURADA BAŞLAT (Form_Load'a gerek kalmadı)
            telemetrySimulator.Start();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        private void Simulator_OnDataReceived(object sender, string e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    telemetryWheel.AddPacket(e);
                }));
            }
            else
            {
                telemetryWheel.AddPacket(e);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            telemetrySimulator.Stop();
        }
    }
}