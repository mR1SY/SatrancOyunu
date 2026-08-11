using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SatrancUI
{
    public partial class SureSecimMenusu : UserControl
    {
        // Sadece süre seçildiğinde (dakika, saniye) fırlatılacak event
        public event Action<int, int> SureSecildi;
        public event Action MenuIptalEdildi;
        public event Action GeriDonuldu; 

        public SureSecimMenusu()
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
            MenuIptalEdildi?.Invoke(); // ESC ile aynı işlev
        }

        private void GeriButon_Click(object sender, RoutedEventArgs e)
        {
            GeriDonuldu?.Invoke(); // Ana forma geri dön komutu gönder
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void SureSecim_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag != null)
            {
                string[] parcalar = btn.Tag.ToString().Split(',');
                if (parcalar.Length == 2)
                {
                    int dakika = int.Parse(parcalar[0]);
                    int saniye = int.Parse(parcalar[1]);

                    // Süreler seçildiğinde Ana Menü'ye bildir
                    SureSecildi?.Invoke(dakika, saniye);
                }
            }
        }
    }
}