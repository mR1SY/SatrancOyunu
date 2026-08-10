using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SatrancUI
{
    public partial class OyunModuMenusu : UserControl
    {
        // Artık sadece seçilen oyun modunu (Yapay zeka mı?) fırlatıyoruz
        public event Action<bool> ModSecildi;
        public event Action MenuIptalEdildi;

        public OyunModuMenusu()
        {
            InitializeComponent();
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Visibility = Visibility.Collapsed;
                e.Handled = true;

                MenuIptalEdildi?.Invoke();
            }
        }

        private void KapatButon_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
            MenuIptalEdildi?.Invoke(); // ESC ile aynı işlevi tetikler
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void NormalOyunButon_Click(object sender, RoutedEventArgs e)
        {
            // Normal oyun (Yapay zeka yok = false)
            ModSecildi?.Invoke(false);
        }

        private void YapayZekaButon_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AnaMenu.AktifStockfishYolu))
            {
                MessageBox.Show("Lütfen Stockfish motoru seçiniz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
                        
            MainWindow mevcutPencere = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (mevcutPencere != null && mevcutPencere.tasDuzenlemeModu)
            {
                MessageBox.Show("Orijinal Stockfish bu modda stabil çalışmayabilir.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Yapay zeka modunu seçtik (true)
            ModSecildi?.Invoke(true);
        }
    }
}