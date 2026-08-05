using System.Windows;
using System.Windows.Controls;

namespace SatrancUI
{
    // Oyunun ana menüsünü temsil eden pencere sınıfı.
    public partial class AnaMenu : Window
    {
        public static string AktifStockfishYolu = "";
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
                this.Close(); // Ana menü penceresini kapatır.
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

        private void Stockfish_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog diyalog = new Microsoft.Win32.OpenFileDialog();
    
            // Arama penceresinde sadece .exe uzantılı dosyaların görünmesini sağlar
            diyalog.Filter = "Uygulama Dosyaları (*.exe)|*.exe"; 
            diyalog.Title = "Stockfish.exe Dosyasını Seçin";

            if (diyalog.ShowDialog() == true)
            {
                // Kullanıcının rastgele bir .exe seçmesini engellemek için isim doğrulaması
                if (diyalog.SafeFileName.ToLower().Contains("stockfish"))
                {
                    AktifStockfishYolu = diyalog.FileName;     // Arka plan işlemleri için değişkene atar
                }
                else
                {
                    MessageBox.Show("Lütfen geçerli bir Stockfish.exe dosyasını seçin.", "Hatalı Dosya Seçimi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}