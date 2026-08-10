namespace SatrancMantigi
{
    public class At : Tas // At taşını temsil eden sınıf.
    {
        #region Özellikler
        public override TasTuru Tur => TasTuru.At; // Taş türünü At olarak tanımlar.
        public override Oyuncu Renk { get; } // Atın rengini tutar.
        #endregion

        #region Yapıcı metod
        public At(Oyuncu renk) // At nesnesini renk parametresiyle oluşturan yapıcı metod.
        {
            Renk = renk; // Atın rengini renk parametresinden alır.
        }
        #endregion

        #region At nesnesinin bir kopyasını oluşturan metod
        public override Tas Kopya() // At nesnesinin bir kopyasını oluşturan metod.
        {
            At kopya = new At(Renk); // Yeni bir At nesnesi oluşturur ve rengini kopyalar.
            kopya.Tasindi = Tasindi; // Taşın hareket edip etmediği bilgisini kopyalar.
            return kopya; // Kopya At nesnesini döndürür.
        }
        #endregion

        #region Atın potansiyel hareket edebileceği pozisyonları hesaplayan metod
        private static IEnumerable<Pozisyon> PozisyonlaraYonelikPotansiyel(Pozisyon from) // Atın potansiyel hareket edebileceği pozisyonları hesaplayan metod.
        {
            foreach (Yon vyon in new Yon[] { Yon.Kuzey, Yon.Guney }) // Dikey yönler (kuzey ve güney) üzerinde döngü yapar.
            {
                foreach (Yon hyon in new Yon[] { Yon.Bati, Yon.Dogu }) // Yatay yönler (batı ve doğu) üzerinde döngü yapar.
                {
                    //Burası sekiz potansiyel konumun tamamını verir.
                    yield return from + 2 * vyon + hyon; // Bir dikey yönde 2 kare, bir yatay yönde 1 kare hareket eder.
                    yield return from + 2 * hyon + vyon; // Bir yatay yönde 2 kare, bir dikey yönde 1 kare hareket eder.
                }
            }
        }
        #endregion

        #region Atın geçerli hamle yapabileceği pozisyonları hesaplayan metod
        private IEnumerable<Pozisyon> HamlePozisyonlari(Pozisyon from, Tahta tahta) // Atın geçerli hamle yapabileceği pozisyonları hesaplayan metod.
        {
            return PozisyonlaraYonelikPotansiyel(from).Where(poz => Tahta.IcerideMi(poz) && (tahta.BosMu(poz) || tahta[poz].Renk != Renk));
            // Potansiyel pozisyonlar arasından tahtanın içinde olan ve boş veya rakip taş içeren pozisyonları filtreler.
        }
        #endregion

        #region Atın yapabileceği tüm hamleleri döndüren metod
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta) // Atın yapabileceği tüm hamleleri döndüren metod.
        {
            return HamlePozisyonlari(from, tahta).Select(to => new NormalHamle(from, to));
            // Geçerli hamle pozisyonları için NormalHamle nesneleri oluşturur.
        }
        #endregion
    }
}