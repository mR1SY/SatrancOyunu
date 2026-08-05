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
        private TimeSpan siyahKalanSure = TimeSpan.FromMinutes(10); // Siyah oyuncunun kalan süresi (varsayılan: 10 dakika).
        private TimeSpan beyazKalanSure = TimeSpan.FromMinutes(10); // Beyaz oyuncunun kalan süresi (varsayılan: 10 dakika).

        private TasSecmeMenusu acikTasSecmeMenusu = null; // Açık olan taş seçme menüsü nesnesine referans (açık menü yoksa null).

        public bool yapayZekaModu = false; // Yapay zeka modunun aktif olup olmadığını belirten bool değişkeni (varsayılan: false).
        //public static string AktifStockfishYolu = "";
        #endregion

        #region Yapıcı metod
        public MainWindow() // MainWindow penceresini oluşturan yapıcı metod.
        {
            InitializeComponent(); // Pencere bileşenlerini başlatır.
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
                // Sol veya sağ fare tuşuna basılırsa...
                {
                    return; // Metodu sonlandırır (menü açıkken tahtaya müdahaleyi engeller).
                }
            }

            Point point = e.GetPosition(TasIzgarasi); // Fare imlecinin tahta ızgarasına göre konumunu alır.
            Pozisyon poz = KarePozisyona(point); // Fare imlecinin konumunu satranç tahtası koordinatlarına dönüştürür.

            if (yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah) // Yapay zeka modu aktifse ve tıklanan kare siyah oyuncuya aitse...
            {
                return; // Metodu sonlandırır (yapay zekanın taşına tıklanmasını engeller).
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
                // Tıklanan karede taş varsa taşın rengini alır, yoksa mevcut oyuncunun rengini alır.

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
            if (tasDuzenlemeModu && e.Key == Key.Delete && SecilmisPoz != null) // Taş düzenleme modunda ise ve Delete tuşuna basıldıysa ve bir kare seçiliyse...
            {
                oyunDurumu.Tahta[SecilmisPoz] = null; // Seçili karedeki taşı siler.
                TaslarinResimleri[SecilmisPoz.Satir, SecilmisPoz.Sutun].Source = null; // Taşın görüntüsünü temizler.
                SecilmisPoz = null; // Seçili kareyi sıfırlar.
                VurgulariGizle(); // Vurguları gizler.
                for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
                {
                    for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
                    {
                        Vurgular[r, c].Fill = Brushes.Transparent; // Tüm vurguları temizler.
                    }
                }
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
                oyunDurumu.OyunuBitir(Oyuncu.Beyaz, BitisSebebi.SureDoldu); // Oyunu Beyaz oyuncunun kazanmasıyla bitirir (sebep: Süre Doldu).
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
                oyunDurumu.OyunuBitir(Oyuncu.Siyah, BitisSebebi.SureDoldu); // Oyunu Siyah oyuncunun kazanmasıyla bitirir (sebep: Süre Doldu).
                OyunBitisiGoster(); // Oyun bitiş ekranını gösterir.
            }
        }
        #endregion

        #region "Durdur" butonuna tıklandığında çalışacak metod
        private void DurdurButonu_Click(object sender, RoutedEventArgs e) // "Durdur" butonuna tıklandığında çalışacak metod.
        {
            siyahSureSayaci.Stop(); // Siyah oyuncunun süresini durdurur.
            beyazSureSayaci.Stop(); // Beyaz oyuncunun süresini durdurur.
        }
        #endregion

        #region "Yeniden Başlat" butonuna tıklandığında çalışacak metod
        private void YenidenBaslatButonu_Click(object sender, RoutedEventArgs e) // "Yeniden Başlat" butonuna tıklandığında çalışacak metod.
        {
            beyazKalanSure = TimeSpan.FromMinutes(10); // Beyaz oyuncunun süresini sıfırlar (10 dakika).
            siyahKalanSure = TimeSpan.FromMinutes(10); // Siyah oyuncunun süresini sıfırlar (10 dakika).
            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss"); // Beyaz oyuncunun kalan süresini ekrana yazar.
            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss"); // Siyah oyuncunun kalan süresini ekrana yazar.

            for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
            {
                for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
                {
                    Vurgular[r, c].Fill = Brushes.Transparent; // Tüm vurguları temizler.
                }
            }

            beyazSureSayaci.Stop(); // Beyaz oyuncunun süresini durdurur.
            siyahSureSayaci.Stop(); // Siyah oyuncunun süresini durdurur.
            OyunuYenidenBaslat(); // Oyunu yeniden başlatır.
        }
        #endregion

        #region "Devam Et" butonuna tıklandığında çalışacak metod
        private void DevamEtButonu_Click(object sender, RoutedEventArgs e) // "Devam Et" butonuna tıklandığında çalışacak metod.
        {
            Sayma sayma = oyunDurumu.Tahta.ParcaSayisi(); // Tahtadaki taşların sayısını alır.
            if (sayma.Beyaz(TasTuru.Sah) != 1 || sayma.Siyah(TasTuru.Sah) != 1) // Her iki tarafta da birer şah yoksa...
            {
                MessageBox.Show("Her iki oyuncunun da bir şahı olmalı."); // Hata mesajı gösterir.
                return; // Metodu sonlandırır.
            }

            for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
            {
                for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
                {
                    Vurgular[r, c].Fill = Brushes.Transparent; // Tüm vurguları temizler.
                }
            }

            if (oyunDurumu.Tahta.TehditAltinda(oyunDurumu.MevcutOyuncu) || oyunDurumu.Tahta.TehditAltinda(oyunDurumu.MevcutOyuncu.Rakip()))
            // Şahlardan biri tehdit altında ise...
            {
                MessageBox.Show("Şahınız tehdit altında."); // Hata mesajı gösterir.
                return; // Metodu sonlandırır.
            }

            oyunDurumu.OyunBitisiKontrol(); // Oyun bitiş koşullarını kontrol eder.
            if (oyunDurumu.OyunBittiMi()) // Oyun bittiyse...
            {
                OyunBitisiGoster(); // Oyun bitiş ekranını gösterir.
                return; // Metodu sonlandırır.
            }

            if (oyunDurumu.Tahta.ParcaSayisi().ToplamSayi > 2 && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
            // Tahtada 2'den fazla taş varsa ve sıra siyah oyuncu daysa...
            {
                siyahSureSayaci.IsEnabled = true; // Siyah oyuncunun süresini başlatır.
            }

            if (oyunDurumu.Tahta.ParcaSayisi().ToplamSayi > 2 && oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz)
            // Tahtada 2'den fazla taş varsa ve sıra beyaz oyuncu daysa...
            {
                beyazSureSayaci.IsEnabled = true; // Beyaz oyuncunun süresini başlatır.
            }

            if (tasDuzenlemeModu) // Taş düzenleme modunda ise...
            {
                beyazSureSayaci.Stop(); // Beyaz oyuncunun süresini durdurur.
                siyahSureSayaci.Stop(); // Siyah oyuncunun süresini durdurur.
            }

            DevamEtButonu.Visibility = Visibility.Collapsed; // "Devam Et" butonunu gizler.
            tasDuzenlemeModu = false; // Taş düzenleme modunu kapatır.
        }
        #endregion

        #region Satranç tahtasını başlatan metod
        private void TahtayiBaslat() // Satranç tahtasını başlatan metod.
        {
            for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
            {
                for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
                {
                    Image image = new(); // Yeni bir Image nesnesi oluşturur.
                    TaslarinResimleri[r, c] = image; // Image nesnesini diziye ekler.
                    TasIzgarasi.Children.Add(image); // Image nesnesini tahta ızgarasına ekler.

                    Rectangle vurgu = new Rectangle(); // Yeni bir Rectangle nesnesi oluşturur (vurgu için).
                    Vurgular[r, c] = vurgu; // Rectangle nesnesini diziye ekler.
                    VurguIzgarasi.Children.Add(vurgu); // Rectangle nesnesini vurgu ızgarasına ekler.
                }
            }
        }
        #endregion

        #region Tahtayı çizen metod
        private void TahtaCiz(Tahta tahta) // Tahtayı çizen metod.
        {
            for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
            {
                for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
                {
                    Tas tas = tahta[r, c]; // Pozisyondaki taşı alır.

                    if (tas != null && tas.Tur == TasTuru.Sah && oyunDurumu.Tahta.TehditAltinda(tas.Renk))
                    // Taş şah ise ve tehdit altında ise...
                    {
                        if (tas.Renk == Oyuncu.Beyaz) // Şah beyaz ise...
                        {
                            TaslarinResimleri[r, c].Source = Resimler.ResimYukle("Assets/BeyazSahTehditAltinda.png"); // Tehdit altındaki beyaz şah görüntüsünü ayarlar.
                        }
                        else // Şah siyah ise...
                        {
                            TaslarinResimleri[r, c].Source = Resimler.ResimYukle("Assets/SiyahSahTehditAltinda.png"); // Tehdit altındaki siyah şah görüntüsünü ayarlar.
                        }
                    }
                    else // Taş şah değilse veya tehdit altında değilse...
                    {
                        TaslarinResimleri[r, c].Source = Resimler.ResimAl(tas); // Taşın normal görüntüsünü ayarlar.
                    }

                    TahtaIzgarasi.LayoutUpdated += (sender, e) => // Tahta ızgarasının düzeni güncellendiğinde çalışacak olay işleyicisi.
                    {
                        if (!yapayZekaModu) // Yapay zeka modu aktif değilse...
                        {
                            if (oyunDurumu.MevcutOyuncu == Oyuncu.Siyah) // Sıra siyah oyuncu daysa...
                            {
                                for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
                                {
                                    for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
                                    {
                                        TaslarinResimleri[r, c].RenderTransform = new RotateTransform(180, TaslarinResimleri[r, c].ActualWidth / 2, TaslarinResimleri[r, c].ActualHeight / 2);
                                        // Taş görüntülerini 180 derece döndürür.
                                    }
                                }

                                TahtaIzgarasi.RenderTransform = new RotateTransform(180, TahtaIzgarasi.ActualWidth / 2, TahtaIzgarasi.ActualHeight / 2);
                                // Tahta ızgarasını 180 derece döndürür.
                            }
                            else // Sıra beyaz oyuncu daysa...
                            {
                                for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
                                {
                                    for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
                                    {
                                        TaslarinResimleri[r, c].RenderTransform = null; // Taş görüntülerinin döndürme efektini kaldırır.
                                    }
                                }

                                TahtaIzgarasi.RenderTransform = null; // Tahta ızgarasının döndürme efektini kaldırır.
                            }
                        }
                    };

                    if (!yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah) // Yapay zeka modu aktif değilse ve sıra siyah oyuncu daysa...
                    {
                        TahtaIzgarasi.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Tahta_180.png")));
                        // Tahta arkaplanını 180 derece döndürülmüş olarak ayarlar.
                    }
                    else // Yapay zeka modu aktifse veya sıra beyaz oyuncu daysa...
                    {
                        TahtaIzgarasi.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Assets/Tahta.png")));
                        // Tahta arkaplanını normal olarak ayarlar.
                    }
                }
            }
        }
        #endregion

        #region Taş düzenleme modunu başlatan metod
        private void TasDuzenlemeModuBaslat() // Taş düzenleme modunu başlatan metod.
        {
            siyahSureSayaci.IsEnabled = false; // Siyah oyuncunun süresini durdurur.
            beyazSureSayaci.IsEnabled = false; // Beyaz oyuncunun süresini durdurur.

            DevamEtButonu.Visibility = Visibility.Visible; // "Devam Et" butonunu görünür yapar.
        }
        #endregion

        #region Verilen pozisyona verilen renkte ve türde bir taş ekleyen metod
        public void KareyeTasEkle(Pozisyon poz, Oyuncu oyuncu, TasTuru tur) // Verilen pozisyona verilen renkte ve türde bir taş ekleyen metod.
        {
            oyunDurumu.Tahta[poz] = TasOlustur(oyuncu, tur); // Taşı oluşturur ve tahtaya yerleştirir.
            TaslarinResimleri[poz.Satir, poz.Sutun].Source = Resimler.ResimAl(oyunDurumu.Tahta[poz]); // Taşın görüntüsünü ayarlar.
        }
        #endregion

        #region Verilen renkte ve türde bir taş oluşturan metod
        private Tas TasOlustur(Oyuncu renk, TasTuru tur) // Verilen renkte ve türde bir taş oluşturan metod.
        {
            return tur switch // Taş türüne göre taş nesnesi oluşturur.
            {
                TasTuru.Piyon => new Piyon(renk), // Piyon
                TasTuru.At => new At(renk), // At
                TasTuru.Kale => new Kale(renk), // Kale
                TasTuru.Fil => new Fil(renk), // Fil
                TasTuru.Vezir => new Vezir(renk), // Vezir
                TasTuru.Sah => new Sah(renk), // Şah
                _ => null // Diğer durumlarda null
            };
        }
        #endregion

        #region Fare imlecinin konumunu satranç tahtası koordinatlarına dönüştüren metod
        private Pozisyon KarePozisyona(Point point) // Fare imlecinin konumunu satranç tahtası koordinatlarına dönüştüren metod.
        {
            double squareSize = TahtaIzgarasi.ActualWidth / 8; // Karenin boyutunu hesaplar.
            int satir = (int)(point.Y / squareSize); // Satır numarasını hesaplar.
            int sutun = (int)(point.X / squareSize); // Sütun numarasını hesaplar.
            return new Pozisyon(satir, sutun); // Pozisyon nesnesi oluşturur ve döndürür.
        }
        #endregion

        #region Seçili kareye göre hamleleri hesaplar ve vurgular
        private void SecilenPozisyondanItibaren(Pozisyon poz) // Seçili kareye göre hamleleri hesaplar ve vurgular.
        {
            if (poz == SecilmisPoz) // Tıklanan kare zaten seçili kare ise...
            {
                SecilmisPoz = null; // Seçili kareyi sıfırlar.
                VurgulariGizle(); // Vurguları gizler.
                return; // Metodu sonlandırır.
            }

            VurgulariGizle(); // Vurguları gizler.

            IEnumerable<Hamle> hamleler = oyunDurumu.TaslarIcinYasalHamleler(poz, tasDuzenlemeModu); // Seçili karedeki taş için yasal hamleleri alır.

            if (hamleler.Any()) // Yasal hamle varsa...
            {
                SecilmisPoz = poz; // Tıklanan kareyi seçili kare olarak ayarlar.
                OnbellekHamleleri(hamleler); // Hamleleri önbelleğe alır.

                if (!tasDuzenlemeModu) // Taş düzenleme modunda değilse...
                {
                    VurgulamayiGoster(); // Olası hamleleri vurgular.
                }

                ImlecAyarla(oyunDurumu.MevcutOyuncu); // Fare imlecini mevcut oyuncunun rengine göre ayarlar.
                Vurgular[SecilmisPoz.Satir, SecilmisPoz.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 255, 255, 0)); // Seçili kareyi sarı renkle vurgular.
            }
            else // Yasal hamle yoksa...
            {
                SecilenKonuma(poz); // Tıklanan kareyi seçili kare olarak ayarlar (taş yoksa).
            }
        }
        #endregion

        #region Tıklanan kareyi seçili kare olarak ayarlar (taş yoksa)
        private void SecilenKonuma(Pozisyon poz) // Tıklanan kareyi seçili kare olarak ayarlar (taş yoksa).
        {
            VurgulariGizle(); // Vurguları gizler.

            if (poz == SecilmisPoz) // Tıklanan kare zaten seçili kare ise...
            {
                SecilmisPoz = null; // Seçili kareyi sıfırlar.
                return; // Metodu sonlandırır.
            }

            if (SecilmisPoz != null && hamleBellegi.TryGetValue(poz, out Hamle hamle)) // Seçili bir kare varsa ve tıklanan kare bir hamle hedefi ise...
            {
                if (hamle != null && hamle.FromPos == SecilmisPoz) // Hamle null değilse ve başlangıç karesi seçili kareye eşitse...
                {
                    if (hamle.Tur == HamleTuru.PiyonTerfi) // Hamle piyon terfi ise...
                    {
                        TerfiTasima(hamle.FromPos, hamle.ToPos); // Piyon terfi işlemini başlatır.
                    }
                    else // Hamle piyon terfi değilse...
                    {
                        TasimaHamlesi(hamle); // Hamleyi gerçekleştirir.
                    }
                }
            }

            SecilmisPoz = null; // Seçili kareyi sıfırlar.
        }
        #endregion

        #region Piyon terfi işlemini başlatan metod
        private void TerfiTasima(Pozisyon from, Pozisyon to) // Piyon terfi işlemini başlatan metod.
        {
            TaslarinResimleri[to.Satir, to.Sutun].Source = Resimler.ResimAl(oyunDurumu.MevcutOyuncu, TasTuru.Piyon); // Hedef kareye piyon görüntüsünü yerleştirir (geçici).
            TaslarinResimleri[to.Satir, to.Sutun].Source = null; // Hedef karedeki görüntüyü temizler.

            TerfiMenusu trfMenusu = new TerfiMenusu(oyunDurumu.MevcutOyuncu); // Yeni bir piyon terfi menüsü oluşturur.
            MenuContainer.Content = trfMenusu; // Piyon terfi menüsünü içerik alanına yerleştirir.

            trfMenusu.SecilenTas += tur => // Piyon terfi menüsünden bir taş seçildiğinde çalışacak olay işleyicisi.
            {
                MenuContainer.Content = null; // Piyon terfi menüsünü kapatır.
                Hamle trfHamlesi = new PiyonTerfi(from, to, tur); // Seçilen taş türüne göre piyon terfi hamlesi oluşturur.
                TasimaHamlesi(trfHamlesi); // Piyon terfi hamlesini gerçekleştirir.
            };
        }
        #endregion


        private readonly Dictionary<Pozisyon, Hamle> hamleBellegi = new Dictionary<Pozisyon, Hamle>(); // Olası hamleleri önbelleğe almak için kullanılır.

        #region Hamleyi gerçekleştiren metod
        private async void TasimaHamlesi(Hamle hamle) // Hamleyi gerçekleştiren metod.
        {
            if (yapayZekaModu) // Yapay zeka modu aktifse...
            {
                if (oyunDurumu.MevcutOyuncu == Oyuncu.Siyah) // Sıra siyah oyuncu daysa...
                {
                    siyahSureSayaci.Stop(); // Siyah oyuncunun süresini durdurur.
                    beyazSureSayaci.Start(); // Beyaz oyuncunun süresini başlatır.
                }
                else // Sıra beyaz oyuncu daysa...
                {
                    beyazSureSayaci.Stop(); // Beyaz oyuncunun süresini durdurur.
                    siyahSureSayaci.Start(); // Siyah oyuncunun süresini başlatır.
                }
            }

            if (hamle != null) // Hamle null değilse...
            {
                if (tasDuzenlemeModu) // Taş düzenleme modunda ise...
                {
                    if (oyunDurumu.Tahta[hamle.FromPos] != null) // Başlangıç karesinde taş varsa...
                    {
                        TaslarinResimleri[hamle.FromPos.Satir, hamle.FromPos.Sutun].Source = null; // Taşın görüntüsünü temizler.
                        oyunDurumu.Tahta[hamle.FromPos] = null; // Taşı tahtadan kaldırır.
                    }

                    if (oyunDurumu.Tahta[hamle.ToPos] != null) // Hedef karede taş varsa...
                    {
                        TaslarinResimleri[hamle.ToPos.Satir, hamle.ToPos.Sutun].Source = null; // Taşın görüntüsünü temizler.
                        oyunDurumu.Tahta[hamle.ToPos] = null; // Taşı tahtadan kaldırır.
                    }

                    oyunDurumu.Tahta[hamle.ToPos] = oyunDurumu.Tahta[hamle.FromPos]; // Taşı hedef kareye taşır.
                    oyunDurumu.Tahta[hamle.FromPos] = null; // Taşı başlangıç karesinden kaldırır.

                    if (!yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah) // Yapay zeka modu aktif değilse ve sıra siyah oyuncu daysa...
                    {
                        TahtaIzgarasi.RenderTransform = new RotateTransform(180, TahtaIzgarasi.ActualWidth / 2, TahtaIzgarasi.ActualHeight / 2);
                        // Tahta ızgarasını 180 derece döndürür.
                    }
                    else // Yapay zeka modu aktifse veya sıra beyaz oyuncu daysa...
                    {
                        TahtaIzgarasi.RenderTransform = null; // Tahta ızgarasının döndürme efektini kaldırır.
                    }

                    TahtaIzgarasi.UpdateLayout(); // Tahta ızgarasının düzenini günceller.
                    TaslarinResimleri[hamle.ToPos.Satir, hamle.ToPos.Sutun].Source = Resimler.ResimAl(oyunDurumu.Tahta[hamle.ToPos]); // Taşın görüntüsünü ayarlar.

                    SecilmisPoz = null; // Seçili kareyi sıfırlar.
                    VurgulariGizle(); // Vurguları gizler.
                    return; // Metodu sonlandırır.
                }

                oyunDurumu.HareketEt(hamle); // Hamleyi gerçekleştirir ve oyun durumunu günceller.
                VurgulariGizle(); // Vurguları gizler.

                if (!tasDuzenlemeModu) // Taş düzenleme modunda değilse...
                {
                    Vurgular[hamle.FromPos.Satir, hamle.FromPos.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 0, 255, 0)); // Başlangıç karesini yeşil renkle vurgular.
                    Vurgular[hamle.ToPos.Satir, hamle.ToPos.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 0, 255, 0)); // Hedef kareyi yeşil renkle vurgular.
                }

                TahtaCiz(oyunDurumu.Tahta); // Tahtayı çizer.
                ImlecAyarla(oyunDurumu.MevcutOyuncu); // Fare imlecini mevcut oyuncunun rengine göre ayarlar.
                oyunDurumu.HamleyiKaydet(hamle); // Hamleyi txt dosyasına kaydeder.
                OnbellekHamleleri(oyunDurumu.TaslarIcinYasalHamleler(hamle.ToPos)); // Yeni yasal hamleleri önbelleğe alır.

                if (yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah) // Yapay zeka modu aktifse ve sıra siyah oyuncu daysa...
                {
                    Hamle yapayZekaHamlesi = await Task.Run(() => YapayZekaHamlesiHesapla()); // Yapay zekanın hamlesini hesaplar.

                    if (yapayZekaHamlesi != null) // Yapay zeka bir hamle bulduysa...
                    {
                        TasimaHamlesi(yapayZekaHamlesi); // Yapay zekanın hamlesini gerçekleştirir.
                    }
                }

                if (oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz) // Sıra beyaz oyuncu daysa...
                {
                    siyahSureSayaci.Stop(); // Siyah oyuncunun süresini durdurur.
                    beyazSureSayaci.Start(); // Beyaz oyuncunun süresini başlatır.
                }
                else // Sıra siyah oyuncu daysa...
                {
                    beyazSureSayaci.Stop(); // Beyaz oyuncunun süresini durdurur.
                    siyahSureSayaci.Start(); // Siyah oyuncunun süresini başlatır.
                }

                if (oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz && !yapayZekaModu) // Sıra beyaz oyuncu daysa ve yapay zeka modu aktif değilse...
                {
                    SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss"); // Siyah oyuncunun kalan süresini ekrana yazar.
                    BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss"); // Beyaz oyuncunun kalan süresini ekrana yazar.
                    SiyahSureText.VerticalAlignment = VerticalAlignment.Bottom; // Siyah oyuncunun süresini alt tarafa hizalar.
                    SiyahOyuncuText.VerticalAlignment = VerticalAlignment.Top; // Siyah oyuncu etiketini üst tarafa hizalar.
                    BeyazSureText.VerticalAlignment = VerticalAlignment.Top; // Beyaz oyuncunun süresini üst tarafa hizalar.
                    BeyazOyuncuText.VerticalAlignment = VerticalAlignment.Bottom; // Beyaz oyuncu etiketini alt tarafa hizalar.
                    Grid.SetRow(SiyahSureText, 0); // Siyah oyuncunun süresini 0. satıra yerleştirir.
                    Grid.SetRow(SiyahOyuncuText, 1); // Siyah oyuncu etiketini 1. satıra yerleştirir.
                    Grid.SetRow(BeyazSureText, 6); // Beyaz oyuncunun süresini 6. satıra yerleştirir.
                    Grid.SetRow(BeyazOyuncuText, 5); // Beyaz oyuncu etiketini 5. satıra yerleştirir.
                }
                else if (oyunDurumu.MevcutOyuncu == Oyuncu.Siyah && !yapayZekaModu) // Sıra siyah oyuncu daysa ve yapay zeka modu aktif değilse...
                {
                    SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss"); // Siyah oyuncunun kalan süresini ekrana yazar.
                    BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss"); // Beyaz oyuncunun kalan süresini ekrana yazar.
                    SiyahSureText.VerticalAlignment = VerticalAlignment.Top; // Siyah oyuncunun süresini üst tarafa hizalar.
                    SiyahOyuncuText.VerticalAlignment = VerticalAlignment.Bottom; // Siyah oyuncu etiketini alt tarafa hizalar.
                    BeyazSureText.VerticalAlignment = VerticalAlignment.Bottom; // Beyaz oyuncunun süresini alt tarafa hizalar.
                    BeyazOyuncuText.VerticalAlignment = VerticalAlignment.Top; // Beyaz oyuncu etiketini üst tarafa hizalar.
                    Grid.SetRow(BeyazSureText, 0); // Beyaz oyuncunun süresini 0. satıra yerleştirir.
                    Grid.SetRow(BeyazOyuncuText, 1); // Beyaz oyuncu etiketini 1. satıra yerleştirir.
                    Grid.SetRow(SiyahSureText, 6); // Siyah oyuncunun süresini 6. satıra yerleştirir.
                    Grid.SetRow(SiyahOyuncuText, 5); // Siyah oyuncu etiketini 5. satıra yerleştirir.
                }

                if (oyunDurumu.OyunBittiMi()) // Oyun bittiyse...
                {
                    OyunBitisiGoster(); // Oyun bitiş ekranını gösterir.
                }
            }
        }
        #endregion

        #region Yapay zekanın hamlesini hesaplayan metod
        private Hamle YapayZekaHamlesiHesapla() // Yapay zekanın hamlesini hesaplayan metod.
        {
            var yasalHamleler = oyunDurumu.ButunYasalHamlelerIcin(Oyuncu.Siyah).ToList(); // Siyah oyuncunun yasal hamlelerini alır.

            if (yasalHamleler.Count == 0) // Yasal hamle yoksa...
            {
                return null; // Null döndürür.
            }
            string stockfishYolu=AnaMenu.AktifStockfishYolu;
            string enIyiHamleMetni = "";
            using (System.Diagnostics.Process motor = new System.Diagnostics.Process())
            {
                motor.StartInfo.FileName = stockfishYolu;
                motor.StartInfo.UseShellExecute = false;
                motor.StartInfo.RedirectStandardInput = true;
                motor.StartInfo.RedirectStandardOutput = true;
                motor.StartInfo.CreateNoWindow = true;
                motor.Start();
                string fen = oyunDurumu.TahtaDurumunuFenYap();
                motor.StandardInput.WriteLine($"position fen {fen}");
                motor.StandardInput.WriteLine("go movetime 1000");
                while (true)
                {
                    string cikti = motor.StandardOutput.ReadLine();
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
            return enIyiHamle; // En iyi hamleyi döndürür.
        }
        #endregion

        #region Vezirin erken hareketini cezalandıran değerlendirme fonksiyonu
        private int VezirErkenHareketCezasi(Tahta tahta, Oyuncu oyuncu) // Vezirin erken hareketini cezalandıran değerlendirme fonksiyonu.
        {
            if (oyuncu == Oyuncu.Beyaz)
            {
                if (tahta[7, 3] == null || tahta[7, 3].Tasindi)
                {
                    return 1; // Beyaz vezir hareket etmişse ceza puanı verir.
                }
            }
            else // oyuncu == Oyuncu.Siyah
            {
                if (tahta[0, 3] == null || tahta[0, 3].Tasindi)
                {
                    return 1; // Siyah vezir hareket etmişse ceza puanı verir.
                }
            }

            return 0; // Vezir hareket etmemişse ceza verme.
        }
        #endregion

        #region Vezirin güvenliğini değerlendiren değerlendirme fonksiyonu
        private int VezirGuvenligi(Tahta tahta, Oyuncu oyuncu) // Vezirin güvenliğini değerlendiren değerlendirme fonksiyonu.
        {
            Pozisyon vezirPozisyonu = tahta.TasBul(oyuncu, TasTuru.Vezir);

            // Vezir tehdit altında ise cezalandır
            if (tahta.TehditAltinda(vezirPozisyonu, oyuncu))
            {
                return -1;
            }

            return 1; // Vezir güvende ise puan ver
        }
        #endregion

        #region Şahın erken hareketini cezalandıran değerlendirme fonksiyonu
        private int SahErkenHareketCezasi(Tahta tahta, Oyuncu oyuncu) // Şahın erken hareketini cezalandıran değerlendirme fonksiyonu.
        {
            if (oyuncu == Oyuncu.Beyaz)
            {
                if (tahta[7, 4] == null || tahta[7, 4].Tasindi)
                {
                    return 1; // Beyaz şah hareket etmişse ceza puanı verir.
                }
            }
            else // oyuncu == Oyuncu.Siyah
            {
                if (tahta[0, 4] == null || tahta[0, 4].Tasindi)
                {
                    return 1; // Siyah şah hareket etmişse ceza puanı verir.
                }
            }

            return 0; // Şah hareket etmemişse ceza verme.
        }
        #endregion

        #region Rok yapılıp yapılmadığını değerlendiren değerlendirme fonksiyonu
        private int RokAvantajı(Tahta tahta, Oyuncu oyuncu) // Rok yapılıp yapılmadığını değerlendiren değerlendirme fonksiyonu.
        {
            if (oyuncu == Oyuncu.Beyaz)
            {
                // Beyaz şah hareket etmişse ve rok yapmamışsa cezalandır
                if (tahta[7, 4] == null || tahta[7, 4].Tasindi)
                {
                    return -1;
                }
                // Beyaz rok yapmışsa puan ver
                if ((tahta[7, 6] != null && tahta[7, 6].Tur == TasTuru.Sah) ||
                    (tahta[7, 2] != null && tahta[7, 2].Tur == TasTuru.Sah))
                {
                    return 1;
                }
            }
            else // oyuncu == Oyuncu.Siyah
            {
                // Siyah şah hareket etmişse ve rok yapmamışsa cezalandır
                if (tahta[0, 4] == null || tahta[0, 4].Tasindi)
                {
                    return -1;
                }
                // Siyah rok yapmışsa puan ver
                if ((tahta[0, 6] != null && tahta[0, 6].Tur == TasTuru.Sah) ||
                    (tahta[0, 2] != null && tahta[0, 2].Tur == TasTuru.Sah))
                {
                    return 1;
                }
            }

            return 0; // Rok durumu yoksa puan verme
        }
        #endregion

        #region Tahtanın değerini hesaplayan değerlendirme fonksiyonu
        private int DegerlendirmeFonksiyonu(Tahta tahta) // Tahtanın değerini hesaplayan değerlendirme fonksiyonu.
        {
            int puan = 0;

            // Vezirin güvenliğini değerlendirme
            puan += VezirGuvenligi(tahta, Oyuncu.Siyah) * 50;
            puan -= VezirGuvenligi(tahta, Oyuncu.Beyaz) * 50;

            puan -= VezirErkenHareketCezasi(tahta, Oyuncu.Siyah) * 30;
            puan += VezirErkenHareketCezasi(tahta, Oyuncu.Beyaz) * 30;

            puan += RokAvantajı(tahta, Oyuncu.Siyah) * 50; // Rok yapılmışsa puanı artır
            puan -= RokAvantajı(tahta, Oyuncu.Beyaz) * 50;

            // Şahın erken hareketlerini cezalandırma
            puan -= SahErkenHareketCezasi(tahta, Oyuncu.Siyah) * 20;
            puan += SahErkenHareketCezasi(tahta, Oyuncu.Beyaz) * 20;

            // Taş değerleri
            Dictionary<TasTuru, int> tasDegerleri = new Dictionary<TasTuru, int>()
    {
        {TasTuru.Piyon, 100},
        {TasTuru.At, 320},
        {TasTuru.Fil, 330},
        {TasTuru.Kale, 500},
        {TasTuru.Vezir, 900},
        {TasTuru.Sah, 20000}
    };

            // Tahtadaki tüm taşlar için puanı hesapla
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Tas tas = tahta[r, c];
                    if (tas != null)
                    {
                        int tasDegeri = tasDegerleri[tas.Tur];

                        // Taş konumuna göre puan ayarlaması (örnek)
                        if (tas.Tur == TasTuru.Piyon)
                        {
                            // Piyonlar ilerledikçe değer kazansın
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

            // Merkez kontrolü (örnek)
            puan += MerkezKontrolu(tahta, Oyuncu.Siyah) * 20;
            puan -= MerkezKontrolu(tahta, Oyuncu.Beyaz) * 20;

            // Geliştirme (örnek)
            puan += Gelistirme(tahta, Oyuncu.Siyah) * 10;
            puan -= Gelistirme(tahta, Oyuncu.Beyaz) * 10;

            // Şah güvenliği (örnek)
            puan -= SahGuvenligi(tahta, Oyuncu.Siyah) * 15;
            puan += SahGuvenligi(tahta, Oyuncu.Beyaz) * 15;

            return puan;
        }
        #endregion

        #region Merkez kontrolü
        private int MerkezKontrolu(Tahta tahta, Oyuncu oyuncu)
        {
            int puan = 0;
            // Merkez kareler (d4, e4, d5, e5)
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
            // At ve fillerin geliştirilmesi
            for (int c = 0; c < 8; c++)
            {
                if (oyuncu == Oyuncu.Beyaz)
                {
                    if (tahta[7, c] != null && (tahta[7, c].Tur == TasTuru.At || tahta[7, c].Tur == TasTuru.Fil))
                    {
                        puan--; // Beyaz taşlar başlangıç konumlarında kalmış
                    }
                }
                else // oyuncu == Oyuncu.Siyah
                {
                    if (tahta[0, c] != null && (tahta[0, c].Tur == TasTuru.At || tahta[0, c].Tur == TasTuru.Fil))
                    {
                        puan++; // Siyah taşlar başlangıç konumlarında kalmış
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

            // Şahın etrafındaki piyonlar
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
            else // oyuncu == Oyuncu.Siyah
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
        // Tahta konumunu temsil eden bir string döndüren fonksiyon
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
                        sb.Append("-"); // Boş kare
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
                _ => '-' // Boş kare
            };
        }
        #endregion

        #region Hamleleri önbelleğe alan metod
        private void OnbellekHamleleri(IEnumerable<Hamle> hamleler) // Hamleleri önbelleğe alan metod.
        {
            hamleBellegi.Clear(); // Hamle belleğini temizler.

            foreach (Hamle hamle in hamleler) // Hamleler üzerinde döngü yapar.
            {
                hamleBellegi[hamle.ToPos] = hamle; // Hedef kareyi anahtar, hamleyi değer olarak önbelleğe ekler.
                hamleBellegi[hamle.FromPos] = hamle; // Başlangıç kareyi anahtar, hamleyi değer olarak önbelleğe ekler.
            }
        }
        #endregion

        #region Olası hamleleri vurgular
        private void VurgulamayiGoster() // Olası hamleleri vurgular.
        {
            Color color = Color.FromArgb(150, 255, 125, 125); // Vurgu rengi (açık kırmızı).

            foreach (var hamle in hamleBellegi.Values) // Önbelleğe alınmış hamleler üzerinde döngü yapar.
            {
                Vurgular[hamle.ToPos.Satir, hamle.ToPos.Sutun].Fill = new SolidColorBrush(color); // Hedef kareleri vurgular.
            }
        }
        #endregion

        #region Tüm vurguları gizler
        private void VurgulariGizle() // Tüm vurguları gizler.
        {
            for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
            {
                for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
                {
                    Vurgular[r, c].Fill = Brushes.Transparent; // Karelerin dolgu rengini şeffaf yapar.
                }
            }
        }
        #endregion

        #region Fare imlecini oyuncunun rengine göre ayarlar
        private void ImlecAyarla(Oyuncu oyuncu) // Fare imlecini oyuncunun rengine göre ayarlar.
        {
            if (oyuncu == Oyuncu.Beyaz) // Oyuncu beyaz ise...
            {
                Cursor = SatrancImlecleri.BeyazImlec; // Beyaz imleci ayarlar.
            }
            else // Oyuncu siyah ise...
            {
                Cursor = SatrancImlecleri.SiyahImlec; // Siyah imleci ayarlar.
            }
        }
        #endregion

        #region Menü ekranında olup olmadığını kontrol eden metod
        private bool MenuEkrandaMi() // Menü ekranında olup olmadığını kontrol eden metod.
        {
            return MenuContainer.Content != null; // İçerik alanında bir içerik varsa true döner.
        }
        #endregion

        #region Durdurma menüsünü gösteren metod
        private void DurdurmaMenusunuGoster() // Durdurma menüsünü gösteren metod.
        {
            siyahSureSayaci.Stop(); // Siyah oyuncunun süresini durdurur.
            beyazSureSayaci.Stop(); // Beyaz oyuncunun süresini durdurur.

            DurdurmaMenusu durdurmaMenusu = new DurdurmaMenusu(this); // Yeni bir DurdurmaMenusu nesnesi oluşturur.
            durdurmaMenusu.mainWindow = this; // Ana oyun penceresi referansını ayarlar.
            MenuContainer.Content = durdurmaMenusu; // Durdurma menüsünü içerik alanına yerleştirir.

            durdurmaMenusu.SecilenSecenek += (secenek, mw) => // Menüden bir seçenek seçildiğinde çalışacak olay işleyicisi.
            {
                MenuContainer.Content = null; // Durdurma menüsünü kapatır.

                if (secenek == Secenek.AnaMenu) // Seçilen seçenek "Ana Menü" ise...
                {
                    mw.AnaMenuyeDon(); // Ana menüye döner.
                }
                else if (secenek == Secenek.Cikis) // Seçilen seçenek "Çıkış" ise...
                {
                    Application.Current.Shutdown(); // Uygulamayı kapatır.
                }
                else if (secenek == Secenek.DevamEt && !tasDuzenlemeModu) // Seçilen seçenek "Devam Et" ise ve taş düzenleme modunda değilse...
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
            };
        }
        #endregion

        #region Ana menüye dönen metod
        public void AnaMenuyeDon() // Ana menüye dönen metod.
        {
            AnaMenu anaMenu = new AnaMenu(); // Yeni bir AnaMenu penceresi oluşturur.
            anaMenu.Show(); // Ana menü penceresini gösterir.

            this.Close(); // Ana oyun penceresini kapatır.
        }
        #endregion

        #region Oyun bitiş ekranını gösteren metod
        private void OyunBitisiGoster() // Oyun bitiş ekranını gösteren metod.
        {
            OyunBitisMenusu oyunBitisMenusu = new OyunBitisMenusu(oyunDurumu); // Yeni bir oyun bitiş menüsü oluşturur.
            MenuContainer.Content = oyunBitisMenusu; // Oyun bitiş menüsünü içerik alanına yerleştirir.

            oyunBitisMenusu.SeciliSecenek += secenek => // Menüden bir seçenek seçildiğinde çalışacak olay işleyicisi.
            {
                if (secenek == Secenek.YenidenBaslat) // Seçilen seçenek "Yeniden Başlat" ise...
                {
                    MenuContainer.Content = null; // Oyun bitiş menüsünü kapatır.
                    OyunuYenidenBaslat(); // Oyunu yeniden başlatır.
                }
                else // Seçilen seçenek "Çıkış" ise...
                {
                    Application.Current.Shutdown(); // Uygulamayı kapatır.
                }
            };

            siyahSureSayaci.Stop(); // Siyah oyuncunun süresini durdurur.
            beyazSureSayaci.Stop(); // Beyaz oyuncunun süresini durdurur.
        }
        #endregion

        #region Oyunu yeniden başlatan metod
        private void OyunuYenidenBaslat() // Oyunu yeniden başlatan metod.
        {
            SecilmisPoz = null; // Seçili kareyi sıfırlar.
            VurgulariGizle(); // Vurguları gizler.
            hamleBellegi.Clear(); // Hamle belleğini temizler.
            oyunDurumu = new OyunDurumu(Oyuncu.Beyaz, Tahta.Baslangic()); // Yeni bir oyun durumu oluşturur.
            TahtaCiz(oyunDurumu.Tahta); // Tahtayı çizer.
            ImlecAyarla(oyunDurumu.MevcutOyuncu); // Fare imlecini mevcut oyuncunun rengine göre ayarlar.

            beyazSureSayaci.Stop(); // Beyaz oyuncunun süresini durdurur.
            siyahSureSayaci.Stop(); // Siyah oyuncunun süresini durdurur.

            beyazKalanSure = TimeSpan.FromMinutes(10); // Beyaz oyuncunun süresini sıfırlar (10 dakika).
            siyahKalanSure = TimeSpan.FromMinutes(10); // Siyah oyuncunun süresini sıfırlar (10 dakika).
            SiyahOyuncuText.VerticalAlignment = VerticalAlignment.Top; // Siyah oyuncu etiketini üst tarafa hizalar.
            BeyazOyuncuText.VerticalAlignment = VerticalAlignment.Bottom; // Beyaz oyuncu etiketini alt tarafa hizalar.
            Grid.SetRow(SiyahOyuncuText, 1); // Siyah oyuncu etiketini 1. satıra yerleştirir.
            Grid.SetRow(BeyazOyuncuText, 5); // Beyaz oyuncu etiketini 5. satıra yerleştirir.
            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss"); // Beyaz oyuncunun kalan süresini ekrana yazar.
            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss"); // Siyah oyuncunun kalan süresini ekrana yazar.
        }
        #endregion

        #region Açık olan taş seçme menüsünü kapatan metod
        private void TasSecmeMenusunuKapat() // Açık olan taş seçme menüsünü kapatan metod.
        {
            if (acikTasSecmeMenusu != null) // Açık bir taş seçme menüsü varsa...
            {
                if (acikTasSecmeMenusu.Parent is Popup popup) // Taş seçme menüsü bir Popup içindeyse...
                {
                    popup.Closed -= TasSecmeMenusu_Closed; // Closed olay işleyicisini kaldırır.
                    popup.IsOpen = false; // Popup'ı kapatır.
                    acikTasSecmeMenusu = null; // Açık taş seçme menüsü referansını null olarak ayarlar.
                }
                acikTasSecmeMenusu = null; // Açık taş seçme menüsü referansını null olarak ayarlar.
            }
        }
        #endregion
    }
}