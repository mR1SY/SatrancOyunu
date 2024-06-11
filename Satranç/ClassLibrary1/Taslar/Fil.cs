namespace SatrancMantigi
{
    // Fil taşını temsil eden sınıf.
    public class Fil : Tas
    {
        #region Özellikler
        public override TasTuru Tur => TasTuru.Fil; // Taş türünü Fil olarak tanımlar.
        public override Oyuncu Renk { get; } // Filin rengini tutar.
        #endregion

        #region Filin hareket edebileceği yönleri tanımlar
        private static readonly Yon[] yonler = new Yon[] // Filin hareket edebileceği yönleri tanımlar.
        {
            Yon.KuzeyBati, // Kuzeybatı yönü.
            Yon.KuzeyDogu, // Kuzeydoğu yönü.
            Yon.GuneyBati, // Güneybatı yönü.
            Yon.GuneyDogu  // Güneydoğu yönü.
        };
        #endregion

        #region Yapıcı metod
        public Fil(Oyuncu renk) // Fil nesnesini renk parametresiyle oluşturan yapıcı metod.
        {
            Renk = renk; // Filin rengini renk parametresinden alır.
        }
        #endregion

        #region Fil nesnesinin bir kopyasını oluşturan metod
        public override Tas Kopya() // Fil nesnesinin bir kopyasını oluşturan metod.
        {
            Fil kopya = new(Renk) // Yeni bir Fil nesnesi oluşturur ve rengini kopyalar.
            {
                Tasindi = Tasindi // Taşın hareket edip etmediği bilgisini kopyalar.
            };
            return kopya; // Kopya Fil nesnesini döndürür.
        }
        #endregion

        #region Filin yapabileceği tüm hamleleri döndüren metod
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta) // Filin yapabileceği tüm hamleleri döndüren metod.
        {
            return BelirliBirYondeUlasilabilirTumKonumlar(from, tahta, yonler).Select(to => new NormalHamle(from, to));
            // Filin çapraz yönlerde gidebileceği tüm pozisyonları hesaplar ve bu pozisyonlara NormalHamle nesneleri oluşturur.
        }
        #endregion
    }
}