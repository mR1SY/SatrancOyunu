using System;
using System.Windows;
using System.Windows.Controls;

namespace SatrancUI
{
    // Oyun modu seçim menüsünü temsil eden UserControl sınıfı.
    public partial class OyunModuMenusu : UserControl
    {
        public event Action<bool> ModSecildi; // Oyun modu seçildiğinde tetiklenen olay.

        public OyunModuMenusu() // OyunModuMenusu nesnesini oluşturan yapıcı metod.
        {
            InitializeComponent(); // UserControl bileşenlerini başlatır.
        }

        private void NormalOyunButon_Click(object sender, RoutedEventArgs e) // "Normal Oyun" butonuna tıklandığında çalışacak metod.
        {
            ModSecildi?.Invoke(false); // ModSecildi olayını tetikler ve yapay zeka modu olarak false değerini gönderir.
        }

        private void YapayZekaButon_Click(object sender, RoutedEventArgs e) // "Yapay Zeka" butonuna tıklandığında çalışacak metod.
        {
            // Stockfish .exe yolunun seçilip seçilmediğini denetler.
            if (string.IsNullOrEmpty(AnaMenu.AktifStockfishYolu))
            {
                // Yol boşsa ekrana uyarı mesajı çıkarır.
                MessageBox.Show("Lütfen Stockfish motoru seçiniz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Visibility = Visibility.Collapsed; //StockFish seçilmediği için oyun modu penceresini gizler
                // return komutu, aşağıdaki kodların çalışmasını durdurur ve oyuna geçişi engeller.
                return; 
            }
            ModSecildi?.Invoke(true); // ModSecildi olayını tetikler ve yapay zeka modu olarak true değerini gönderir.
        }
    }
}