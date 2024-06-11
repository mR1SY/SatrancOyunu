using SatrancMantigi.Taslar;

namespace SatrancMantigi
{
    // Satranç tahtasını ve üzerindeki taşları temsil eden sınıf.
    public class Tahta
    {
        #region Tanımlamalar
        private readonly Tas[,] taslar = new Tas[8, 8]; // 8x8 boyutunda bir dizi, satranç taşlarını tutar.
        #endregion

        #region Her oyuncu için en passant yakalama pozisyonunu tutar
        private readonly Dictionary<Oyuncu, Pozisyon> piyonAtlamaPozisyonlari = new Dictionary<Oyuncu, Pozisyon>()
        // Her oyuncu için en passant yakalama pozisyonunu tutar.
        {
            {Oyuncu.Beyaz, null }, // Beyaz oyuncu için en passant yakalama pozisyonu (başlangıçta null).
            {Oyuncu.Siyah, null }  // Siyah oyuncu için en passant yakalama pozisyonu (başlangıçta null).
        };
        #endregion

        #region Verilen satır ve sütun numaralarındaki taşı döndüren indeksleyici
        public Tas this[int satir, int sutun] // Verilen satır ve sütun numaralarındaki taşı döndüren indeksleyici.
        {
            get { return taslar[satir, sutun]; } // Taşı döndürür.
            set { taslar[satir, sutun] = value; } // Taşı ayarlar.
        }
        #endregion

        #region Verilen pozisyondaki taşı döndüren indeksleyici
        public Tas this[Pozisyon pozisyon] // Verilen pozisyondaki taşı döndüren indeksleyici.
        {
            get { return this[pozisyon.Satir, pozisyon.Sutun]; } // Taşı döndürür.
            set { this[pozisyon.Satir, pozisyon.Sutun] = value; } // Taşı ayarlar.
        }
        #endregion

        #region Oyunun başlangıç durumundaki tahtayı oluşturan statik metod
        public static Tahta Baslangic() // Oyunun başlangıç durumundaki tahtayı oluşturan statik metod.
        {
            Tahta tahta = new Tahta(); // Yeni bir Tahta nesnesi oluşturur.
            tahta.BaslangicParcalariEkle(); // Başlangıç taşlarını tahtaya ekler.
            return tahta; // Oluşturulan tahtayı döndürür.
        }
        #endregion

        #region Başlangıç taşlarını tahtaya ekleyen metod
        private void BaslangicParcalariEkle() // Başlangıç taşlarını tahtaya ekleyen metod.
        {
            this[0, 0] = new Kale(Oyuncu.Siyah); // Siyah kale (a8).
            this[0, 1] = new At(Oyuncu.Siyah); // Siyah at (b8).
            this[0, 2] = new Fil(Oyuncu.Siyah); // Siyah fil (c8).
            this[0, 3] = new Vezir(Oyuncu.Siyah); // Siyah vezir (d8).
            this[0, 4] = new Sah(Oyuncu.Siyah); // Siyah şah (e8).
            this[0, 5] = new Fil(Oyuncu.Siyah); // Siyah fil (f8).
            this[0, 6] = new At(Oyuncu.Siyah); // Siyah at (g8).
            this[0, 7] = new Kale(Oyuncu.Siyah); // Siyah kale (h8).

            this[7, 0] = new Kale(Oyuncu.Beyaz); // Beyaz kale (a1).
            this[7, 1] = new At(Oyuncu.Beyaz); // Beyaz at (b1).
            this[7, 2] = new Fil(Oyuncu.Beyaz); // Beyaz fil (c1).
            this[7, 3] = new Vezir(Oyuncu.Beyaz); // Beyaz vezir (d1).
            this[7, 4] = new Sah(Oyuncu.Beyaz); // Beyaz şah (e1).
            this[7, 5] = new Fil(Oyuncu.Beyaz); // Beyaz fil (f1).
            this[7, 6] = new At(Oyuncu.Beyaz); // Beyaz at (g1).
            this[7, 7] = new Kale(Oyuncu.Beyaz); // Beyaz kale (h1).

            for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
            {
                this[1, c] = new Piyon(Oyuncu.Siyah); // Siyah piyonlar (2. satır).
                this[6, c] = new Piyon(Oyuncu.Beyaz); // Beyaz piyonlar (7. satır).
            }
        }
        #endregion

