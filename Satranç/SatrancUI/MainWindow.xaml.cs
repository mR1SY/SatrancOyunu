using System.Text;
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
using SatrancMantigi;
using SatrancUI;
using System.Collections.Generic;
using SatrancMantigi.Taslar;
using System.Windows.Controls.Primitives;
using System.Linq; // Distinct() için eklendi

namespace SatrancUI
{
    public partial class MainWindow : Window
    {
        #region Genel_Pencere_Tanımlamaları
        //Görüntü kontrolleri için diziyi tanımlıyoruz
        private readonly Image[,] TaslarinResimleri = new Image[8, 8];

        //Bu kısımda vurgular için çift boyutlu bir dizi tanımlıyoruz
        private readonly Rectangle[,] Vurgular = new Rectangle[8, 8];
        private readonly Dictionary<Pozisyon, Hamle> hamleBellegi = new Dictionary<Pozisyon, Hamle>();

        //Önce mevcut oyuncu taşımak istediği parçaya tıklar. O parça seçilir ve konumu seçilen konumda saklanır. Ardından oyun durumuna sorarız, seçilen parçayı hareket ettiren bu hamleleri yapabilir, daha sonra bunlar önbellekte anahtar olarak saklanır ve bunları ekranda vurgu karelerini kullanarak gösteririz. Daha sonra vurgulardan birine tıklar, bu gerçekleştiğinde önbellekten karşılık gelen hamleyi alırız ve konumlandırırız.

        #region Vurgu_Örneği
        /*
                     Seçilen poz 
                  (0,3)(SiyahVezir)

                      hamlebelleği
               (1,4) ----->  (0,3) -> (1,4)
               (2,5) ----->  (0,3) -> (2,5)
               (3,6) ----->  (0,3) -> (3,6)
               (4,7) ----->  (0,3) -> (4,7)

        */
        #endregion

        private OyunDurumu oyunDurumu;
        private Pozisyon SecilmisPoz = null;

        #endregion

        //private bool layoutUpdatedEventEklendi = false; // Yeni değişken
        public bool tasDuzenlemeModu = false;
        private TasTuru secilenTasTuru = TasTuru.Piyon;

        private List<Rectangle> hamleVurgulari = new List<Rectangle>();

        // Zamanlayıcılar için değişkenler
        private DispatcherTimer siyahSureSayaci;
        private DispatcherTimer beyazSureSayaci;
        private TimeSpan siyahKalanSure = TimeSpan.FromMinutes(10);
        private TimeSpan beyazKalanSure = TimeSpan.FromMinutes(10);

        private TasSecmeMenusu acikTasSecmeMenusu = null;

        public bool yapayZekaModu = false;

        private void TasSecmeMenusu_Closed(object sender, EventArgs e)
        {
            // Popup kapandığında takip değişkenini sıfırla
            acikTasSecmeMenusu = null;
        }

        private void TasSecmeMenusunuKapat()
        {
            if (acikTasSecmeMenusu != null)
            {
                if (acikTasSecmeMenusu.Parent is Popup popup)
                {
                    popup.Closed -= TasSecmeMenusu_Closed; // Olay işleyicisini kaldır
                    popup.IsOpen = false; // Popup'ı kapat
                    acikTasSecmeMenusu = null; // Takip değişkenini sıfırla    
                }
                acikTasSecmeMenusu = null;
            }
        }
        public void KareyeTasEkle(Pozisyon poz, Oyuncu oyuncu, TasTuru tur)
        {
            oyunDurumu.Tahta[poz] = TasOlustur(oyuncu, tur); // tur parametresini kullanın
            TaslarinResimleri[poz.Satir, poz.Sutun].Source = Resimler.ResimAl(oyunDurumu.Tahta[poz]);
        }

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

        private void TasDuzenlemeModuBaslat()
        {
            // Zamanlayıcıları devre dışı bırak
            siyahSureSayaci.IsEnabled = false;
            beyazSureSayaci.IsEnabled = false;

            // Düzenleme modu için gerekli diğer ayarlamaları buraya ekleyin
            DevamEtButonu.Visibility = Visibility.Visible;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            TasSecmeMenusunuKapat(); // TasSecmeMenusu'nu kapat
        }


