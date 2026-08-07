using SatrancMantigi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SatrancUI;
using System.Windows.Controls.Primitives;
using SatrancMantigi.Taslar;

namespace SatrancUI
{
    // Satranç oyununun oynandığı ana pencereyi temsil eden sınıf.
    public partial class MainWindow : Window
    {
        #region Tanımlamalar
        private readonly Image[,] TaslarinResimleri = new Image[8, 8]; // Satranç taşlarını gösteren Image nesneleri dizisi.
        private readonly Rectangle[,] Vurgular = new Rectangle[8, 8]; // Seçili kareyi ve olası hamleleri vurgulamak için Rectangle nesneleri dizisi.

        private OyunDurumu oyunDurumu; // Oyunun mevcut durumunu tutar.
        private Pozisyon SecilmisPoz = null; // Seçili karenin pozisyonunu tutar (seçili kare yoksa null).

        public bool tasDuzenlemeModu = false; // Taş düzenleme modunun aktif olup olmadığını belirten bool değişkeni (varsayılan: false).
        private TasTuru secilenTasTuru = TasTuru.Piyon; // Taş düzenleme modunda seçilen taş türünü tutar (varsayılan: Piyon).

        private List<Rectangle> hamleVurgulari = new List<Rectangle>(); // Olası hamleleri vurgulamak için kullanılan Rectangle nesnelerini tutan liste.

        private DispatcherTimer siyahSureSayaci; // Siyah oyuncunun süresini tutan zamanlayıcı.
        private DispatcherTimer beyazSureSayaci; // Beyaz oyuncunun süresini tutan zamanlayıcı.

        private TimeSpan siyahKalanSure;
        private TimeSpan beyazKalanSure;
        private int baslangicDakikasi = 10;
        private int ekSureSaniyesi = 0;

        private TasSecmeMenusu acikTasSecmeMenusu = null; // Açık olan taş seçme menüsü nesnesine referans (açık menü yoksa null).

        public bool yapayZekaModu = false; // Yapay zeka modunun aktif olup olmadığını belirten bool değişkeni (varsayılan: false).
        #endregion