        #region Verilen pozisyonun tahtanın içinde olup olmadığını kontrol eden statik metod
        public static bool IcerideMi(Pozisyon pozisyon) // Verilen pozisyonun tahtanın içinde olup olmadığını kontrol eden statik metod.
        {
            return pozisyon.Satir >= 0 && pozisyon.Satir < 8 && pozisyon.Sutun >= 0 && pozisyon.Sutun < 8;
            // Pozisyonun satır ve sütun numaraları 0 ile 7 arasında ise true döner.
        }
        #endregion

        #region Verilen pozisyonun boş olup olmadığını kontrol eden metod
        public bool BosMu(Pozisyon pozisyon) // Verilen pozisyonun boş olup olmadığını kontrol eden metod.
        {
            return this[pozisyon] == null; // Pozisyonda taş yoksa true döner.
        }
        #endregion

        #region Tahtadaki tüm taşların pozisyonlarını döndüren metod
        public IEnumerable<Pozisyon> TasPozisyonlari() // Tahtadaki tüm taşların pozisyonlarını döndüren metod.
        {
            for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
            {
                for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
                {
                    Pozisyon poz = new Pozisyon(r, c); // Pozisyon nesnesi oluşturur.

                    if (!BosMu(poz)) // Pozisyon boş değilse...
                    {
                        yield return poz; // Pozisyonu döndürür.
                    }
                }
            }
        }
        #endregion

        #region Verilen oyuncunun taşlarının pozisyonlarını döndüren metod
        public IEnumerable<Pozisyon> TasPozisyonlariIcin(Oyuncu oyuncu) // Verilen oyuncunun taşlarının pozisyonlarını döndüren metod.
        {
            return TasPozisyonlari().Where(poz => this[poz].Renk == oyuncu); // Tüm taş pozisyonları arasından verilen oyuncunun taşlarını filtreler.
        }
        #endregion

        #region Verilen renkteki ve türdeki ilk taşı bulur ve pozisyonunu döndürür
        public Pozisyon TasBul(Oyuncu renk, TasTuru tur) // Verilen renkteki ve türdeki ilk taşı bulur ve pozisyonunu döndürür.
        {
            return TasPozisyonlariIcin(renk).FirstOrDefault(poz => this[poz].Tur == tur);
        }
        #endregion

        #region Verilen oyuncunun şahının tehdit altında olup olmadığını kontrol eden metod
        public bool TehditAltinda(Oyuncu oyuncu) // Verilen oyuncunun şahının tehdit altında olup olmadığını kontrol eden metod.
        {
            return TasPozisyonlariIcin(oyuncu.Rakip()).Any(poz => // Rakip oyuncunun taşlarının pozisyonları üzerinde döngü yapar.
            {
                Tas tas = this[poz]; // Pozisyondaki taşı alır.
                return tas.RakipSahiEleGecirilebilir(poz, this); // Taşın rakip şahı ele geçirip geçiremeyeceğini kontrol eder.
            });
        }
        #endregion

        #region Verilen pozisyonun verilen oyuncunun taşları tarafından tehdit edilip edilmediğini kontrol eden metod
        public bool TehditAltinda(Pozisyon pozisyon, Oyuncu oyuncu) // Verilen pozisyonun verilen oyuncunun taşları tarafından tehdit edilip edilmediğini kontrol eden metod.
        {
            return TasPozisyonlariIcin(oyuncu.Rakip()).Any(poz => // Rakip oyuncunun taşlarının pozisyonları üzerinde döngü yapar.
            {
                Tas tas = this[poz]; // Pozisyondaki taşı alır.
                return tas.RakipSahiEleGecirilebilir(poz, this) && tas.HamleYapmak(poz, this).Any(h => h.ToPos == pozisyon);
                // Taşın rakip şahı ele geçirip geçiremeyeceğini ve verilen pozisyona hamle yapıp yapamayacağını kontrol eder.
            });
        }
        #endregion

        #region Tahtanın bir kopyasını oluşturan metod
        public Tahta Kopya() // Tahtanın bir kopyasını oluşturan metod.
        {
            Tahta Kopya = new Tahta(); // Yeni bir Tahta nesnesi oluşturur.

            foreach (Pozisyon poz in TasPozisyonlari()) // Tahtadaki tüm taş pozisyonları üzerinde döngü yapar.
            {
                Kopya[poz] = this[poz].Kopya(); // Taşları kopyalar.
            }

            return Kopya; // Kopya tahtayı döndürür.
        }
        #endregion

