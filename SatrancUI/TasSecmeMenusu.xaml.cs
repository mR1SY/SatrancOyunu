using SatrancMantigi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SatrancUI
{
    // Taş düzenleme modunda, kullanıcı tarafından seçilebilecek taşları gösteren bir menü.
    public partial class TasSecmeMenusu : UserControl
    {
        public event Action<TasTuru> SecilenTas; // Menüden bir taş seçildiğinde tetiklenen olay.

        private Oyuncu oyuncu; // Menüye ait oyuncunun rengini saklar.
        private MainWindow mainWindow; // Ana oyun penceresine referans.
        public Pozisyon TıklananPozisyon { get; set; } // Tıklanan karenin pozisyonunu saklar.

        public TasSecmeMenusu(Oyuncu oyuncu, MainWindow mainWindow) // TasSecmeMenusu nesnesini oyuncu ve ana oyun penceresi referansıyla oluşturan yapıcı metod.
        {
            InitializeComponent(); // UserControl bileşenlerini başlatır.
            this.oyuncu = oyuncu; // Oyuncu rengini kaydeder.
            this.mainWindow = mainWindow; // Ana oyun penceresi referansını kaydeder.

            SiyahPiyonResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Piyon); // Siyah piyon resmi ayarlanır.
            SiyahAtResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.At); // Siyah at resmi ayarlanır.
            SiyahFilResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Fil); // Siyah fil resmi ayarlanır.
            SiyahKaleResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Kale); // Siyah kale resmi ayarlanır.
            SiyahVezirResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Vezir); // Siyah vezir resmi ayarlanır.
            SiyahSahResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Sah); // Siyah şah resmi ayarlanır.

            BeyazPiyonResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Piyon); // Beyaz piyon resmi ayarlanır.
            BeyazAtResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.At); // Beyaz at resmi ayarlanır.
            BeyazFilResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Fil); // Beyaz fil resmi ayarlanır.
            BeyazKaleResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Kale); // Beyaz kale resmi ayarlanır.
            BeyazVezirResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Vezir); // Beyaz vezir resmi ayarlanır.
            BeyazSahResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Sah); // Beyaz şah resmi ayarlanır.

        }

        public void PopupKapat() // Popup'ı kapatan metod.
        {
            if (Parent is Popup popup) // Parent bir Popup ise...
            {
                popup.IsOpen = false; // Popup'ı kapatır.
            }
        }

        private void TasResmi_MouseDown(object sender, MouseButtonEventArgs e) // Taş resmine tıklandığında çalışacak metod.
        {
            Image image = (Image)sender; // Tıklanan Image nesnesini alır.
            string imageName = image.Name; // Image nesnesinin adını alır.
            string tag = image.Tag.ToString(); // Image nesnesinin Tag değerini alır.
            Pozisyon poz = TıklananPozisyon; // Tıklanan karenin pozisyonunu alır.

            Oyuncu renk = imageName.Contains("Siyah") ? Oyuncu.Siyah : Oyuncu.Beyaz; // Image adında "Siyah" geçiyorsa siyah, aksi halde beyaz oyuncuyu belirler.

            TasTuru tur = tag switch // Tag değerine göre taş türünü belirler.
            {
                "Piyon" => TasTuru.Piyon,
                "At" => TasTuru.At,
                "Fil" => TasTuru.Fil,
                "Kale" => TasTuru.Kale,
                "Vezir" => TasTuru.Vezir,
                "Sah" => TasTuru.Sah,
                _ => throw new ArgumentException("Geçersiz taş türü.")
            };

            mainWindow.KareyeTasEkle(poz, renk, tur); // Seçilen taşı tahtaya ekler.
            PopupKapat(); // Popup'ı kapatır.
        }

        private void PiyonResmi_MouseDown(object sender, MouseButtonEventArgs e) // "Piyon" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.Piyon); // SecilenTas olayını tetikler ve TasTuru.Piyon'u parametre olarak gönderir.
        }

        private void AtResmi_MouseDown(object sender, MouseButtonEventArgs e) // "At" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.At); // SecilenTas olayını tetikler ve TasTuru.At'ı parametre olarak gönderir.
        }

        private void FilResmi_MouseDown(object sender, MouseButtonEventArgs e) // "Fil" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.Fil); // SecilenTas olayını tetikler ve TasTuru.Fil'i parametre olarak gönderir.
        }

        private void KaleResmi_MouseDown(object sender, MouseButtonEventArgs e) // "Kale" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.Kale); // SecilenTas olayını tetikler ve TasTuru.Kale'yi parametre olarak gönderir.
        }

        private void VezirResmi_MouseDown(object sender, MouseButtonEventArgs e) // "Vezir" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.Vezir); // SecilenTas olayını tetikler ve TasTuru.Vezir'i parametre olarak gönderir.
        }

        private void SahResmi_MouseDown(object sender, MouseButtonEventArgs e) // "Şah" resmine tıklandığında çalışacak metod.
        {
            SecilenTas?.Invoke(TasTuru.Sah); // SecilenTas olayını tetikler ve TasTuru.Sah'i parametre olarak gönderir.
        }

        private void KapatButon_Click(object sender, RoutedEventArgs e)
        {
            PopupKapat();
        }

        private void SilButon_Click(object sender, RoutedEventArgs e)
        {
            if (TıklananPozisyon != null)
            {
                // MainWindow'daki silme metodunu çağır
                mainWindow.SeciliTasiSil(TıklananPozisyon);
                PopupKapat(); // İşlem sonrası menüyü kapat
            }
        }
    }
}