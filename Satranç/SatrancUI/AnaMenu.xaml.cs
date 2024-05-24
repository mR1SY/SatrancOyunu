using System.Windows;
using System.Windows.Controls;

namespace SatrancUI
{
    public partial class AnaMenu : Window
    {
        public AnaMenu()
        {
            InitializeComponent();
        }

        private void OyunaBaslaButon_Click(object sender, RoutedEventArgs e)
        {
            MainWindow oyunPenceresi = new MainWindow(); // Yeni bir oyun penceresi oluştur
            oyunPenceresi.Show(); // Oyun penceresini göster
            oyunPenceresi.DevamEtButonu.Visibility = Visibility.Collapsed;
            this.Close(); // Ana menü penceresini kapat
        }

        private void TahtaDuzenleButon_Click(object sender, RoutedEventArgs e)
        {
            MainWindow duzenlemePenceresi = new MainWindow();
            duzenlemePenceresi.tasDuzenlemeModu = true; // Pencereyi taş düzenleme modunda başlat
            duzenlemePenceresi.Show();
            this.Close();
        }

        private void CikisButon_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); // Uygulamayı kapat
        }
    }
}
