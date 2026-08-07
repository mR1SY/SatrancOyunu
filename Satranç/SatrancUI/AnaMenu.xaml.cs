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

        private void OyunaBaslaButon_Click(object sender, RoutedEventArgs e)
        {
            // Arkadaki butonları gizle
            OyunaBaslaButon.Visibility = Visibility.Collapsed;
            TahtaDuzenleButon.Visibility = Visibility.Collapsed;
            Stockfish.Visibility = Visibility.Collapsed;
            CikisButon.Visibility = Visibility.Collapsed;

            // MenuContainer ayarlarını sıfırlayalım
            MenuContainer.Height = double.NaN;
            MenuContainer.Width = double.NaN;
            MenuContainer.ClipToBounds = false;

            OyunModuMenusu oyunModuMenusu = new OyunModuMenusu();

            oyunModuMenusu.MenuIptalEdildi += () =>
            {
                OyunaBaslaButon.Visibility = Visibility.Visible;
                TahtaDuzenleButon.Visibility = Visibility.Visible;
                Stockfish.Visibility = Visibility.Visible;
                CikisButon.Visibility = Visibility.Visible;
                
                // Konteyner içeriğini temizle
                MenuContainer.Content = null; 
            };

            // 1. Kullanıcı oyun modunu (Normal/Yapay Zeka) seçtiğinde bu olay tetiklenecek
            oyunModuMenusu.ModSecildi += (yapayZekaModu) =>
            {
                SureSecimMenusu sureMenusu = new SureSecimMenusu();

                sureMenusu.MenuIptalEdildi += () =>
                {
                    OyunaBaslaButon.Visibility = Visibility.Visible;
                    TahtaDuzenleButon.Visibility = Visibility.Visible;
                    Stockfish.Visibility = Visibility.Visible;
                    CikisButon.Visibility = Visibility.Visible;
                    
                    // Konteyner içeriğini temizle
                    MenuContainer.Content = null;
                };

                sureMenusu.GeriDonuldu += () =>
                {
                    // Container içeriğini bir önceki menü olan oyunModuMenusu ile değiştir
                    MenuContainer.Content = oyunModuMenusu; 
                };

                // Mod seçildiği an, ekrandaki Oyun Modu menüsünü Süre Seçim menüsü ile değiştiriyoruz
                MenuContainer.Content = sureMenusu;

                // 2. Kullanıcı süreyi de seçtiğinde bu olay tetiklenecek ve asıl oyun başlayacak
                sureMenusu.SureSecildi += (dakika, saniye) =>
                {
                    MainWindow oyunPenceresi = new MainWindow(dakika, saniye);
                    oyunPenceresi.yapayZekaModu = yapayZekaModu;
                    oyunPenceresi.Show();
                    oyunPenceresi.DevamEtButonu.Visibility = Visibility.Collapsed;
                    this.Close();
                };
                
            };

            // İlk aşama olarak ekrana Oyun Modu menüsünü yüklüyoruz
            MenuContainer.Content = oyunModuMenusu;
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