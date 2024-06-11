namespace SatrancMantigi
{
    // Şah taşını temsil eden sınıf.
    public class Sah : Tas
    {
        #region Özellikler
        public override TasTuru Tur => TasTuru.Sah; // Taş türünü Şah olarak tanımlar.
        public override Oyuncu Renk { get; } // Şahın rengini tutar.
        #endregion

        #region Şahın hareket edebileceği yönleri tanımlar
        private static readonly Yon[] yonler = new Yon[] // Şahın hareket edebileceği yönleri tanımlar.
        {
           Yon.Kuzey, // Kuzey yönü.
           Yon.Guney, // Güney yönü.
           Yon.Dogu, // Doğu yönü.
           Yon.Bati, // Batı yönü.
           Yon.KuzeyBati, // Kuzeybatı yönü.
           Yon.KuzeyDogu, // Kuzeydoğu yönü.
           Yon.GuneyBati, // Güneybatı yönü.
           Yon.GuneyDogu // Güneydoğu yönü.
        };
        #endregion

        #region Yapıcı metod
        public Sah(Oyuncu renk) // Şah nesnesini renk parametresiyle oluşturan yapıcı metod.
        {
            Renk = renk; // Şahın rengini renk parametresinden alır.
        }
        #endregion

        #region Şah nesnesinin bir kopyasını oluşturan metod
        public override Tas Kopya() // Şah nesnesinin bir kopyasını oluşturan metod.
        {
            Sah kopya = new Sah(Renk); // Yeni bir Şah nesnesi oluşturur ve rengini kopyalar.
            kopya.Tasindi = Tasindi; // Taşın hareket edip etmediği bilgisini kopyalar.
            return kopya; // Kopya Şah nesnesini döndürür.
        }
        #endregion

        #region Rok için kalenin hareket edip etmediğini kontrol eden metod
        private static bool KaleHareketEttiMi(Pozisyon poz, Tahta tahta) // Rok için kalenin hareket edip etmediğini kontrol eden metod.
        {
            if (tahta.BosMu(poz)) // Verilen pozisyon boş ise...
            {
                return false; // Kale hareket etmemiştir, false döner.
            }

            Tas tas = tahta[poz]; // Verilen pozisyondaki taşı alır.

            return tas.Tur == TasTuru.Kale && !tas.Tasindi; // Taş bir kale ise ve hareket etmemişse true döner.
        }
        #endregion

        #region Verilen pozisyonların hepsinin boş olup olmadığını kontrol eden metod
        private static bool HepsiBos(IEnumerable<Pozisyon> pozisyonlar, Tahta tahta) // Verilen pozisyonların hepsinin boş olup olmadığını kontrol eden metod.
        {
            return pozisyonlar.All(poz => tahta.BosMu(poz)); // Tüm pozisyonlar boş ise true döner.
        }
        #endregion

        #region Şah kanadı rok yapılabilir mi kontrol eden metod
        private bool SahKanadiRokOlurMu(Pozisyon from, Tahta tahta) // Şah kanadı rok yapılabilir mi kontrol eden metod.
        {
            if (Tasindi) // Şah daha önce hareket etmişse...
            {
                return false; // Rok yapılamaz, false döner.
            }

            Pozisyon kalePoz = new Pozisyon(from.Satir, 7); // Şah kanadı rok için kalenin olması gereken pozisyon.

            Pozisyon[] pozisyonlarArasinda = new Pozisyon[] { new(from.Satir, 5), new(from.Satir, 6) }; // Şah ve kale arasındaki pozisyonlar.

            return KaleHareketEttiMi(kalePoz, tahta) && HepsiBos(pozisyonlarArasinda, tahta); // Kale hareket etmemişse ve aradaki pozisyonlar boşsa true döner.
        }
        #endregion

        #region Vezir kanadı rok yapılabilir mi kontrol eden metod
        private bool VezirKanadiRokOlurMu(Pozisyon from, Tahta tahta) // Vezir kanadı rok yapılabilir mi kontrol eden metod.
        {
            if (Tasindi) // Şah daha önce hareket etmişse...
            {
                return false; // Rok yapılamaz, false döner.
            }

            Pozisyon kalePoz = new Pozisyon(from.Satir, 0); // Vezir kanadı rok için kalenin olması gereken pozisyon.

            Pozisyon[] pozisyonlarArasinda = new Pozisyon[] { new(from.Satir, 1), new(from.Satir, 2), new(from.Satir, 3) }; // Şah ve kale arasındaki pozisyonlar.

            return KaleHareketEttiMi(kalePoz, tahta) && HepsiBos(pozisyonlarArasinda, tahta); // Kale hareket etmemişse ve aradaki pozisyonlar boşsa true döner.
        }
        #endregion

        #region Şahın geçerli hamle yapabileceği pozisyonları hesaplayan metod
        private IEnumerable<Pozisyon> HamlePozisyonlari(Pozisyon from, Tahta tahta) // Şahın geçerli hamle yapabileceği pozisyonları hesaplayan metod.
        {
            foreach (Yon yon in yonler) // Şahın hareket edebileceği yönler üzerinde döngü yapar.
            {
                Pozisyon to = from + yon; // Hedef pozisyonu hesaplar.

                if (!Tahta.IcerideMi(to)) // Hedef pozisyon tahtanın içinde değilse...
                {
                    continue; // Döngünün bir sonraki adımına geçer.
                }

                if (tahta.BosMu(to) || tahta[to].Renk != Renk) // Hedef pozisyon boş veya rakip taş içeriyorsa...
                {
                    yield return to; // Hedef pozisyonu döndürür.
                }
            }
        }
        #endregion

        #region Şahın yapabileceği tüm hamleleri döndüren metod
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta) // Şahın yapabileceği tüm hamleleri döndüren metod.
        {
            foreach (Pozisyon to in HamlePozisyonlari(from, tahta)) // Şahın geçerli hamle pozisyonları üzerinde döngü yapar.
            {
                yield return new NormalHamle(from, to); // Normal hamle nesnesi oluşturur ve döndürür.
            }

            if (SahKanadiRokOlurMu(from, tahta)) // Şah kanadı rok yapılabilirse...
            {
                yield return new Rok(HamleTuru.RokSahKanadi, from); // Şah kanadı rok hamlesi nesnesi oluşturur ve döndürür.
            }

            if (VezirKanadiRokOlurMu(from, tahta)) // Vezir kanadı rok yapılabilirse...
            {
                yield return new Rok(HamleTuru.RokVezirKanadi, from); // Vezir kanadı rok hamlesi nesnesi oluşturur ve döndürür.
            }
        }
        #endregion

        #region Şahın rakip şahı ele geçirip geçiremeyeceğini kontrol eden metod
        public override bool RakipSahiEleGecirilebilir(Pozisyon from, Tahta tahta) // Şahın rakip şahı ele geçirip geçiremeyeceğini kontrol eden metod.
        {
            return HamlePozisyonlari(from, tahta).Any(to => // Şahın geçerli hamle pozisyonları arasında rakip şahın pozisyonu var mı kontrol eder.
            {
                Tas tas = tahta[to]; // Hedef pozisyondaki taşı alır.
                return tas != null && tas.Tur == TasTuru.Sah; // Taş şah ise true döner.
            });
        }
        #endregion
    }
}