        #region Ana_Pencere_Load_Kısmı
        public MainWindow()
        {
            InitializeComponent();
            TahtayiBaslat();
            oyunDurumu = new OyunDurumu(Oyuncu.Beyaz, Tahta.Baslangic());
            oyunDurumu.HamleDosyasiniSil();

            if (tasDuzenlemeModu)
            {
                TasDuzenlemeModuBaslat();
            }
            else
            {
                oyunDurumu = new OyunDurumu(Oyuncu.Beyaz, Tahta.Baslangic());
                TahtaCiz(oyunDurumu.Tahta);
                VurgulariGizle(); // Başlangıçta vurguları temizle
            }
            this.Closed += MainWindow_Closed;

            siyahSureSayaci = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            siyahSureSayaci.Tick += SiyahSureSayaci_Tick;
            beyazSureSayaci = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            beyazSureSayaci.Tick += BeyazSureSayaci_Tick;

            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss");
            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss");
        }

        #endregion

        #region Zamanlayıcı_Olayları

        private void SiyahSureSayaci_Tick(object sender, EventArgs e)
        {
            siyahKalanSure -= TimeSpan.FromSeconds(1);
            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss");

            if (siyahKalanSure <= TimeSpan.Zero)
            {
                siyahSureSayaci.Stop();
                oyunDurumu.OyunuBitir(Oyuncu.Beyaz, BitisSebebi.SureDoldu);
                OyunBitisiGoster();
            }
        }
        private void BeyazSureSayaci_Tick(object sender, EventArgs e)
        {
            beyazKalanSure -= TimeSpan.FromSeconds(1);
            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss");

            if (beyazKalanSure == TimeSpan.Zero)
            {
                beyazSureSayaci.Stop();
                // Beyaz'ın süresi bittiğinde oyunu bitir ve sebebini belirt
                oyunDurumu.OyunuBitir(Oyuncu.Siyah, BitisSebebi.SureDoldu); // Oyuncu.Siyah kazandı, sebep: Süre Doldu
                OyunBitisiGoster();
            }
        }
        #endregion

        private void DurdurButonu_Click(object sender, RoutedEventArgs e)
        {
            siyahSureSayaci.Stop(); // siyahZamanlayici yerine siyahSureSayaci
            beyazSureSayaci.Stop(); // beyazZamanlayici yerine beyazSureSayaci
        }
        private void YenidenBaslatButonu_Click(object sender, RoutedEventArgs e)
        {
            // Zamanlayıcıları sıfırla ve başlat
            beyazKalanSure = TimeSpan.FromMinutes(10);
            siyahKalanSure = TimeSpan.FromMinutes(10);
            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss"); // BeyazSure yerine BeyazSureText
            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss"); // SiyahSure yerine SiyahSureText

            // Vurguları temizle
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Vurgular[r, c].Fill = Brushes.Transparent;
                }
            }

