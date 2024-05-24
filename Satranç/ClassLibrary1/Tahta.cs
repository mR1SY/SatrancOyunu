using SatrancMantigi.Taslar;

namespace SatrancMantigi
{
    //Bu sınıf tüm aktif taşları saklayacak ve çeşitli yardımcı metodlar saklayacak
    public class Tahta
    {
        #region Temel_Konumsal_Tanımlama
        //Taşları depolamak için dikdörtgen(çift boyutlu) bir dizi tanımlıyoruz.
        private readonly Tas[,] taslar = new Tas[8, 8];

        private readonly Dictionary<Oyuncu, Pozisyon> piyonAtlamaPozisyonlari = new Dictionary<Oyuncu, Pozisyon>()
        {
            //Bir oyuncu piyonlarından birini iki hamle hareket ettirdikten sonra atladığı konumu burada saklar bu atlanan konum geçerken alma(En Passant) olarak adlandırılır
            {Oyuncu.Beyaz, null },
            {Oyuncu.Siyah, null }
        };

        public Tas this[int satir, int sutun]
        {
            get { return taslar[satir, sutun]; }
            set { taslar[satir, sutun] = value; }
        }
        
        //Bir satır ve bir sütun veya bir konum nesnesi sağlayarak taşı belirli bir kareye alabilir ve ayarlayabiliriz.
        public Tas this[Pozisyon pozisyon]
        {
            get { return this[pozisyon.Satir, pozisyon.Sutun]; }
            set { this[pozisyon.Satir, pozisyon.Sutun] = value; }
        }

        public Pozisyon PiyonAtlamaPozisyonunuAl(Oyuncu oyuncu)
        {
            return piyonAtlamaPozisyonlari[oyuncu];
        }

        public void PiyonAtlamaPozisyonunuAyarla(Oyuncu oyuncu, Pozisyon poz)
        {
            piyonAtlamaPozisyonlari[oyuncu] = poz;
        }
        #endregion

        #region Tahta_Taşları_Yerleştirme
        //Bu metod taşların doğru şekilde kurulduğu bir tahta döndürüyor.
        public static Tahta Baslangic()
        {
            Tahta tahta = new Tahta(); //Boş tahta oluşturuyoruz
            tahta.BaslangicParcalariEkle(); //Tüm taşları metodla birlikte ekliyoruz
            return tahta; //Geri döndürüyoruz
        }
        
        //Bu metod içerisinde taşların konumsal yapılandırılmasını sağlıyoruz
        private void BaslangicParcalariEkle()
        {
            this[0, 0] = new Kale(Oyuncu.Siyah);
            this[0, 1] = new At(Oyuncu.Siyah);
            this[0, 2] = new Fil(Oyuncu.Siyah);
            this[0, 3] = new Vezir(Oyuncu.Siyah);
            this[0, 4] = new Sah(Oyuncu.Siyah);
            this[0, 5] = new Fil(Oyuncu.Siyah);
            this[0, 6] = new At(Oyuncu.Siyah);
            this[0, 7] = new Kale(Oyuncu.Siyah);

            this[7, 0] = new Kale(Oyuncu.Beyaz);
            this[7, 1] = new At(Oyuncu.Beyaz);
            this[7, 2] = new Fil(Oyuncu.Beyaz);
            this[7, 3] = new Vezir(Oyuncu.Beyaz);
            this[7, 4] = new Sah(Oyuncu.Beyaz);
            this[7, 5] = new Fil(Oyuncu.Beyaz);
            this[7, 6] = new At(Oyuncu.Beyaz);
            this[7, 7] = new Kale(Oyuncu.Beyaz);

            for (int c = 0; c < 8; c++)
            {
                this[1, c] = new Piyon(Oyuncu.Siyah);
                this[6, c] = new Piyon(Oyuncu.Beyaz);
            }
        }
        #endregion

        #region Taş_Tahtanın_İçindemi
        //Döndürülen konum tahtanın içinde mi kontrolü burada yapılıyor
        public static bool IcerideMi(Pozisyon pozisyon)
        {
            return pozisyon.Satir >= 0 && pozisyon.Satir < 8 && pozisyon.Sutun >= 0 && pozisyon.Sutun < 8;
        }
        #endregion

        #region Poziyon_Boşmu
        
        //Oynanacak pozisyon boş mu bunu kontrol ediyor
        public bool BosMu(Pozisyon pozisyon)
        {
            return this[pozisyon] == null;
        }
        #endregion