        #region Tahtadaki taşların sayısını döndüren metod
        public Sayma ParcaSayisi() // Tahtadaki taşların sayısını döndüren metod.
        {
            Sayma sayma = new Sayma(); // Yeni bir Sayma nesnesi oluşturur.

            foreach (Pozisyon poz in TasPozisyonlari()) // Tahtadaki tüm taş pozisyonları üzerinde döngü yapar.
            {
                Tas tas = this[poz]; // Pozisyondaki taşı alır.
                sayma.Artis(tas.Renk, tas.Tur); // Taşın rengine ve türüne göre sayacı artırır.
            }
            return sayma; // Sayma nesnesini döndürür.
        }
        #endregion

        #region Tahtada yetersiz materyal olup olmadığını kontrol eden metod
        public bool YetersizMateryal() // Tahtada yetersiz materyal olup olmadığını kontrol eden metod.
        {
            Sayma sayma = ParcaSayisi(); // Taş sayısını alır.

            return SahVSSahMi(sayma) || SahFilVSSahMi(sayma) || SahAtVSSahMi(sayma) || SahFilVSSahFilMi(sayma);
            // Yetersiz materyal durumlarını kontrol eder.
        }
        #endregion

        #region Sadece iki şahın kaldığı durumu kontrol eder
        private static bool SahVSSahMi(Sayma sayma) // Sadece iki şahın kaldığı durumu kontrol eder.
        {
            return sayma.ToplamSayi == 2; // Toplam taş sayısı 2 ise true döner.
        }
        #endregion

        #region Şah ve fil vs şah durumunu kontrol eder
        private static bool SahFilVSSahMi(Sayma sayma) // Şah ve fil vs şah durumunu kontrol eder.
        {
            return sayma.ToplamSayi == 3 && (sayma.Beyaz(TasTuru.Fil) == 1 || sayma.Siyah(TasTuru.Fil) == 1);
            // Toplam taş sayısı 3 ise ve bir tarafta bir fil varsa true döner.
        }

        private static bool SahAtVSSahMi(Sayma sayma) // Şah ve at vs şah durumunu kontrol eder.
        {
            return sayma.ToplamSayi == 3 && (sayma.Beyaz(TasTuru.At) == 1 || sayma.Siyah(TasTuru.At) == 1);
            // Toplam taş sayısı 3 ise ve bir tarafta bir at varsa true döner.
        }
        #endregion

        #region Şah ve fil vs şah ve fil durumunu kontrol eder
        private bool SahFilVSSahFilMi(Sayma sayma) // Şah ve fil vs şah ve fil durumunu kontrol eder.
        {
            if (sayma.ToplamSayi != 4) // Toplam taş sayısı 4 değilse...
            {
                return false; // False döner.
            }

            if (sayma.Beyaz(TasTuru.Fil) != 1 || sayma.Siyah(TasTuru.Fil) != 1) // Her iki tarafta da birer fil yoksa...
            {
                return false; // False döner.
            }

            Pozisyon bFilPoz = TasBul(Oyuncu.Beyaz, TasTuru.Fil); // Beyaz filin pozisyonunu bulur.
            Pozisyon sFilPoz = TasBul(Oyuncu.Siyah, TasTuru.Fil); // Siyah filin pozisyonunu bulur.

            return bFilPoz.KareRengi() == sFilPoz.KareRengi(); // Filler aynı renkte karelerde ise true döner.
        }
        #endregion

        #region Şah ve kalenin hareket edip etmediğini kontrol eder
        private bool HareketEtmeyenSahVeKaleMi(Pozisyon sahPoz, Pozisyon kalePoz) // Şah ve kalenin hareket edip etmediğini kontrol eder.
        {
            if (BosMu(sahPoz) || BosMu(kalePoz)) // Pozisyonlardan biri boşsa...
            {
                return false; // False döner.
            }

            Tas sah = this[sahPoz]; // Şahı alır.
            Tas kale = this[kalePoz]; // Kaleyi alır.

            return sah.Tur == TasTuru.Sah && kale.Tur == TasTuru.Kale && !sah.Tasindi && !kale.Tasindi;
            // Şah ve kale hareket etmemişse true döner.
        }
        #endregion