            beyazSureSayaci.Stop(); // beyazZamanlayici yerine beyazSureSayaci
            siyahSureSayaci.Stop(); // siyahZamanlayici yerine siyahSureSayaci
            OyunuYenidenBaslat();
        }

        private void DevamEtButonu_Click(object sender, RoutedEventArgs e)
        {
            // Şah sayısını kontrol et
            Sayma sayma = oyunDurumu.Tahta.ParcaSayisi();
            if (sayma.Beyaz(TasTuru.Sah) != 1 || sayma.Siyah(TasTuru.Sah) != 1)
            {
                MessageBox.Show("Her iki oyuncunun da bir şahı olmalı.");
                return; // Devam etme
            }

            // Vurguları temizle
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Vurgular[r, c].Fill = Brushes.Transparent;
                }
            }
            // Şahın tehdit altında olup olmadığını kontrol et
            if (oyunDurumu.Tahta.TehditAltinda(oyunDurumu.MevcutOyuncu) || oyunDurumu.Tahta.TehditAltinda(oyunDurumu.MevcutOyuncu.Rakip()))
            {
                MessageBox.Show("Şahınız tehdit altında.");
                return; // Devam etme
            }

            // Oyun bitiş durumunu kontrol et
            oyunDurumu.OyunBitisiKontrol();
            if (oyunDurumu.OyunBittiMi())
            {
                OyunBitisiGoster(); // Oyun bitiş ekranını göster
                return; // Devam etme
            }

            // Eğer herhangi bir hamle yapıldıysa veya yeterli taş yoksa süre sayaçlarını başlat
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
            // Taş düzenleme modundan çık
            tasDuzenlemeModu = false;
            // Düzenleme modundan çıkarken yapılması gereken diğer ayarlamaları buraya ekleyin
        }

        #region Görüntü_Kontrolleri
        //Burada her biri için tüm konumların üzerinden geçeceğiz
        private void TahtayiBaslat()
        {
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    //Burada parçalar için tüm görüntü kontrollerini oluşturuyoruz ve bunları hem parça ızgarasına hem de taslarınResimleri dizinimize ekliyoruz

                    Image image = new();  //Yeni bir görüntü nesnesi oluşturuyoruz
                    TaslarinResimleri[r, c] = image;  //Bunu iki boyutlu dizide saklıyoruz
                    TasIzgarasi.Children.Add(image);  //Children.Add: Grafik olarak eklmeye yarar

                    Rectangle vurgu = new Rectangle();//Burada oluşturduğumuz her konum için aynı seyi vurgularımız için yapacağız
                    Vurgular[r, c] = vurgu;//Vurgular dizisinde saklıyoruz
                    VurguIzgarasi.Children.Add(vurgu);//Ve burada bir grafik olarak vurguızgarasina ekliyoruz.
                }
            }
        }
        #endregion

        #region Resimleri_Tahtaya_Ekle(Son_aşama)
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
                    // LayoutUpdated olayını kullanarak tahta ve taşları döndür
                    TahtaIzgarasi.LayoutUpdated += (sender, e) =>
                    {
                        if (!yapayZekaModu) // Yapay zeka modunda dönmeyi engelle
                        {                            // Taşları döndür (siyah oyuncu sırası ise)
                            if (oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
                            {
                                for (int r = 0; r < 8; r++)
                                {
                                    for (int c = 0; c < 8; c++)
                                    {
                                        TaslarinResimleri[r, c].RenderTransform = new RotateTransform(180, TaslarinResimleri[r, c].ActualWidth / 2, TaslarinResimleri[r, c].ActualHeight / 2);
                                    }
                                }

                                TahtaIzgarasi.RenderTransform = new RotateTransform(180, TahtaIzgarasi.ActualWidth / 2, TahtaIzgarasi.ActualHeight / 2);
                            }
                            else
                            {
                                // Döndürmeyi sıfırla (beyaz oyuncu sırası ise)
                                for (int r = 0; r < 8; r++)
                                {
                                    for (int c = 0; c < 8; c++)
                                    {
                                        TaslarinResimleri[r, c].RenderTransform = null;
                                    }
                                }

                                TahtaIzgarasi.RenderTransform = null;
                            }
                        }
                    };
                    // Arkaplan resmini ayarla
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (tasDuzenlemeModu && e.Key == Key.Delete && SecilmisPoz != null)
            {
                oyunDurumu.Tahta[SecilmisPoz] = null;
                TaslarinResimleri[SecilmisPoz.Satir, SecilmisPoz.Sutun].Source = null;
                SecilmisPoz = null; // Silindikten sonra seçimi kaldırın
                VurgulariGizle(); // Vurgulamayı temizle
                for (int r = 0; r < 8; r++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        Vurgular[r, c].Fill = Brushes.Transparent;
                    }
                }
            }
            if (e.Key == Key.Escape)
            {
                if (MenuEkrandaMi() && MenuContainer.Content is DurdurmaMenusu) // Durdurma menüsü açıksa
                {
                    MenuContainer.Content = null; // Durdurma menüsünü kapat

                    // Süre sayaçlarını tekrar başlat (eğer taş düzenleme modunda değilse)
                    if (!tasDuzenlemeModu)
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
                }
                else if (!MenuEkrandaMi()) // Durdurma menüsü açık değilse
                {
                    DurdurmaMenusunuGoster(); // Durdurma menüsünü aç
                }
            }
        }
        private void DurdurmaMenusunuGoster()
        {
            // Süre sayaçlarını durdur
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
                    // Süre sayaçlarını tekrar başlat
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
        public void AnaMenuyeDon()
        {
            // Ana menüyü aç
            AnaMenu anaMenu = new AnaMenu();
            anaMenu.Show();

            this.Close();
        }

        #region Tahtaya_Tıklama
        //Bu metod oyuncu tahtada bir yere tıkladığında çağrılır. 
        private void TahtaIzgarasi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Menü açıkken tahtaya müdahaleyi engelle
            if (MenuEkrandaMi())
            {
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                {
                    return; // Menü açıksa sol veya sağ tıklama işlemlerini engelle
                }
            }

            Point point = e.GetPosition(TasIzgarasi);
            Pozisyon poz = KarePozisyona(point);

            if (yapayZekaModu && oyunDurumu.Tahta[poz]?.Renk == Oyuncu.Siyah)
            {
                return; // Yapay zekanın taşına tıklandıysa hiçbir işlem yapma
            }
            // Sadece sol tıklama için işlem yap
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (tasDuzenlemeModu)
                {
                    // Taş düzenleme modunda taşları seçme:
                    SecilmisPoz = poz; // Tıklanan kareyi seçilen pozisyon olarak ayarla
                    VurgulariGizle(); // Önceki vurguları temizle
                    Vurgular[SecilmisPoz.Satir, SecilmisPoz.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 255, 255, 0)); // Sarı vurgu

                    // (Ekstra) Hamleleri önbelleğe al (isteğe bağlı)
                    // OnbellekHamleleri(oyunDurumu.TaslarIcinYasalHamleler(poz, tasDuzenlemeModu)); 
                }
                else
                {
                    // Normal oyun modunda taşları seçme:
                    SecilenPozisyondanItibaren(poz);
                }
            }
            // Sağ tıklama ile taş düzenleme menüsünü aç
            // Sağ tıklama ile taş düzenleme menüsünü aç
            if (tasDuzenlemeModu && e.RightButton == MouseButtonState.Pressed)
            {
                Oyuncu oyuncu = !oyunDurumu.Tahta.BosMu(poz) ? oyunDurumu.Tahta[poz].Renk : oyunDurumu.MevcutOyuncu;

                // Açık olan TasSecmeMenusu'nu kapat
                TasSecmeMenusunuKapat();

                // Taş seçme menüsünü popup olarak aç
                TasSecmeMenusu tasSecmeMenusu = new TasSecmeMenusu(oyuncu, this);
                tasSecmeMenusu.TıklananPozisyon = poz;
                Popup popup = new Popup
                {
                    Child = tasSecmeMenusu,
                    IsOpen = true,
                    PlacementTarget = TasIzgarasi,
                    Placement = PlacementMode.MousePoint,
                    StaysOpen = false // StaysOpen özelliğini false yap
                };

                // Açık olan menüyü takip et
                acikTasSecmeMenusu = tasSecmeMenusu;

                popup.Closed += TasSecmeMenusu_Closed;

                tasSecmeMenusu.SecilenTas += tur =>
                {
                    popup.IsOpen = false;
                    secilenTasTuru = tur;
                    KareyeTasEkle(poz, oyuncu, tur);

                    // Menü kapandığında takip değişkenini sıfırla
                    acikTasSecmeMenusu = null;
                };
            }
        }
        private Pozisyon KarePozisyona(Point point)
        {
            double squareSize = TahtaIzgarasi.ActualWidth / 8;
            int satir = (int)(point.Y / squareSize);
            int sutun = (int)(point.X / squareSize);
            return new Pozisyon(satir, sutun);
        }

        //Metod bir kareye tıklandığında ve seçilen bir tas olmadığında çağrılır. Konum paremetresi tıklanan karedir. 
        private void SecilenPozisyondanItibaren(Pozisyon poz)
        {
            // Eğer tıklanan konum zaten seçili konumsa seçimi kaldır
            if (poz == SecilmisPoz)
            {
                SecilmisPoz = null;
                VurgulariGizle();
                return; // Metodu sonlandır
            }

            // Önceki vurguları temizleriz (kırmızı vurgular dahil)
            VurgulariGizle();

            // İlk önce bu karedeki taş için yasal hamleleri çağırırız
            IEnumerable<Hamle> hamleler = oyunDurumu.TaslarIcinYasalHamleler(poz, tasDuzenlemeModu);

            if (hamleler.Any())
            {
                // Seçilen konumu göz önünde bulundururuz.
                SecilmisPoz = poz;

                // Hamleleri önbelleğe alırız.
                OnbellekHamleleri(hamleler);

                if (!tasDuzenlemeModu)
                {
                    VurgulamayiGoster();
                }
                // İmleci ayarlarız.
                ImlecAyarla(oyunDurumu.MevcutOyuncu);

                // Seçilen kareyi vurgularız.
                Vurgular[SecilmisPoz.Satir, SecilmisPoz.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 255, 255, 0)); // Sarı vurgu
            }
            else
            {
                // Yasal hamle yoksa SecilenKonuma metodunu çağır
                SecilenKonuma(poz);
            }
        }

        private void SecilenKonuma(Pozisyon poz)
        {
            // Önceki vurguları temizleriz (kırmızı vurgular dahil)
            VurgulariGizle();

            // Tıklanan konum, seçili konumla aynıysa seçimi kaldır
            if (poz == SecilmisPoz)
            {
                SecilmisPoz = null;
                return; // Metodu sonlandır
            }

            if (SecilmisPoz != null && hamleBellegi.TryGetValue(poz, out Hamle hamle))
            {
                if (hamle != null && hamle.FromPos == SecilmisPoz) // hamle null değilse ve FromPos seçili pozisyona eşitse işlemleri yap
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
            // Seçili kareyi kaldırırız
            SecilmisPoz = null;
        }
        #endregion

        #region Terfi_Taşıma
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

        #region Hamleyi_Gerçekleştir_Ve_Tahtaya_Kaydet

        //Bu metod oyun durumuna şunu söyler: Verilen hamleyi gerçekleştirin.
        private async void TasimaHamlesi(Hamle hamle)
        {
            // Süre sayaçlarını değiştir (Yapay Zeka Modu)
            if (yapayZekaModu)
            {
                if (oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
                {
                    siyahSureSayaci.Stop();
                    beyazSureSayaci.Start();
                }
                else
                {
                    beyazSureSayaci.Stop();
                    siyahSureSayaci.Start();
                }
            }

            if (hamle != null)
            {
                // Düzenleme modunda hamle yapma
                if (tasDuzenlemeModu)
                {
                    // Taşı kaldırma
                    if (oyunDurumu.Tahta[hamle.FromPos] != null)
                    {
                        TaslarinResimleri[hamle.FromPos.Satir, hamle.FromPos.Sutun].Source = null;
                        oyunDurumu.Tahta[hamle.FromPos] = null;
                    }
                    // Taşı yerleştirme (eğer boş değilse)
                    if (oyunDurumu.Tahta[hamle.ToPos] != null)
                    {
                        TaslarinResimleri[hamle.ToPos.Satir, hamle.ToPos.Sutun].Source = null;
                        oyunDurumu.Tahta[hamle.ToPos] = null;
                    }
                    // Taşı yeni konuma yerleştir
                    oyunDurumu.Tahta[hamle.ToPos] = oyunDurumu.Tahta[hamle.FromPos]; // Taşı yeni konuma yerleştir
                    oyunDurumu.Tahta[hamle.FromPos] = null; // Taşı eski konumdan kaldır

                    if (!yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
                    {
                        TahtaIzgarasi.RenderTransform = new RotateTransform(180, TahtaIzgarasi.ActualWidth / 2, TahtaIzgarasi.ActualHeight / 2);
                    }
                    else
                    {
                        TahtaIzgarasi.RenderTransform = null;
                    }

                    // TahtaIzgarasi'nin layoutunu güncelle, bu da TahtaCiz metodunu tetikleyecek
                    TahtaIzgarasi.UpdateLayout();

                    TaslarinResimleri[hamle.ToPos.Satir, hamle.ToPos.Sutun].Source = Resimler.ResimAl(oyunDurumu.Tahta[hamle.ToPos]);

                    // Hamle yapıldıktan sonra seçimi kaldır
                    SecilmisPoz = null;
                    VurgulariGizle(); // Vurgulamayı temizle

                    return;
                }
                if (!tasDuzenlemeModu)
                {
                    Vurgular[hamle.FromPos.Satir, hamle.FromPos.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 0, 255, 0));
                    Vurgular[hamle.ToPos.Satir, hamle.ToPos.Sutun].Fill = new SolidColorBrush(Color.FromArgb(150, 0, 255, 0));
                }
                oyunDurumu.HareketEt(hamle);
                //Ve değişiklikleri yansıtacak şekilde tahtayı güncelle.
                TahtaCiz(oyunDurumu.Tahta);
                ImlecAyarla(oyunDurumu.MevcutOyuncu);

                oyunDurumu.HamleyiKaydet(hamle);

                // Yasal hamleleri yeniden hesapla ve önbelleğe al
                OnbellekHamleleri(oyunDurumu.TaslarIcinYasalHamleler(hamle.ToPos));

                if (yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah)
                {
                    Hamle yapayZekaHamlesi = await Task.Run(() => YapayZekaHamlesiHesapla());

                    // Yapay zeka hamlesi null değilse gerçekleştir
                    if (yapayZekaHamlesi != null)
                    {
                        TasimaHamlesi(yapayZekaHamlesi);
                    }
                }
                // Süre sayaclarını değiştir
                if (oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz)
                {
                    siyahSureSayaci.Stop();
                    beyazSureSayaci.Start();
                }
                else
                {
                    beyazSureSayaci.Stop();
                    siyahSureSayaci.Start();
                }
                if (oyunDurumu.MevcutOyuncu == Oyuncu.Beyaz && !yapayZekaModu) // Sıra beyaza geçtiyse
                {
                    SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss");
                    BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss");
                    SiyahSureText.VerticalAlignment = VerticalAlignment.Bottom;
                    SiyahOyuncuText.VerticalAlignment = VerticalAlignment.Top;
                    BeyazSureText.VerticalAlignment = VerticalAlignment.Top;
                    BeyazOyuncuText.VerticalAlignment = VerticalAlignment.Bottom;
                    Grid.SetRow(SiyahSureText, 0);
                    Grid.SetRow(SiyahOyuncuText, 1);
                    Grid.SetRow(BeyazSureText, 6);
                    Grid.SetRow(BeyazOyuncuText, 5);
                }
                else if (oyunDurumu.MevcutOyuncu == Oyuncu.Siyah && !yapayZekaModu) // Sıra siyaha geçtiyse
                {
                    SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss");
                    BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss");
                    SiyahSureText.VerticalAlignment = VerticalAlignment.Top;
                    SiyahOyuncuText.VerticalAlignment = VerticalAlignment.Bottom;
                    BeyazSureText.VerticalAlignment = VerticalAlignment.Bottom;
                    BeyazOyuncuText.VerticalAlignment = VerticalAlignment.Top;
                    Grid.SetRow(BeyazSureText, 0);
                    Grid.SetRow(BeyazOyuncuText, 1);
                    Grid.SetRow(SiyahSureText, 6);
                    Grid.SetRow(SiyahOyuncuText, 5);
                }

                if (yapayZekaModu && oyunDurumu.MevcutOyuncu == Oyuncu.Siyah) // varsayalım ki yapay zeka siyah taşlarla oynuyor
                {
                    // Yapay zeka hamlesini hesapla
                    Hamle yapayZekaHamlesi = YapayZekaHamlesiHesapla(); // Bu metodu daha sonra yazacağız

                    // Yapay zeka hamlesini gerçekleştir
                    TasimaHamlesi(yapayZekaHamlesi);
                }
                //Her hamle oynandığında oyun bittimi diye kontrol ediyoruz
                if (oyunDurumu.OyunBittiMi())
                {
                    OyunBitisiGoster();
                }
            }
        }
        private Hamle YapayZekaHamlesiHesapla()
        {
            var yasalHamleler = oyunDurumu.ButunYasalHamlelerIcin(Oyuncu.Siyah).ToList();

            if (yasalHamleler.Count == 0)
            {
                return null;
            }

            int derinlik = 4;
            int enIyiDeger = int.MinValue;
            Hamle enIyiHamle = null;

            // Hamleleri sırala
            yasalHamleler = yasalHamleler.OrderByDescending(h =>
            {
                // Önce taş yakalayan hamleleri sırala
                if (h.TasSilindi)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }).ToList();

            foreach (Hamle hamle in yasalHamleler)
            {
                Tahta yeniTahta = oyunDurumu.Tahta.Kopya();
                hamle.Execute(yeniTahta);
                int deger = Minimax(yeniTahta, derinlik - 1, false, int.MinValue, int.MaxValue);
                if (deger > enIyiDeger)
                {
                    enIyiDeger = deger;
                    enIyiHamle = hamle;
                }
            }

            return enIyiHamle;
        }
        private int VezirErkenHareketCezasi(Tahta tahta, Oyuncu oyuncu)
        {
            if (oyuncu == Oyuncu.Beyaz)
            {
                if (tahta[7, 3] == null || tahta[7, 3].Tasindi)
                {
                    return 1; // Beyaz vezir hareket etmişse ceza
                }
            }
            else // oyuncu == Oyuncu.Siyah
            {
                if (tahta[0, 3] == null || tahta[0, 3].Tasindi)
                {
                    return 1; // Siyah vezir hareket etmişse ceza
                }
            }

            return 0; // Vezir hareket etmemişse ceza verme
        }

        private int VezirGuvenligi(Tahta tahta, Oyuncu oyuncu)
        {
            Pozisyon vezirPozisyonu = tahta.TasBul(oyuncu, TasTuru.Vezir);

            // Vezir tehdit altında ise cezalandır
            if (tahta.TehditAltinda(vezirPozisyonu, oyuncu))
            {
                return -1;
            }

            return 1; // Vezir güvende ise puan ver
        }

        private int SahErkenHareketCezasi(Tahta tahta, Oyuncu oyuncu)
        {
            if (oyuncu == Oyuncu.Beyaz)
            {
                if (tahta[7, 4] == null || tahta[7, 4].Tasindi)
                {
                    return 1; // Beyaz şah hareket etmişse ceza
                }
            }
            else // oyuncu == Oyuncu.Siyah
            {
                if (tahta[0, 4] == null || tahta[0, 4].Tasindi)
                {
                    return 1; // Siyah şah hareket etmişse ceza
                }
            }

            return 0; // Şah hareket etmemişse ceza verme
        }

        private int RokAvantajı(Tahta tahta, Oyuncu oyuncu)
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

        private int DegerlendirmeFonksiyonu(Tahta tahta)
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
        private Dictionary<string, int> transpositionTable = new Dictionary<string, int>();

        private int Minimax(Tahta tahta, int derinlik, bool maximizingPlayer, int alfa, int beta)
        {
            string tahtaKonumu = TahtaKonumunuAl(tahta);

            // Konum daha önce değerlendirildiyse, değeri transposition table'dan al
            if (transpositionTable.ContainsKey(tahtaKonumu))
            {
                return transpositionTable[tahtaKonumu];
            }

            OyunDurumu geçiciOyunDurumu = new OyunDurumu(maximizingPlayer ? Oyuncu.Siyah : Oyuncu.Beyaz, tahta);

            if (derinlik == 0 || geçiciOyunDurumu.OyunBittiMi())
            {
                return DegerlendirmeFonksiyonu(tahta);
            }

            var yasalHamleler = geçiciOyunDurumu.ButunYasalHamlelerIcin(maximizingPlayer ? Oyuncu.Siyah : Oyuncu.Beyaz).ToList();

            if (yasalHamleler.Count == 0)
            {
                return DegerlendirmeFonksiyonu(tahta);
            }

            // Hamleleri sırala (maximizingPlayer için azalan, minimizingPlayer için artan sırada)
            yasalHamleler = maximizingPlayer
                ? yasalHamleler.OrderByDescending(h => h.TasSilindi ? 1 : 0).ToList()
                : yasalHamleler.OrderBy(h => h.TasSilindi ? 1 : 0).ToList();

            if (maximizingPlayer)
            {
                int maxEval = int.MinValue;
                foreach (Hamle hamle in yasalHamleler)
                {
                    Tahta yeniTahta = tahta.Kopya();
                    hamle.Execute(yeniTahta);
                    int eval = Minimax(yeniTahta, derinlik - 1, false, alfa, beta);
                    maxEval = Math.Max(maxEval, eval);
                    alfa = Math.Max(alfa, eval);
                    if (beta <= alfa)
                    {
                        break; // Beta budaması
                    }
                }

                // Hesaplanan değeri transposition table'a kaydet
                transpositionTable[tahtaKonumu] = maxEval;

                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                foreach (Hamle hamle in yasalHamleler)
                {
                    Tahta yeniTahta = tahta.Kopya();
                    hamle.Execute(yeniTahta);
                    int eval = Minimax(yeniTahta, derinlik - 1, true, alfa, beta);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alfa)
                    {
                        break; // Alfa budaması
                    }
                }

                // Hesaplanan değeri transposition table'a kaydet
                transpositionTable[tahtaKonumu] = minEval;

                return minEval;
            }
        }

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

        #region Seçilen_Parça_İçin_Yasal_Hamlelerin_Toplanması_Ve_Saklanması

        //Seçilen parça için yasal hamlelerin tolanması ve bunları önbellekte saklamak burada yapılır.
        private void OnbellekHamleleri(IEnumerable<Hamle> hamleler)
        {
            // Önbellekteki her şeyi boşaltıyoruz boşaltıyoruz.
            hamleBellegi.Clear();

            // Ardından verilen hamleler üzerinden döngü yapıyoruz
            foreach (Hamle hamle in hamleler)
            {
                // Hedef konumu (ToPos) ve başlangıç konumunu (FromPos) önbelleğe ekle
                hamleBellegi[hamle.ToPos] = hamle;
                hamleBellegi[hamle.FromPos] = hamle;
            }
        }

        #endregion

        #region Vurgulama_Yöntemini_Göster
        private void VurgulamayiGoster()
        {
            //FromArgb(Alfa(şeffaflık),kırmızı, yeşil, mavi)
            Color color = Color.FromArgb(150, 255, 125, 125);
            //150,125,255,125

            //Tüm konumlar için rengi döndürüyoruz.
            foreach (var hamle in hamleBellegi.Values)
            {
                Vurgular[hamle.ToPos.Satir, hamle.ToPos.Sutun].Fill = new SolidColorBrush(color);
            }
        }

        #endregion

        #region Vurguluma_Yöntemini_Gizle
        private void VurgulariGizle()
        {
            // Tüm vurguları temizle
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Vurgular[r, c].Fill = Brushes.Transparent;
                }
            }
        }
        #endregion

        #region İmleç_Oluşturma
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

        #region Menü_Ekranda_Mı?
        //Oyun sonlarında çıkacağı için varsayılan olarak sıfır değerini alır
        private bool MenuEkrandaMi()
        {
            return MenuContainer.Content != null;
        }
        #endregion

        #region Oyun_Bitiş_Menüsünü_Göster
        private void OyunBitisiGoster()
        {
            //Bir nesne oluşturarak menüyü çağırıyoruz
            OyunBitisMenusu oyunBitisMenusu = new OyunBitisMenusu(oyunDurumu);
            MenuContainer.Content = oyunBitisMenusu;


            oyunBitisMenusu.SeciliSecenek += secenek =>
            {
                if (secenek == Secenek.YenidenBaslat)
                {
                    //Eğer oyuncu yeniden başlat komutunu seçerse menü ekrandan kaldırır ve oyun yeniden başlatılır
                    MenuContainer.Content = null;
                    OyunuYenidenBaslat();
                }
                else
                {
                    //Eğer oyuncu yeniden başlay yerine çıkışa basarsa Application.Current.Shutdown() metodu ile oyun kapatılır
                    Application.Current.Shutdown();
                }
            };
            // Zamanlayıcıları durdur
            siyahSureSayaci.Stop();
            beyazSureSayaci.Stop();
        }
        #endregion

        #region Yeniden_Başlat_Metodu
        private void OyunuYenidenBaslat()
        {
            SecilmisPoz = null;
            //Öncelikle tüm vurguları gizliyoruz gizleme metodu ile
            VurgulariGizle();
            //Hamle belleğini temizliyoruz
            hamleBellegi.Clear();
            //İlk tahta kurulumuyla yeni bir oyun durumu yaratıyoruz
            oyunDurumu = new OyunDurumu(Oyuncu.Beyaz, Tahta.Baslangic());
            //Tahtayı çiziyoruz
            TahtaCiz(oyunDurumu.Tahta);
            //Ve imlecin doğru renge sahip olduğundan emin oluyoruz
            ImlecAyarla(oyunDurumu.MevcutOyuncu);

            beyazSureSayaci.Stop();
            siyahSureSayaci.Stop();

            beyazKalanSure = TimeSpan.FromMinutes(10);
            siyahKalanSure = TimeSpan.FromMinutes(10);
            BeyazSureText.Text = beyazKalanSure.ToString(@"mm\:ss");
            SiyahSureText.Text = siyahKalanSure.ToString(@"mm\:ss");
        }
        #endregion

    }
}