        #region Şah_Tehdit_Altındayken

        public IEnumerable<Pozisyon> TasPozisyonlari()
        {
            for(int r=0;r<8;r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    Pozisyon poz = new Pozisyon(r, c);

                    if (!BosMu(poz))
                    {
                        yield return poz;
                    }
                }
            }
        }


        //Bir taşı içeren tüm konumları elde etmek için bu kısmı yazıyoruz
        public IEnumerable<Pozisyon> TasPozisyonlariIcin(Oyuncu oyuncu)
        {
            //Yalnızca doğru renkleri içeren konumları seçeceğiz
            return TasPozisyonlari().Where(poz => this[poz].Renk == oyuncu);
        }


        //Oyuncuyu parametre alır ve ancak o oyuncunun şahı şahta ise true değerini döndürür. 
        public bool TehditAltinda(Oyuncu oyuncu)
        {
            //Bunu yapmak için rakibin taşlarından herhangi birinin oyuncunun şahını ele geçirip geçiremeyceğini kontrol ediyoruz

            return TasPozisyonlariIcin(oyuncu.Rakip()).Any(poz =>
                {
                    //Böylece rakip taşı alırız ve sonra çagırarak rakip şahı yakalayabilir
                    Tas tas = this[poz];
                    return tas.RakipSahiEleGecirilebilir(poz, this);
                });
        }

        //Son aşama olarak tahtayı kopyalamamız gerekiyor
        public Tahta Kopya()
        {
            //Önce yeni bir boş tahta oluşturuyoruz
            Tahta Kopya = new Tahta();
            
            //Sonra bir parça içeren tüm pozisyonlar üzerinde döngü yapıyoruz
            foreach (Pozisyon poz in TasPozisyonlari())
            {
                //Döngüden sonra taşları yeni tahtaya kopyalıyoruz
                Kopya[poz] = this[poz].Kopya();
            }
            //Kopyayı geri veriyoruz
            return Kopya;
        }
        #endregion

        #region Yetersiz_Taş_Toplam_Sayı
        //Bu metod tahtadaki tüm taşların muhasebesini döndürür
        public Sayma ParcaSayisi()
        {
            Sayma sayma = new Sayma();
            //Sayma sınıfından bir nesne oluşturuyourz ve her biri için tahtadaki tüm işgal edilen konumlar üzerinde döngü oluştururz

            foreach (Pozisyon poz in TasPozisyonlari())
            {
                //Parçayı alırız
                Tas tas = this[poz];
                
                //Ve döngüden sonra sayım tamamlandıktan sonra rengi ve türü için sayımı arttırırız
                sayma.Artis(tas.Renk, tas.Tur);
            }
            return sayma;
        }
        #endregion

        #region Yetersiz_Taş_Ana_Kalıp
        //Eğer tahtada kalan parçalar herhangi bir oyuncunun diğerini şah mat etmesi için yeterli değilse doğru değerini döndürür
        public bool YetersizMateryal()
        {
            Sayma sayma = ParcaSayisi();

            //Tahtadaki mevcut parçaları sayarak başlarız
            return SahVSSahMi(sayma) || SahFilVSSahMi(sayma) || SahAtVSSahMi(sayma) || SahFilVSSahFilMi(sayma);
        }
        #endregion

        #region Yetersiz_Taş_Şah_Vs_Şah

        //Bu metodda şaha şah mı kaldı diye kontrol ediyoruz
        private static bool SahVSSahMi(Sayma sayma)
        {
            //Eğer sona iki taş kalmışsa bunlar şahlardır kesinlikle
            return sayma.ToplamSayi == 2;
        }
        #endregion

        #region Yetersiz_Taş_Şah_Fil_Vs_Şah
        private static bool SahFilVSSahMi(Sayma sayma)
        {
            //Üç taş kalıp kalmadığını ve bunlardan birinin beyaz bir fil mi yoksa siyah bir fil mi olup olmadığını kontrol ediyoruz
            return sayma.ToplamSayi == 3 && (sayma.Beyaz(TasTuru.Fil) == 1 || sayma.Siyah(TasTuru.Fil) == 1);
        }
        #endregion

        #region Yetersiz_Taş_Şah_At_Vs_Şah
        private static bool SahAtVSSahMi(Sayma sayma)
        {
            //Üç taş kalıp kalmadığını ve bunlardan birinin beyaz bir at mi yoksa siyah bir at mı olup olmadığını kontrol ediyoruz
            return sayma.ToplamSayi == 3 && (sayma.Beyaz(TasTuru.At) == 1 || sayma.Siyah(TasTuru.At) == 1);
        }
        #endregion

