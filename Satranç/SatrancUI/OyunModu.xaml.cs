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
            ModSecildi?.Invoke(true); // ModSecildi olayını tetikler ve yapay zeka modu olarak true değerini gönderir.
        }
    }
}