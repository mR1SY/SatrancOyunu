using System;
using System.Windows;
using System.Windows.Controls;

namespace SatrancUI
{
    // Oyun sırasında durdurma menüsünü temsil eden UserControl sınıfı.
    public partial class DurdurmaMenusu : UserControl
    {
        public event Action<Secenek, MainWindow> SecilenSecenek; // Menüden bir seçenek seçildiğinde tetiklenen olay.

        public MainWindow mainWindow; // Ana oyun penceresine referans.

        public DurdurmaMenusu(MainWindow mainWindow) // DurdurmaMenusu nesnesini ana oyun penceresi referansıyla oluşturan yapıcı metod.
        {
            InitializeComponent(); // UserControl bileşenlerini başlatır.
            this.mainWindow = mainWindow; // Ana oyun penceresi referansını kaydeder.
        }

        private void AnaMenu_Click(object sender, RoutedEventArgs e) // "Ana Menü" butonuna tıklandığında çalışacak metod.
        {
            SecilenSecenek?.Invoke(Secenek.AnaMenu, mainWindow); // SecilenSecenek olayını tetikler ve Secenek.AnaMenu'yü parametre olarak gönderir.
        }

        private void DevamEtMenu_Click(object sender, RoutedEventArgs e) // "Devam Et" butonuna tıklandığında çalışacak metod.
        {
            SecilenSecenek?.Invoke(Secenek.DevamEt, mainWindow); // SecilenSecenek olayını tetikler ve Secenek.DevamEt'i parametre olarak gönderir.
        }

        private void CikisMenu_Click(object sender, RoutedEventArgs e) // "Çıkış" butonuna tıklandığında çalışacak metod.
        {
            SecilenSecenek?.Invoke(Secenek.Cikis, mainWindow); // SecilenSecenek olayını tetikler ve Secenek.Cikis'i parametre olarak gönderir.
        }
    }
}