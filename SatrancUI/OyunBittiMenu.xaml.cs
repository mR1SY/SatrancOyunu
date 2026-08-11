using SatrancMantigi;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SatrancUI
{
    // Oyun bitiminde görüntülenen menüyü temsil eden UserControl sınıfı.
    public partial class OyunBitisMenusu : UserControl
    {
        public event Action<Secenek> SeciliSecenek; // Menüden bir seçenek seçildiğinde tetiklenen olay.

        public OyunBitisMenusu(OyunDurumu oyunDurumu) // OyunBitisMenusu nesnesini oyun durumu bilgisiyle oluşturan yapıcı metod.
        {
            InitializeComponent(); // UserControl bileşenlerini başlatır.

            Sonuc sonuc = oyunDurumu.Sonuc; // Oyunun sonucunu alır.
            WinnerText.Text = KazananınMetniniAl(sonuc.Kazanan); // Kazananı metin olarak ayarlar.
            ReasonText.Text = SebepMetniniAl(sonuc.Sebep, oyunDurumu.MevcutOyuncu); // Bitiş sebebini metin olarak ayarlar.
        }

        private static string KazananınMetniniAl(Oyuncu kazanan) // Kazananı metin olarak döndüren metod.
        {
            return kazanan switch // Kazanan oyuncuya göre metin döndürür.
            {
                Oyuncu.Beyaz => "BEYAZ KAZANDI!", // Beyaz oyuncu kazandıysa.
                Oyuncu.Siyah => "SİYAH KAZANDI!", // Siyah oyuncu kazandıysa.
                _ => "BERABERLİK" // Beraberlik durumunda.
            };
        }

        private static string OyuncuStringi(Oyuncu oyuncu) // Oyuncuyu metin olarak döndüren metod.
        {
            return oyuncu switch // Oyuncuya göre metin döndürür.
            {
                Oyuncu.Beyaz => "BEYAZ", // Beyaz oyuncu.
                Oyuncu.Siyah => "SİYAH", // Siyah oyuncu.
                _ => "" // Diğer durumlarda boş dize.
            };
        }

        private static string SebepMetniniAl(BitisSebebi sebep, Oyuncu mevcutOyuncu) // Bitiş sebebini metin olarak döndüren metod.
        {
            return sebep switch // Bitiş sebebine göre metin döndürür.
            {
                BitisSebebi.Pat => $"PAT - {OyuncuStringi(mevcutOyuncu)} HAMLE YAPAMIYOR", // Pat durumunda.
                BitisSebebi.SahMat => $"ŞAH MAT - {OyuncuStringi(mevcutOyuncu)} HAMLE YAPAMIYOR", // Şah mat durumunda.
                BitisSebebi.ElliHamleKurali => "ELLİ-HAMLE KURALI", // 50 hamle kuralı durumunda.
                BitisSebebi.YetersizTas => "YETERSİZ TAŞ", // Yetersiz taş durumunda.
                BitisSebebi.UcKatliTekrar => "ÜÇ KATLI TEKRAR", // Üç katlı tekrar durumunda.
                BitisSebebi.SureDoldu => $"{OyuncuStringi(mevcutOyuncu)}'IN SÜRESİ DOLDU!", // Süre dolması durumunda.
                _ => "" // Diğer durumlarda boş dize.
            };
        }

        private void Restart_Click(object sender, RoutedEventArgs e) // "Yeniden Başlat" butonuna tıklandığında çalışacak metod.
        {
            SeciliSecenek?.Invoke(Secenek.YenidenBaslat); // SeciliSecenek olayını tetikler ve Secenek.YenidenBaslat'ı parametre olarak gönderir.
        }

        private void Exit_Click(object sender, RoutedEventArgs e) // "Çıkış" butonuna tıklandığında çalışacak metod.
        {
            SeciliSecenek?.Invoke(Secenek.Cikis); // SeciliSecenek olayını tetikler ve Secenek.Cikis'i parametre olarak gönderir.
        }
    }
}