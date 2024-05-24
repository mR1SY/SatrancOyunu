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
            OyunModuMenusu oyunModuMenusu = new OyunModuMenusu();
            oyunModuMenusu.ModSecildi += yapayZekaModu =>
            {
                MainWindow oyunPenceresi = new MainWindow();
                oyunPenceresi.yapayZekaModu = yapayZekaModu; // Oyun modunu ayarla
                oyunPenceresi.Show();
                oyunPenceresi.DevamEtButonu.Visibility = Visibility.Collapsed;
                this.Close();
            };

            MenuContainer.Content = oyunModuMenusu; // Menü Container'ına Oyun Modu Menüsü'nü ekleyin
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
