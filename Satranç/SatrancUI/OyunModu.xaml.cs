using System;
using System.Windows;
using System.Windows.Controls;

namespace SatrancUI
{
    public partial class OyunModuMenusu : UserControl
    {
        public event Action<bool> ModSecildi; // Yapay zeka modu seçilip seçilmediğini belirten bool parametresi

        public OyunModuMenusu()
        {
            InitializeComponent();
        }

        private void NormalOyunButon_Click(object sender, RoutedEventArgs e)
        {
            ModSecildi?.Invoke(false); // Normal oyun modu seçildi
        }

        private void YapayZekaButon_Click(object sender, RoutedEventArgs e)
        {
            ModSecildi?.Invoke(true); // Yapay zeka modu seçildi
        }
    }
}