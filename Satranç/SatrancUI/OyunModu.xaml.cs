using System;
using System.Windows;
using System.Windows.Controls;

namespace SatrancUI
{
    public partial class OyunModuMenusu : UserControl
    {
        // Artık sadece seçilen oyun modunu (Yapay zeka mı?) fırlatıyoruz
        public event Action<bool> ModSecildi;

        public OyunModuMenusu()
        {
            InitializeComponent();
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

            // Yapay zeka modunu seçtik (true)
            ModSecildi?.Invoke(true);
        }
    }
}