using SatrancMantigi;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SatrancUI
{
    public partial class TasSecmeMenusu : UserControl
    {
        public event Action<TasTuru> SecilenTas;

        private Oyuncu oyuncu; // Oyuncu alanını ekleyin
        private MainWindow mainWindow; // MainWindow referansını ekleyin
        public Pozisyon TıklananPozisyon { get; set; } // Yeni özellik

        public TasSecmeMenusu(Oyuncu oyuncu, MainWindow mainWindow)
        {
            InitializeComponent();
            this.oyuncu = oyuncu; // Gelen oyuncuyu kaydedin
            this.mainWindow = mainWindow; // Gelen mainWindow referansını kaydedin

            // Siyah taş resimlerini ayarla
            SiyahPiyonResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Piyon);
            SiyahAtResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.At);
            SiyahFilResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Fil);
            SiyahKaleResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Kale);
            SiyahVezirResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Vezir);
            SiyahSahResmi.Source = Resimler.ResimAl(Oyuncu.Siyah, TasTuru.Sah);

            // Beyaz taş resimlerini ayarla
            BeyazPiyonResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Piyon);
            BeyazAtResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.At);
            BeyazFilResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Fil);
            BeyazKaleResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Kale);
            BeyazVezirResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Vezir);
            BeyazSahResmi.Source = Resimler.ResimAl(Oyuncu.Beyaz, TasTuru.Sah);

        }
        public void PopupKapat()
        {
            if (Parent is Popup popup)
            {
                popup.IsOpen = false;
            }
        }

        private void TasResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = (Image)sender;
            string imageName = image.Name; // Image kontrolünün adını alın
            string tag = image.Tag.ToString();
            Pozisyon poz = TıklananPozisyon;

            // Image adında "Siyah" geçiyorsa siyah, aksi halde beyaz
            Oyuncu renk = imageName.Contains("Siyah") ? Oyuncu.Siyah : Oyuncu.Beyaz;

            TasTuru tur = tag switch
            {
                "Piyon" => TasTuru.Piyon,
                "At" => TasTuru.At,
                "Fil" => TasTuru.Fil,
                "Kale" => TasTuru.Kale,
                "Vezir" => TasTuru.Vezir,
                "Sah" => TasTuru.Sah,
                _ => throw new ArgumentException("Geçersiz taş türü.")
            };

            // KareyeTasEkle metoduna renk parametresini de gönderin
            mainWindow.KareyeTasEkle(poz, renk, tur);
            PopupKapat();
        }

        private void PiyonResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.Piyon);
        }

        private void AtResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.At);
        }

        private void FilResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.Fil);
        }

        private void KaleResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.Kale);
        }

        private void VezirResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.Vezir);
        }

        private void SahResmi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SecilenTas?.Invoke(TasTuru.Sah);
        }
    }
}
