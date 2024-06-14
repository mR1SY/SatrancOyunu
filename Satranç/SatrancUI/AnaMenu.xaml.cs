using System.Windows;
using System.Windows.Controls;

namespace SatrancUI
{
    // Oyunun ana menüsünü temsil eden pencere sınıfı.
    public partial class AnaMenu : Window
    {
        public AnaMenu() // AnaMenu penceresini oluşturan yapıcı metod.
        {
            InitializeComponent(); // Pencere bileşenlerini başlatır.
        }

        private void OyunaBaslaButon_Click(object sender, RoutedEventArgs e) // "Oyuna Başla" butonuna tıklandığında çalışacak metod.
        {
            OyunModuMenusu oyunModuMenusu = new OyunModuMenusu(); // Oyun modu seçme menüsünü oluşturur.
            oyunModuMenusu.ModSecildi += yapayZekaModu => // Oyun modu seçildiğinde çalışacak olay işleyicisi.
            {
                MainWindow oyunPenceresi = new MainWindow(); // Ana oyun penceresini oluşturur.
                oyunPenceresi.yapayZekaModu = yapayZekaModu; // Oyun modunu ayarlar.
                oyunPenceresi.Show(); // Ana oyun penceresini gösterir.
                oyunPenceresi.DevamEtButonu.Visibility = Visibility.Collapsed; // "Devam Et" butonunu gizler.
                this.Close(); // Ana menü penceresini kapatır
            };

            MenuContainer.Content = oyunModuMenusu; // Ana menüdeki içerik alanına oyun modu seçme menüsünü yerleştirir.
        }

        private void TahtaDuzenleButon_Click(object sender, RoutedEventArgs e) // "Tahta Düzenle" butonuna tıklandığında çalışacak metod.
        {
            MainWindow duzenlemePenceresi = new MainWindow(); // Ana oyun penceresini oluşturur.
            duzenlemePenceresi.tasDuzenlemeModu = true; // Taş düzenleme modunu aktifleştirir.
            duzenlemePenceresi.Show(); // Ana oyun penceresini gösterir.
            this.Close(); // Ana menü penceresini kapatır.
        }

        private void CikisButon_Click(object sender, RoutedEventArgs e) // "Çıkış" butonuna tıklandığında çalışacak metod.
        {
            Application.Current.Shutdown(); // Uygulamayı kapatır.
        }
    }
}