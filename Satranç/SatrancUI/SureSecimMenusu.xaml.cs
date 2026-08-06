using System;
using System.Windows;
using System.Windows.Controls;

namespace SatrancUI
{
    public partial class SureSecimMenusu : UserControl
    {
        // Sadece süre seçildiğinde (dakika, saniye) fırlatılacak event
        public event Action<int, int> SureSecildi;

        public SureSecimMenusu()
        {
            InitializeComponent();
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