        #region Yapıcı metod
        public MainWindow(int dakika = 10, int ekSure = 0) // MainWindow penceresini oluşturan yapıcı metod.
        {
            InitializeComponent(); // Pencere bileşenlerini başlatır.

            baslangicDakikasi = dakika;
            ekSureSaniyesi = ekSure;
            siyahKalanSure = TimeSpan.FromMinutes(dakika);
            beyazKalanSure = TimeSpan.FromMinutes(dakika);

            TahtayiBaslat(); // Satranç tahtasını başlatır.
            oyunDurumu = new OyunDurumu(Oyuncu.Beyaz, Tahta.Baslangic()); // Yeni bir oyun durumu oluşturur.
            oyunDurumu.HamleDosyasiniSil(); // Hamle dosyasını siler.

            if (tasDuzenlemeModu) // Taş düzenleme modu aktifse...
            {
                TasDuzenlemeModuBaslat(); // Taş düzenleme modunu başlatır.
            }
            else // Taş düzenleme modu aktif değilse...
            {
                oyunDurumu = new OyunDurumu(Oyuncu.Beyaz, Tahta.Baslangic()); // Yeni bir oyun durumu oluşturur.
                TahtaCiz(oyunDurumu.Tahta); // Tahtayı çizer.
                VurgulariGizle(); // Vurguları gizler.
            }
            this.Closed += MainWindow_Closed; // Pencere kapatıldığında MainWindow_Closed metodunu çalıştırır.

            siyahSureSayaci = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) }; // Siyah oyuncunun süresini tutan zamanlayıcıyı oluşturur.
            siyahSureSayaci.Tick += SiyahSureSayaci_Tick; // Zamanlayıcının her saniye SiyahSureSayaci_Tick metodunu çalıştırmasını sağlar.
            beyazSureSayaci = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) }; // Beyaz oyuncunun süresini tutan zamanlayıcıyı oluşturur.
            beyazSureSayaci.Tick += BeyazSureSayaci_Tick; // Zamanlayıcının her saniye BeyazSureSayaci_Tick metodunu çalıştırmasını sağlar.

            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss"); // Siyah oyuncunun kalan süresini ekrana yazar.
            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss"); // Beyaz oyuncunun kalan süresini ekrana yazar.
        }
        #endregion

        #region Pencere kapatıldığında çalışacak olay işleyicisi
        private void MainWindow_Closed(object sender, EventArgs e) // Pencere kapatıldığında çalışacak olay işleyicisi.
        {
            TasSecmeMenusunuKapat(); // Taş seçme menüsünü kapatır.
        }
        #endregion

        #region Taş seçme menüsü kapatıldığında çalışacak olay işleyicisi
        private void TasSecmeMenusu_Closed(object sender, EventArgs e) // Taş seçme menüsü kapatıldığında çalışacak olay işleyicisi.
        {
            acikTasSecmeMenusu = null; // Açık taş seçme menüsü referansını null olarak ayarlar.
        }
        #endregion

        #region Tahta ızgarasına tıklandığında çalışacak olay işleyicisi
        private void TahtaIzgarasi_MouseDown(object sender, MouseButtonEventArgs e) // Tahta ızgarasına tıklandığında çalışacak olay işleyicisi.
        {
            if (MenuEkrandaMi()) // Menü açıksa...
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    return; // Metodu sonlandırır (menü açıkken tahtaya müdahaleyi engeller).
                }
            }

            Point point = e.GetPosition(TasIzgarasi); // Fare imlecinin tahta ızgarasına göre konumunu alır.
            Pozisyon poz = KarePozisyona(point); // Fare imlecinin konumunu satranç tahtası koordinatlarına dönüştürür.

            if (yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah) // Yapay zeka modu aktifse ve sıra siyah oyuncudaysa...
            {
                return; // Metodu sonlandırır (yapay zekanın taşına veya sırasına tıklanmasını engeller).
            }

            if (e.LeftButton == MouseButtonState.Pressed) // Sol fare tuşuna basılırsa...
            {
                if (tasDuzenlemeModu) // Taş düzenleme modunda ise...
                {
                    SecilmisPoz = poz; // Tıklanan kareyi seçili kare olarak ayarlar.
                    VurgulariGizle(); // Vurguları gizler.
                    Vurgular[SecilmisPoz.Satir, SecilmisPoz.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 255, 255, 0)); // Seçili kareyi sarı renkle vurgular.
                }
                else // Taş düzenleme modunda değilse...
                {
                    SecilenPozisyondanItibaren(poz); // Seçili kareye göre hamleleri hesaplar ve vurgular.
                }
            }

            if (tasDuzenlemeModu && e.RightButton == MouseButtonState.Pressed) // Taş düzenleme modunda ise ve sağ fare tuşuna basılırsa...
            {
                Oyuncu oyuncu = !oyunDurumu.Tahta.BosMu(poz) ? oyunDurumu.Tahta[poz].Renk : oyunDurumu.MevcutOyuncu;

                TasSecmeMenusunuKapat(); // Açık olan taş seçme menüsünü kapatır.

                TasSecmeMenusu tasSecmeMenusu = new TasSecmeMenusu(oyuncu, this); // Yeni bir taş seçme menüsü oluşturur.
                tasSecmeMenusu.TıklananPozisyon = poz; // Taş seçme menüsüne tıklanan kareyi bildirir.
                Popup popup = new Popup // Yeni bir Popup penceresi oluşturur.
                {
                    Child = tasSecmeMenusu, // Taş seçme menüsünü Popup'ın içeriği olarak ayarlar.
                    IsOpen = true, // Popup'ı açar.
                    PlacementTarget = TasIzgarasi, // Popup'ın yerleştirileceği hedefi ayarlar (tahta ızgarası).
                    Placement = PlacementMode.MousePoint, // Popup'ı fare imlecinin konumuna yerleştirir.
                    StaysOpen = false // Popup'ın tıklama dışında kapatılmasını sağlar.
                };

                acikTasSecmeMenusu = tasSecmeMenusu; // Açık taş seçme menüsü referansını ayarlar.

                popup.Closed += TasSecmeMenusu_Closed; // Popup kapatıldığında TasSecmeMenusu_Closed metodunu çalıştırır.

                tasSecmeMenusu.SecilenTas += tur => // Taş seçme menüsünden bir taş seçildiğinde çalışacak olay işleyicisi.
                {
                    popup.IsOpen = false; // Popup'ı kapatır.
                    secilenTasTuru = tur; // Seçilen taş türünü kaydeder.
                    KareyeTasEkle(poz, oyuncu, tur); // Seçilen taşı tahtaya ekler.
                    acikTasSecmeMenusu = null; // Açık taş seçme menüsü referansını null olarak ayarlar.
                };
            }
        }
        #endregion

        #region Klavye tuşuna basıldığında çalışacak olay işleyicisi
        private void Window_KeyDown(object sender, KeyEventArgs e) // Klavye tuşuna basıldığında çalışacak olay işleyicisi.
        {
            if (tasDuzenlemeModu && e.Key == Key.Delete && SecilmisPoz != null)
            {
                oyunDurumu.Tahta[SecilmisPoz] = null; // Seçili karedeki taşı siler.
                TaslarinResimleri[SecilmisPoz.Satir, SecilmisPoz.Sutun].Source = null; // Taşın görüntüsünü temizler.
                SecilmisPoz = null; // Seçili kareyi sıfırlar.
                VurgulariGizle(); // Vurguları gizler.
            }

            if (e.Key == Key.Escape) // Escape tuşuna basıldıysa...
            {
                if (MenuEkrandaMi() && MenuContainer.Content is DurdurmaMenusu) // Durdurma menüsü açıksa...
                {
                    MenuContainer.Content = null; // Durdurma menüsünü kapatır.

                    if (!tasDuzenlemeModu) // Taş düzenleme modunda değilse...
                    {
                        if (oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz) // Sıra beyaz oyuncu daysa...
                        {
                            beyazSureSayaci.Start(); // Beyaz oyuncunun süresini başlatır.
                        }
                        else // Sıra siyah oyuncu daysa...
                        {
                            siyahSureSayaci.Start(); // Siyah oyuncunun süresini başlatır.
                        }
                    }
                }
                else if (!MenuEkrandaMi()) // Durdurma menüsü açık değilse...
                {
                    DurdurmaMenusunuGoster(); // Durdurma menüsünü gösterir.
                }
            }
        }
        #endregion

        #region Siyah oyuncunun süresi her saniye azaldığında çalışacak metod
        private void SiyahSureSayaci_Tick(object sender, EventArgs e) // Siyah oyuncunun süresi her saniye azaldığında çalışacak metod.
        {
            siyahKalanSure -= TimeSpan.FromSeconds(1); // Siyah oyuncunun kalan süresinden 1 saniye çıkarır.
            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss"); // Siyah oyuncunun kalan süresini ekrana yazar.

            if (siyahKalanSure <= TimeSpan.Zero) // Siyah oyuncunun süresi bittiyse...
            {
                siyahSureSayaci.Stop(); // Zamanlayıcıyı durdurur.
                oyunDurumu.OyunuBitir(Oyuncu.Beyaz, BitisSebebi.SureDoldu); // Oyunu Beyaz oyuncunun kazanmasıyla bitirir.
                OyunBitisiGoster(); // Oyun bitiş ekranını gösterir.
            }
        }
        #endregion

        #region Beyaz oyuncunun süresi her saniye azaldığında çalışacak metod
        private void BeyazSureSayaci_Tick(object sender, EventArgs e) // Beyaz oyuncunun süresi her saniye azaldığında çalışacak metod.
        {
            beyazKalanSure -= TimeSpan.FromSeconds(1); // Beyaz oyuncunun kalan süresinden 1 saniye çıkarır.
            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss"); // Beyaz oyuncunun kalan süresini ekrana yazar.

            if (beyazKalanSure <= TimeSpan.Zero) // Beyaz oyuncunun süresi bittiyse...
            {
                beyazSureSayaci.Stop(); // Zamanlayıcıyı durdurur.
                oyunDurumu.OyunuBitir(Oyuncu.Siyah, BitisSebebi.SureDoldu); // Oyunu Siyah oyuncunun kazanmasıyla bitirir.
                OyunBitisiGoster(); // Oyun bitiş ekranını gösterir.
            }
        }
        #endregion

        #region "Durdur" butonuna tıklandığında çalışacak metod
        private void DurdurButonu_Click(object sender, RoutedEventArgs e)
        {
            siyahSureSayaci.Stop();
            beyazSureSayaci.Stop();
        }
        #endregion

        #region "Yeniden Başlat" butonuna tıklandığında çalışacak metod
        private void YenidenBaslatButonu_Click(object sender, RoutedEventArgs e)
        {
            beyazKalanSure = TimeSpan.FromMinutes(baslangicDakikasi);
            siyahKalanSure = TimeSpan.FromMinutes(baslangicDakikasi);
            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss");
            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss");

            VurgulariGizle();

            beyazSureSayaci.Stop();
            siyahSureSayaci.Stop();
            OyunuYenidenBaslat();
        }
        #endregion

        #region "Devam Et" butonuna tıklandığında çalışacak metod
        private void DevamEtButonu_Click(object sender, RoutedEventArgs e)
        {
            Sayma sayma = oyunDurumu.Tahta.ParcaSayisi();
            if (sayma.Beyaz(TasTuru.Sah) != 1 || sayma.Siyah(TasTuru.Sah) != 1)
            {
                MessageBox.Show("Her iki oyuncunun da bir şahı olmalı.");
                return;
            }

            VurgulariGizle();

            if (oyunDurumu.Tahta.TehditAltinda(oyunDurumu.MevcutOyuncu) || oyunDurumu.Tahta.TehditAltinda(oyunDurumu.MevcutOyuncu.Rakip()))
            {
                MessageBox.Show("Şahınız tehdit altında.");
                return;
            }

            oyunDurumu.OyunBitisiKontrol();
            if (oyunDurumu.OyunBittiMi())
            {
                OyunBitisiGoster();
                return;
            }

            if (oyunDurumu.Tahta.ParcaSayisi().ToplamSayi > 2 && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
            {
                siyahSureSayaci.IsEnabled = true;
            }

            if (oyunDurumu.Tahta.ParcaSayisi().ToplamSayi > 2 && oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz)
            {
                beyazSureSayaci.IsEnabled = true;
            }

            if (tasDuzenlemeModu)
            {
                beyazSureSayaci.Stop();
                siyahSureSayaci.Stop();
            }

            DevamEtButonu.Visibility = Visibility.Collapsed;
            tasDuzenlemeModu = false;
        }
        #endregion

        #region Satranç tahtasını başlatan metod
        private void TahtayiBaslat()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Image image = new();
                    TaslarinResimleri[r, c] = image;
                    TasIzgarasi.Children.Add(image);

                    Rectangle vurgu = new Rectangle();
                    Vurgular[r, c] = vurgu;
                    VurguIzgarasi.Children.Add(vurgu);
                }
            }
        }
        #endregion

        #region Tahtayı çizen metod
        private void TahtaCiz(Tahta tahta)
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Tas tas = tahta[r, c];

                    if (tas != null && tas.Tur == TasTuru.Sah && oyunDurumu.Tahta.TehditAltinda(tas.Renk))
                    {
                        if (tas.Renk == Oyuncu.Beyaz)
                        {
                            TaslarinResimleri[r, c].Source = Resimler.ResimYukle("Assets/BeyazSahTehditAltinda.png");
                        }
                        else
                        {
                            TaslarinResimleri[r, c].Source = Resimler.ResimYukle("Assets/SiyahSahTehditAltinda.png");
                        }
                    }
                    else
                    {
                        TaslarinResimleri[r, c].Source = Resimler.ResimAl(tas);
                    }

                    TahtaIzgarasi.LayoutUpdated += (sender, e) =>
                    {
                        if (!yapayZekaModu)
                        {
                            if (oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        TaslarinResimleri[i, j].RenderTransform = new RotateTransform(180, TaslarinResimleri[i, j].ActualWidth / 2, TaslarinResimleri[i, j].ActualHeight / 2);
                                    }
                                }
                                TahtaIzgarasi.RenderTransform = new RotateTransform(180, TahtaIzgarasi.ActualWidth / 2, TahtaIzgarasi.ActualHeight / 2);
                            }
                            else
                            {
                                for (int i = 0; i < 8; i++)
                                {
                                    for (int j = 0; j < 8; j++)
                                    {
                                        TaslarinResimleri[i, j].RenderTransform = null;
                                    }
                                }
                                TahtaIzgarasi.RenderTransform = null;
                            }
                        }
                    };

                    if (!yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
                    {
                        TahtaIzgarasi.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Tahta_180.png")));
                    }
                    else
                    {
                        TahtaIzgarasi.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Tahta.png")));
                    }
                }
            }
        }
        #endregion

        #region Taş düzenleme modunu başlatan metod
        private void TasDuzenlemeModuBaslat()
        {
            siyahSureSayaci.IsEnabled = false;
            beyazSureSayaci.IsEnabled = false;

            DevamEtButonu.Visibility = Visibility.Visible;
        }
        #endregion

        #region Verilen pozisyona verilen renkte ve türde bir taş ekleyen metod
        public void KareyeTasEkle(Pozisyon poz, Oyuncu oyuncu, TasTuru tur)
        {
            oyunDurumu.Tahta[poz] = TasOlustur(oyuncu, tur);
            TaslarinResimleri[poz.Satir, poz.Sutun].Source = Resimler.ResimAl(oyunDurumu.Tahta[poz]);
        }
        #endregion

        #region Verilen renkte ve türde bir taş oluşturan metod
        private Tas TasOlustur(Oyuncu renk, TasTuru tur)
        {
            return tur switch
            {
                TasTuru.Piyon => new Piyon(renk),
                TasTuru.At => new At(renk),
                TasTuru.Kale => new Kale(renk),
                TasTuru.Fil => new Fil(renk),
                TasTuru.Vezir => new Vezir(renk),
                TasTuru.Sah => new Sah(renk),
                _ => null
            };
        }
        #endregion

        #region Fare imlecinin konumunu satranç tahtası koordinatlarına dönüştüren metod
        private Pozisyon KarePozisyona(Point point)
        {
            double squareSize = TahtaIzgarasi.ActualWidth / 8;
            int satir = (int)(point.Y / squareSize);
            int sutun = (int)(point.X / squareSize);
            return new Pozisyon(satir, sutun);
        }
        #endregion

        #region Seçili kareye göre hamleleri hesaplar ve vurgular
        private void SecilenPozisyondanItibaren(Pozisyon poz)
        {
            if (poz == SecilmisPoz)
            {
                SecilmisPoz = null;
                VurgulariGizle();
                return;
            }

            VurgulariGizle();

            IEnumerable<Hamle> hamleler = oyunDurumu.TaslarIcinYasalHamleler(poz, tasDuzenlemeModu);

            if (hamleler.Any())
            {
                SecilmisPoz = poz;
                OnbellekHamleleri(hamleler);

                if (!tasDuzenlemeModu)
                {
                    VurgulamayiGoster();
                }

                ImlecAyarla(oyunDurumu.MevcutOyuncu);
                Vurgular[SecilmisPoz.Satir, SecilmisPoz.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 255, 255, 0));
            }
            else
            {
                SecilenKonuma(poz);
            }
        }
        #endregion

        #region Tıklanan kareyi seçili kare olarak ayarlar (taş yoksa)
        private void SecilenKonuma(Pozisyon poz)
        {
            VurgulariGizle();

            if (poz == SecilmisPoz)
            {
                SecilmisPoz = null;
                return;
            }

            if (SecilmisPoz != null && hamleBellegi.TryGetValue(poz, out Hamle hamle))
            {
                if (hamle != null && hamle.FromPos == SecilmisPoz)
                {
                    if (hamle.Tur == HamleTuru.PiyonTerfi)
                    {
                        TerfiTasima(hamle.FromPos, hamle.ToPos);
                    }
                    else
                    {
                        TasimaHamlesi(hamle);
                    }
                }
            }

            SecilmisPoz = null;
        }
        #endregion

        #region Piyon terfi işlemini başlatan metod
        private void TerfiTasima(Pozisyon from, Pozisyon to)
        {
            TaslarinResimleri[to.Satir, to.Sutun].Source = Resimler.ResimAl(oyunDurumu.MevcutOyuncu, TasTuru.Piyon);
            TaslarinResimleri[to.Satir, to.Sutun].Source = null;

            TerfiMenusu trfMenusu = new TerfiMenusu(oyunDurumu.MevcutOyuncu);
            MenuContainer.Content = trfMenusu;

            trfMenusu.SecilenTas += tur =>
            {
                MenuContainer.Content = null;
                Hamle trfHamlesi = new PiyonTerfi(from, to, tur);
                TasimaHamlesi(trfHamlesi);
            };
        }
        #endregion

        private readonly Dictionary<Pozisyon, Hamle> hamleBellegi = new Dictionary<Pozisyon, Hamle>();

        #region Hamleyi gerçekleştiren ve süreleri ayarlayan Ana Metod
        private void TasimaHamlesi(Hamle hamle)
        {
            if (hamle != null)
            {
                if (tasDuzenlemeModu)
                {
                    if (oyunDurumu.Tahta[hamle.FromPos] != null)
                    {
                        TaslarinResimleri[hamle.FromPos.Satir, hamle.FromPos.Sutun].Source = null;
                        oyunDurumu.Tahta[hamle.FromPos] = null;
                    }

                    if (oyunDurumu.Tahta[hamle.ToPos] != null)
                    {
                        TaslarinResimleri[hamle.ToPos.Satir, hamle.ToPos.Sutun].Source = null;
                        oyunDurumu.Tahta[hamle.ToPos] = null;
                    }

                    oyunDurumu.Tahta[hamle.ToPos] = oyunDurumu.Tahta[hamle.FromPos];
                    oyunDurumu.Tahta[hamle.FromPos] = null;

                    TahtaIzgarasi.UpdateLayout();
                    TaslarinResimleri[hamle.ToPos.Satir, hamle.ToPos.Sutun].Source = Resimler.ResimAl(oyunDurumu.Tahta[hamle.ToPos]);

                    SecilmisPoz = null;
                    VurgulariGizle();
                    return;
                }

                // 1. HAMLEYİ YÜRÜT
                oyunDurumu.HareketEt(hamle);
                VurgulariGizle();

                // 2. YENİ HAMLE VURGULARINI GÖSTER
                if (!tasDuzenlemeModu)
                {
                    Vurgular[hamle.FromPos.Satir, hamle.FromPos.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 0, 255, 0));
                    Vurgular[hamle.ToPos.Satir, hamle.ToPos.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 0, 255, 0));
                }

                TahtaCiz(oyunDurumu.Tahta);
                ImlecAyarla(oyunDurumu.MevcutOyuncu);
                oyunDurumu.HamleyiKaydet(hamle);
                OnbellekHamleleri(oyunDurumu.TaslarIcinYasalHamleler(hamle.ToPos));

                // 3. SÜRELERİ DEĞİŞTİR (Hamle yapıldı, sıra diğerinde)
                if (oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz) // Siyah hamlesini bitirdi, sıra beyaza geçti
                {
                    siyahKalanSure += TimeSpan.FromSeconds(ekSureSaniyesi); // Siyaha ek süresini ver
                    siyahSureSayaci.Stop();
                    beyazSureSayaci.Start();
                }
                else // Beyaz hamlesini bitirdi, sıra siyaha geçti
                {
                    beyazKalanSure += TimeSpan.FromSeconds(ekSureSaniyesi); // Beyaza ek süresini ver
                    beyazSureSayaci.Stop();
                    siyahSureSayaci.Start();
                }

                // UI Süre güncellemeleri
                SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss");
                BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss");

                // İki Kişilik (Yapay zeka yoksa) tahta dönüş ayarları
                if (!yapayZekaModu)
                {
                    if (oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz)
                    {
                        SiyahSureText.VerticalAlignment = VerticalAlignment.Bottom;
                        SiyahOyuncuText.VerticalAlignment = VerticalAlignment.Top;
                        BeyazSureText.VerticalAlignment = VerticalAlignment.Top;
                        BeyazOyuncuText.VerticalAlignment = VerticalAlignment.Bottom;
                        Grid.SetRow(SiyahSureText, 0);
                        Grid.SetRow(SiyahOyuncuText, 1);
                        Grid.SetRow(BeyazSureText, 6);
                        Grid.SetRow(BeyazOyuncuText, 5);
                    }
                    else if (oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
                    {
                        SiyahSureText.VerticalAlignment = VerticalAlignment.Top;
                        SiyahOyuncuText.VerticalAlignment = VerticalAlignment.Bottom;
                        BeyazSureText.VerticalAlignment = VerticalAlignment.Bottom;
                        BeyazOyuncuText.VerticalAlignment = VerticalAlignment.Top;
                        Grid.SetRow(BeyazSureText, 0);
                        Grid.SetRow(BeyazOyuncuText, 1);
                        Grid.SetRow(SiyahSureText, 6);
                        Grid.SetRow(SiyahOyuncuText, 5);
                    }
                }

                // 4. OYUN BİTTİ Mİ KONTROLÜ
                if (oyunDurumu.OyunBittiMi())
                {
                    OyunBitisiGoster();
                    return;
                }

                // 5. EĞER YAPAY ZEKA MODUYSA VE SIRA SİYAHTAYSA MOTORU TETİKLE
                if (yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
                {
                    YapayZekayiCalistirAsync();
                }
            }
        }
        #endregion

        #region Yapay Zekayı Asenkron Olarak Tetikleyen Güvenli Metod
        #region Yapay Zekayı Asenkron Olarak Tetikleyen Güvenli Metod
        private async void YapayZekayiCalistirAsync()
        {
            // Motor düşünmeden önce arayüzün (UI) rahatça güncellenmesi için ufak bir esneme payı veriyoruz
            await Task.Delay(50);

            try
            {
                // Motorun hamlesini arka planda bekle
                Hamle yapayZekaHamlesi = await Task.Run(() => YapayZekaHamlesiHesapla());

                if (yapayZekaHamlesi != null)
                {
                    TasimaHamlesi(yapayZekaHamlesi); // Motorun hamlesini uygula
                }
            }
            catch (Exception ex)
            {
                // Eğer motorda veya dosya yolunda gerçekten bir sorun varsa sessizce kalmasın, bize hata mesajı versin
                MessageBox.Show("Yapay zeka motoru çalıştırılırken bir hata oluştu:\n" + ex.Message, "Motor Hatası", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Yapay zekanın hamlesini hesaplayan (DİNAMİK SÜRE YÖNETİMLİ) metod
        private Hamle YapayZekaHamlesiHesapla()
        {
            var yasalHamleler = oyunDurumu.ButunYasalHamlelerIcin(Oyuncu.Siyah).ToList();

            if (yasalHamleler.Count == 0)
            {
                return null;
            }

            string stockfishYolu = AnaMenu.AktifStockfishYolu;
            string enIyiHamleMetni = "";

            using (System.Diagnostics.Process motor = new System.Diagnostics.Process())
            {
                motor.StartInfo.FileName = stockfishYolu;
                motor.StartInfo.UseShellExecute = false;
                motor.StartInfo.RedirectStandardInput = true;
                motor.StartInfo.RedirectStandardOutput = true;
                motor.StartInfo.CreateNoWindow = true;
                motor.Start();

                // UCI Protokolüne uygun resmi uyanış komutları
                motor.StandardInput.WriteLine("uci");
                motor.StandardInput.WriteLine("isready");

                string fen = oyunDurumu.TahtaDurumunuFenYap();
                motor.StandardInput.WriteLine($"position fen {fen}");

                // Süre hesaplamaları (Milisaniye cinsinden. Güvenlik amacıyla 1'in altına düşmesini engelliyoruz)
                int wtime = Math.Max(1, (int)beyazKalanSure.TotalMilliseconds);
                int btime = Math.Max(1, (int)siyahKalanSure.TotalMilliseconds);
                int inc = ekSureSaniyesi * 1000;

                // Motor kendi süresine bakarak ne kadar düşüneceğine kendi karar veriyor
                motor.StandardInput.WriteLine($"go wtime {wtime} btime {btime} winc {inc} binc {inc}");

                while (true)
                {
                    string cikti = motor.StandardOutput.ReadLine();
                    // Motor cevabını verene kadar (bestmove diyene kadar) döngü bekler
                    if (cikti != null && cikti.StartsWith("bestmove"))
                    {
                        enIyiHamleMetni = cikti.Split(' ')[1];
                        break;
                    }
                }
                motor.StandardInput.WriteLine("quit");
                motor.WaitForExit();
            }

            Hamle enIyiHamle = yasalHamleler.FirstOrDefault(h => h.UciFormatinaCevir() == enIyiHamleMetni);
            return enIyiHamle;
        }
        #endregion
        #endregion

        #region Vezirin erken hareketini cezalandıran değerlendirme fonksiyonu
        private int VezirErkenHareketCezasi(Tahta tahta, Oyuncu oyuncu)
        {
            if (oyuncu == Oyuncu.Beyaz)
            {
                if (tahta[7, 3] == null || tahta[7, 3].Tasindi)
                {
                    return 1;
                }
            }
            else
            {
                if (tahta[0, 3] == null || tahta[0, 3].Tasindi)
                {
                    return 1;
                }
            }
            return 0;
        }
        #endregion

        #region Vezirin güvenliğini değerlendiren değerlendirme fonksiyonu
        private int VezirGuvenligi(Tahta tahta, Oyuncu oyuncu)
        {
            Pozisyon vezirPozisyonu = tahta.TasBul(oyuncu, TasTuru.Vezir);
            if (tahta.TehditAltinda(vezirPozisyonu, oyuncu))
            {
                return -1;
            }
            return 1;
        }
        #endregion

        #region Şahın erken hareketini cezalandıran değerlendirme fonksiyonu
        private int SahErkenHareketCezasi(Tahta tahta, Oyuncu oyuncu)
        {
            if (oyuncu == Oyuncu.Beyaz)
            {
                if (tahta[7, 4] == null || tahta[7, 4].Tasindi)
                {
                    return 1;
                }
            }
            else
            {
                if (tahta[0, 4] == null || tahta[0, 4].Tasindi)
                {
                    return 1;
                }
            }
            return 0;
        }
        #endregion

        #region Rok yapılıp yapılmadığını değerlendiren değerlendirme fonksiyonu
        private int RokAvantajı(Tahta tahta, Oyuncu oyuncu)
        {
            if (oyuncu == Oyuncu.Beyaz)
            {
                if (tahta[7, 4] == null || tahta[7, 4].Tasindi)
                {
                    return -1;
                }
                if ((tahta[7, 6] != null && tahta[7, 6].Tur == TasTuru.Sah) ||
                    (tahta[7, 2] != null && tahta[7, 2].Tur == TasTuru.Sah))
                {
                    return 1;
                }
            }
            else
            {
                if (tahta[0, 4] == null || tahta[0, 4].Tasindi)
                {
                    return -1;
                }
                if ((tahta[0, 6] != null && tahta[0, 6].Tur == TasTuru.Sah) ||
                    (tahta[0, 2] != null && tahta[0, 2].Tur == TasTuru.Sah))
                {
                    return 1;
                }
            }
            return 0;
        }
        #endregion

        #region Tahtanın değerini hesaplayan değerlendirme fonksiyonu
        private int DegerlendirmeFonksiyonu(Tahta tahta)
        {
            int puan = 0;

            puan += VezirGuvenligi(tahta, Oyuncu.Siyah) * 50;
            puan -= VezirGuvenligi(tahta, Oyuncu.Beyaz) * 50;
            puan -= VezirErkenHareketCezasi(tahta, Oyuncu.Siyah) * 30;
            puan += VezirErkenHareketCezasi(tahta, Oyuncu.Beyaz) * 30;
            puan += RokAvantajı(tahta, Oyuncu.Siyah) * 50;
            puan -= RokAvantajı(tahta, Oyuncu.Beyaz) * 50;
            puan -= SahErkenHareketCezasi(tahta, Oyuncu.Siyah) * 20;
            puan += SahErkenHareketCezasi(tahta, Oyuncu.Beyaz) * 20;

            Dictionary<TasTuru, int> tasDegerleri = new Dictionary<TasTuru, int>()
            {
                {TasTuru.Piyon, 100},
                {TasTuru.At, 320},
                {TasTuru.Fil, 330},
                {TasTuru.Kale, 500},
                {TasTuru.Vezir, 900},
                {TasTuru.Sah, 20000}
            };

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Tas tas = tahta[r, c];
                    if (tas != null)
                    {
                        int tasDegeri = tasDegerleri[tas.Tur];

                        if (tas.Tur == TasTuru.Piyon)
                        {
                            tasDegeri += (tas.Renk == Oyuncu.Beyaz) ? r * 10 : (7 - r) * 10;
                        }

                        if (tas.Renk == Oyuncu.Siyah)
                        {
                            puan += tasDegeri;
                        }
                        else
                        {
                            puan -= tasDegeri;
                        }
                    }
                }
            }

            puan += MerkezKontrolu(tahta, Oyuncu.Siyah) * 20;
            puan -= MerkezKontrolu(tahta, Oyuncu.Beyaz) * 20;
            puan += Gelistirme(tahta, Oyuncu.Siyah) * 10;
            puan -= Gelistirme(tahta, Oyuncu.Beyaz) * 10;
            puan -= SahGuvenligi(tahta, Oyuncu.Siyah) * 15;
            puan += SahGuvenligi(tahta, Oyuncu.Beyaz) * 15;

            return puan;
        }
        #endregion

        #region Merkez kontrolü
        private int MerkezKontrolu(Tahta tahta, Oyuncu oyuncu)
        {
            int puan = 0;
            for (int r = 3; r <= 4; r++)
            {
                for (int c = 3; c <= 4; c++)
                {
                    if (tahta[r, c] != null && tahta[r, c].Renk == oyuncu)
                    {
                        puan++;
                    }
                }
            }
            return puan;
        }
        #endregion

        #region Geliştirme
        private int Gelistirme(Tahta tahta, Oyuncu oyuncu)
        {
            int puan = 0;
            for (int c = 0; c < 8; c++)
            {
                if (oyuncu == Oyuncu.Beyaz)
                {
                    if (tahta[7, c] != null && (tahta[7, c].Tur == TasTuru.At || tahta[7, c].Tur == TasTuru.Fil))
                    {
                        puan--;
                    }
                }
                else
                {
                    if (tahta[0, c] != null && (tahta[0, c].Tur == TasTuru.At || tahta[0, c].Tur == TasTuru.Fil))
                    {
                        puan++;
                    }
                }
            }
            return puan;
        }
        #endregion

        #region Şah güvenliği
        private int SahGuvenligi(Tahta tahta, Oyuncu oyuncu)
        {
            int puan = 0;
            Pozisyon sahPozisyonu = tahta.TasBul(oyuncu, TasTuru.Sah);

            if (oyuncu == Oyuncu.Beyaz)
            {
                if (sahPozisyonu.Satir > 0 && sahPozisyonu.Sutun > 0 && tahta[sahPozisyonu.Satir - 1, sahPozisyonu.Sutun - 1] != null && tahta[sahPozisyonu.Satir - 1, sahPozisyonu.Sutun - 1].Renk == oyuncu && tahta[sahPozisyonu.Satir - 1, sahPozisyonu.Sutun - 1].Tur == TasTuru.Piyon)
                {
                    puan++;
                }
                if (sahPozisyonu.Satir > 0 && sahPozisyonu.Sutun < 7 && tahta[sahPozisyonu.Satir - 1, sahPozisyonu.Sutun + 1] != null && tahta[sahPozisyonu.Satir - 1, sahPozisyonu.Sutun + 1].Renk == oyuncu && tahta[sahPozisyonu.Satir - 1, sahPozisyonu.Sutun + 1].Tur == TasTuru.Piyon)
                {
                    puan++;
                }
            }
            else
            {
                if (sahPozisyonu.Satir < 7 && sahPozisyonu.Sutun > 0 && tahta[sahPozisyonu.Satir + 1, sahPozisyonu.Sutun - 1] != null && tahta[sahPozisyonu.Satir + 1, sahPozisyonu.Sutun - 1].Renk == oyuncu && tahta[sahPozisyonu.Satir + 1, sahPozisyonu.Sutun - 1].Tur == TasTuru.Piyon)
                {
                    puan++;
                }
                if (sahPozisyonu.Satir < 7 && sahPozisyonu.Sutun < 7 && tahta[sahPozisyonu.Satir + 1, sahPozisyonu.Sutun + 1] != null && tahta[sahPozisyonu.Satir + 1, sahPozisyonu.Sutun + 1].Renk == oyuncu && tahta[sahPozisyonu.Satir + 1, sahPozisyonu.Sutun + 1].Tur == TasTuru.Piyon)
                {
                    puan++;
                }
            }
            return puan;
        }
        #endregion

        private Dictionary<string, int> transpositionTable = new Dictionary<string, int>();

        #region Tahta konumunu temsil eden bir string döndüren fonksiyon
        private string TahtaKonumunuAl(Tahta tahta)
        {
            StringBuilder sb = new StringBuilder();
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Tas tas = tahta[r, c];
                    if (tas == null)
                    {
                        sb.Append("-");
                    }
                    else
                    {
                        sb.Append(tas.Renk == Oyuncu.Beyaz ? char.ToUpper(TasKarakteri(tas)) : TasKarakteri(tas));
                    }
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Stringe karakter döndürme
        private static char TasKarakteri(Tas tas)
        {
            return tas.Tur switch
            {
                TasTuru.Piyon => 'p',
                TasTuru.At => 'a',
                TasTuru.Kale => 'k',
                TasTuru.Fil => 'f',
                TasTuru.Vezir => 'v',
                TasTuru.Sah => 's',
                _ => '-'
            };
        }
        #endregion

        #region Hamleleri önbelleğe alan metod
        private void OnbellekHamleleri(IEnumerable<Hamle> hamleler)
        {
            hamleBellegi.Clear();
            foreach (Hamle hamle in hamleler)
            {
                hamleBellegi[hamle.ToPos] = hamle;
                hamleBellegi[hamle.FromPos] = hamle;
            }
        }
        #endregion

        #region Olası hamleleri vurgular
        private void VurgulamayiGoster()
        {
            Color color = Color.FromArgb(150, 255, 125, 125);
            foreach (var hamle in hamleBellegi.Values)
            {
                Vurgular[hamle.ToPos.Satir, hamle.ToPos.Sutun].Fill = new SolidColorBrush(color);
            }
        }
        #endregion

        #region Tüm vurguları gizler
        private void VurgulariGizle()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Vurgular[r, c].Fill = Brushes.Transparent;
                }
            }
        }
        #endregion

        #region Fare imlecini oyuncunun rengine göre ayarlar
        private void ImlecAyarla(Oyuncu oyuncu)
        {
            if (oyuncu == Oyuncu.Beyaz)
            {
                Cursor = SatrancImlecleri.BeyazImlec;
            }
            else
            {
                Cursor = SatrancImlecleri.SiyahImlec;
            }
        }
        #endregion

        #region Menü ekranında olup olmadığını kontrol eden metod
        private bool MenuEkrandaMi()
        {
            return MenuContainer.Content != null;
        }
        #endregion

        #region Durdurma menüsünü gösteren metod
        private void DurdurmaMenusunuGoster()
        {
            siyahSureSayaci.Stop();
            beyazSureSayaci.Stop();

            DurdurmaMenusu durdurmaMenusu = new DurdurmaMenusu(this);
            durdurmaMenusu.mainWindow = this;
            MenuContainer.Content = durdurmaMenusu;

            durdurmaMenusu.SecilenSecenek += (secenek, mw) =>
            {
                MenuContainer.Content = null;

                if (secenek == Secenek.AnaMenu)
                {
                    mw.AnaMenuyeDon();
                }
                else if (secenek == Secenek.Cikis)
                {
                    Application.Current.Shutdown();
                }
                else if (secenek == Secenek.DevamEt && !tasDuzenlemeModu)
                {
                    if (oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz)
                    {
                        beyazSureSayaci.Start();
                    }
                    else
                    {
                        siyahSureSayaci.Start();
                    }
                }
            };
        }
        #endregion

        #region Ana menüye dönen metod
        public void AnaMenuyeDon()
        {
            AnaMenu anaMenu = new AnaMenu();
            anaMenu.Show();
            this.Close();
        }
        #endregion

        #region Oyun bitiş ekranını gösteren metod
        private void OyunBitisiGoster()
        {
            OyunBitisMenusu oyunBitisMenusu = new OyunBitisMenusu(oyunDurumu);
            MenuContainer.Content = oyunBitisMenusu;

            oyunBitisMenusu.SeciliSecenek += secenek =>
            {
                if (secenek == Secenek.YenidenBaslat)
                {
                    MenuContainer.Content = null;
                    OyunuYenidenBaslat();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            };

            siyahSureSayaci.Stop();
            beyazSureSayaci.Stop();
        }
        #endregion

        #region Oyunu yeniden başlatan metod
        private void OyunuYenidenBaslat()
        {
            SecilmisPoz = null;
            VurgulariGizle();
            hamleBellegi.Clear();
            oyunDurumu = new OyunDurumu(Oyuncu.Beyaz, Tahta.Baslangic());
            TahtaCiz(oyunDurumu.Tahta);
            ImlecAyarla(oyunDurumu.MevcutOyuncu);

            beyazSureSayaci.Stop();
            siyahSureSayaci.Stop();

            beyazKalanSure = TimeSpan.FromMinutes(baslangicDakikasi);
            siyahKalanSure = TimeSpan.FromMinutes(baslangicDakikasi);

            if (!yapayZekaModu)
            {
                SiyahOyuncuText.VerticalAlignment = VerticalAlignment.Top;
                BeyazOyuncuText.VerticalAlignment = VerticalAlignment.Bottom;
                Grid.SetRow(SiyahOyuncuText, 1);
                Grid.SetRow(BeyazOyuncuText, 5);
            }

            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss");
            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss");
        }
        #endregion

        #region Açık olan taş seçme menüsünü kapatan metod
        private void TasSecmeMenusunuKapat()
        {
            if (acikTasSecmeMenusu != null)
            {
                if (acikTasSecmeMenusu.Parent is Popup popup)
                {
                    popup.Closed -= TasSecmeMenusu_Closed;
                    popup.IsOpen = false;
                    acikTasSecmeMenusu = null;
                }
                acikTasSecmeMenusu = null;
            }
        }
        #endregion
    }
}