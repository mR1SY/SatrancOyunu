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
            Window aktifPencere = Window.GetWindow(this);
            //MainWindow mevcutPencere = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (string.IsNullOrEmpty(AnaMenu.AktifStockfishYolu))
            {
                OzelUyariPenceresi uyari = new OzelUyariPenceresi("LÜTFEN BİR SATRANÇ MOTORU (STOCKFISH) SEÇİNİZ.");

                if (aktifPencere is MainWindow mainWindow)
                {
                    // İkinci parametreyi göndermiyoruz, sadece uyarı metni veriyoruz.
                    mainWindow.UyariContainer.Content = uyari;
                }
                else if (aktifPencere is AnaMenu anaMenu)
                {
                    anaMenu.UyariContainer.Content = uyari;
                }
                
                // Hatalı durumda alt satırlara inilmesini engelle.
                return;

            }
                        
            if (aktifPencere is MainWindow mWindow && mWindow.tasDuzenlemeModu)
            {
                OzelUyariPenceresi uyari = new OzelUyariPenceresi(
                    "SEÇİLEN SATRANÇ MOTORU BU MODDA STABİL ÇALIŞMAYABİLİR.",
                    () => {
                        ModSecildi?.Invoke(true);
                    }
                );
                        
                mWindow.UyariContainer.Content = uyari;
                // return yazılmazsa alt satıra inip oyunu anında başlatır.
                return;            
            }

            // Yapay zeka modunu seçtik (true)
            ModSecildi?.Invoke(true);
        }
    }
}