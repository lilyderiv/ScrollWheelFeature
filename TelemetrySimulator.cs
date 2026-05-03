using System;
using System.Timers; // Windows.Forms.Timer yerine bunu kullanıyoruz ki gerçek arka plan thread'i simüle edilsin

namespace Scroll_Feature
{
    public class TelemetrySimulator
    {
        private Timer timer;
        private Random rnd = new Random();

        // Başlangıç değerleri (Trabzon/Bursa civarı uydurma koordinatlar)
        private double simHiz = 12.5;
        private double simEnlem = 41.0015;
        private double simBoylam = 39.7178;

        // Dış dünyanın (Form'un) bu veriyi dinleyebilmesi için bir Olay (Event) tanımlıyoruz
        public event EventHandler<string> OnDataReceived;

        public TelemetrySimulator(int msInterval = 1000)
        {
            timer = new Timer(msInterval);
            timer.Elapsed += Timer_Elapsed;
        }

        public void Start() => timer.Start();
        public void Stop() => timer.Stop();

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Değerleri her saniye çok hafif rastgele değiştiriyoruz (Gerçekçi dalgalanma)
            simHiz += (rnd.NextDouble() * 1.0) - 0.5;
            simEnlem += (rnd.NextDouble() * 0.0002) - 0.0001;
            simBoylam += (rnd.NextDouble() * 0.0002) - 0.0001;

            // 17 Parçalı senin formatında sahte paket oluşturuyoruz.
            // Bizim TelemetryWheel sınıfımız 6 (Hız), 9 (Enlem) ve 10 (Boylam) indekslerine bakıyor.
            string sahtePaket = $"<0>, <1>, <2>, <3>, <4>, <5>, <{simHiz:F2}>, <7>, <8>, <{simEnlem:F6}>, <{simBoylam:F6}>, <11>, <12>, <13>, <14>, <15>, <16>";

            // Olayı tetikle (Eğer dinleyen biri varsa ona paketi gönder)
            OnDataReceived?.Invoke(this, sahtePaket);
        }
    }
}