        #region Verilen oyuncunun şah kanadı rok hakkı olup olmadığını kontrol eder
        public bool RokHakkiSahKanadi(Oyuncu oyuncu) // Verilen oyuncunun şah kanadı rok hakkı olup olmadığını kontrol eder.
        {
            return oyuncu switch
            {
                Oyuncu.Beyaz => HareketEtmeyenSahVeKaleMi(new Pozisyon(7, 4), new Pozisyon(7, 7)), // Beyaz şah ve kale.
                Oyuncu.Siyah => HareketEtmeyenSahVeKaleMi(new Pozisyon(0, 4), new Pozisyon(0, 7)), // Siyah şah ve kale.
                _ => false // Diğer durumlarda false.
            };
        }
        #endregion

        #region Verilen oyuncunun vezir kanadı rok hakkı olup olmadığını kontrol eder
        public bool RokHakkiVezirKanadi(Oyuncu oyuncu) // Verilen oyuncunun vezir kanadı rok hakkı olup olmadığını kontrol eder.
        {
            return oyuncu switch
            {
                Oyuncu.Beyaz => HareketEtmeyenSahVeKaleMi(new Pozisyon(7, 4), new Pozisyon(7, 0)), // Beyaz şah ve kale.
                Oyuncu.Siyah => HareketEtmeyenSahVeKaleMi(new Pozisyon(0, 4), new Pozisyon(0, 0)), // Siyah şah ve kale.
                _ => false // Diğer durumlarda false.
            };
        }
        #endregion

        #region Verilen oyuncunun en passant yakalama pozisyonunu döndürür
        public Pozisyon PiyonAtlamaPozisyonunuAl(Oyuncu oyuncu) // Verilen oyuncunun en passant yakalama pozisyonunu döndürür.
        {
            return piyonAtlamaPozisyonlari[oyuncu];
        }

        public void PiyonAtlamaPozisyonunuAyarla(Oyuncu oyuncu, Pozisyon poz) // Verilen oyuncunun en passant yakalama pozisyonunu ayarlar.
        {
            piyonAtlamaPozisyonlari[oyuncu] = poz;
        }
        #endregion

        #region Verilen pozisyonlarda verilen oyuncunun piyonu olup olmadığını kontrol eder
        private bool PiyonVarMi(Oyuncu oyuncu, Pozisyon[] piyonPozisyonlari, Pozisyon atlamaPoz) // Verilen pozisyonlarda verilen oyuncunun piyonu olup olmadığını kontrol eder.
        {
            foreach (Pozisyon poz in piyonPozisyonlari.Where(IcerideMi)) // Pozisyonlar üzerinde döngü yapar.
            {
                Tas tas = this[poz]; // Pozisyondaki taşı alır.
                if (tas == null || tas.Renk != oyuncu || tas.Tur != TasTuru.Piyon) // Taş yoksa, rakip taşsa veya piyon değilse...
                {
                    continue; // Döngünün bir sonraki adımına geçer.
                }

                EnPassant hamle = new EnPassant(poz, atlamaPoz); // EnPassant hamlesi oluşturur.
                if (hamle.Yasal(this)) // Hamle yasal ise...
                {
                    return true; // True döner.
                }
            }

            return false; // Hiçbir piyon bulunamadıysa false döner.
        }
        #endregion

        #region Verilen oyuncunun en passant yakalama yapabilip yapamayacağını kontrol eder
        public bool EnPassantYakalayabilirMi(Oyuncu oyuncu) // Verilen oyuncunun en passant yakalama yapabilip yapamayacağını kontrol eder.
        {
            Pozisyon atlamaPoz = PiyonAtlamaPozisyonunuAl(oyuncu.Rakip()); // Rakip oyuncunun en passant yakalama pozisyonunu alır.

            if (atlamaPoz == null) // En passant yakalama pozisyonu yoksa...
            {
                return false; // False döner.
            }

            Pozisyon[] piyonPozisyonlari = oyuncu switch
            // Oyuncuya göre en passant yakalama yapabilecek piyonların pozisyonlarını belirler.
            {
                Oyuncu.Beyaz => new Pozisyon[] { atlamaPoz + Yon.GuneyBati, atlamaPoz + Yon.GuneyDogu }, // Beyaz oyuncu.
                Oyuncu.Siyah => new Pozisyon[] { atlamaPoz + Yon.KuzeyBati, atlamaPoz + Yon.KuzeyDogu }, // Siyah oyuncu.
                _ => Array.Empty<Pozisyon>() // Diğer durumlarda boş bir dizi.
            };

            return PiyonVarMi(oyuncu, piyonPozisyonlari, atlamaPoz); // Piyon varsa true döner.
        }
        #endregion
    }
}