using SatrancMantigi;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SatrancUI
{
    // Piyon terfi ettiğinde, kullanıcının hangi taşa terfi edeceğini seçebileceği bir menü.
    public partial class TerfiMenusu : UserControl
    {
        public event Action<TasTuru> SecilenTas; // Menüden bir taş seçildiğinde tetiklenen olay.

        public TerfiMenusu(Oyuncu oyuncu) // TerfiMenusu nesnesini oyuncu rengiyle oluşturan yapıcı metod.
        {
            InitializeComponent(); // UserControl bileşenlerini başlatır.

            VezirResmi.Source = Resimler.ResimAl(oyuncu, TasTuru.Vezir); // Vezir resmini ayarlar.
            FilResmi.Source = Resimler.ResimAl(oyuncu, TasTuru.Fil); // Fil resmini ayarlar.
            KaleResmi.Source = Resimler.ResimAl(oyuncu, TasTuru.Kale); // Kale resmini ayarlar.
            AtResmi.Source = Resimler.ResimAl(oyuncu, TasTuru.At); // At resmini ayarlar.
        }

        private void VezirResmi_MouseDown(object sender, MouseButtonEventArgs e) // "Vezir" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.Vezir); // SecilenTas olayını tetikler ve TasTuru.Vezir'i parametre olarak gönderir.
        }

        private void FilResmi_MouseDown(object sender, MouseButtonEventArgs e) // "Fil" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.Fil); // SecilenTas olayını tetikler ve TasTuru.Fil'i parametre olarak gönderir.
        }

        private void KaleResmi_MouseDown(object sender, MouseButtonEventArgs e) // "Kale" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.Kale); // SecilenTas olayını tetikler ve TasTuru.Kale'yi parametre olarak gönderir.
        }

        private void AtResmi_MouseDown(object sender, MouseButtonEventArgs e) // "At" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.At); // SecilenTas olayını tetikler ve TasTuru.At'ı parametre olarak gönderir.
        }
    }
}