        #region Yetersiz_Taş_Şah_Fil_Vs_Şah_Fil
        private bool SahFilVSSahFilMi(Sayma sayma)
        {
            //dört taş kalıp kalmadığını kontrol ediyoruz eğer kalmamışsa oyun devam eder yani false
            if (sayma.ToplamSayi != 4)
            {
                return false;
            }
            //Kalan taş türlerinden olan filler eğer 0' ya da 2'ye eşitse oyun yine devam eder yani false döner
            if (sayma.Beyaz(TasTuru.Fil) != 1 || sayma.Siyah(TasTuru.Fil) != 1)
            {
                return false;
            }

            //Son olarak karşılıklı bir şekilde ters veya ortak renkte birer file ve birer şaha sahip olma durumu beraberlik doğuracağı için gerekli false elemelerini yaptıktan sonra true değerini döndürüyoruz
            Pozisyon bFilPoz = TasBul(Oyuncu.Beyaz, TasTuru.Fil);
            Pozisyon sFilPoz = TasBul(Oyuncu.Siyah, TasTuru.Fil);

            return bFilPoz.KareRengi() == sFilPoz.KareRengi();
        }
        #endregion

        #region Tahtada_Taşın_Alt_Kısmındaki_Kare_Rengini_Bulma_Metodu

        private Pozisyon TasBul(Oyuncu renk, TasTuru tur)
        {
            return TasPozisyonlariIcin(renk).First(poz => this[poz].Tur == tur);
        }

        #endregion

        #region 3_Katlı_Tekrar_İçin_Ana_Metodlar
        private bool HareketEtmeyenSahVeKaleMi(Pozisyon sahPoz, Pozisyon kalePoz)
        {
            if (BosMu(sahPoz) || BosMu(kalePoz))
            {
                return false;
            }

            Tas sah = this[sahPoz];
            Tas kale = this[kalePoz];

            return sah.Tur == TasTuru.Sah && kale.Tur == TasTuru.Kale && !sah.Tasindi && !kale.Tasindi;
        }

        public bool RokHakkiSahKanadi(Oyuncu oyuncu)
        {
            return oyuncu switch
            {
                Oyuncu.Beyaz => HareketEtmeyenSahVeKaleMi(new Pozisyon(7, 4), new Pozisyon(7, 7)),
                Oyuncu.Siyah => HareketEtmeyenSahVeKaleMi(new Pozisyon(0, 4), new Pozisyon(0, 7)),
                _ => false
            };
        }

        public bool RokHakkiVezirKanadi(Oyuncu oyuncu)
        {
            return oyuncu switch
            {
                Oyuncu.Beyaz => HareketEtmeyenSahVeKaleMi(new Pozisyon(7, 4), new Pozisyon(7, 0)),
                Oyuncu.Siyah => HareketEtmeyenSahVeKaleMi(new Pozisyon(0, 4), new Pozisyon(0, 0)),
                _ => false
            };
        }

        private bool PiyonVarMi(Oyuncu oyuncu, Pozisyon[] piyonPozisyonlari, Pozisyon atlamaPoz)
        {
            foreach (Pozisyon poz in piyonPozisyonlari.Where(IcerideMi))
            {
                Tas tas = this[poz];
                if (tas == null || tas.Renk != oyuncu || tas.Tur != TasTuru.Piyon)
                {
                    continue;
                }

                EnPassant hamle = new EnPassant(poz, atlamaPoz);
                if (hamle.Yasal(this))
                {
                    return true;
                }
            }

            return false;
        }

        public bool EnPassantYakalayabilirMi(Oyuncu oyuncu)
        {
            Pozisyon atlamaPoz = PiyonAtlamaPozisyonunuAl(oyuncu.Rakip());

            if (atlamaPoz == null)
            {
                return false;
            }

            Pozisyon[] piyonPozisyonlari = oyuncu switch
            {
                Oyuncu.Beyaz => new Pozisyon[] { atlamaPoz + Yon.GuneyBati, atlamaPoz + Yon.GuneyDogu },
                Oyuncu.Siyah => new Pozisyon[] { atlamaPoz + Yon.KuzeyBati, atlamaPoz + Yon.KuzeyDogu },
                _ => Array.Empty<Pozisyon>()
            };

            return PiyonVarMi(oyuncu, piyonPozisyonlari, atlamaPoz);
        }
        #endregion

    }
}
