namespace SatrancMantigi.Taslar
{
    // Vezir taşını temsil eden sınıf.
    public class Vezir : Tas
    {
        #region Özellikler
        public override TasTuru Tur => TasTuru.Vezir; // Taş türünü Vezir olarak tanımlar.
        public override Oyuncu Renk { get; } // Vezirin rengini tutar.
        #endregion

        #region Vezirin hareket edebileceği yönleri tanımlar
        private static readonly Yon[] yonler = new Yon[] // Vezirin hareket edebileceği yönleri tanımlar.
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

        #region Yapıcı Metod
        public Vezir(Oyuncu renk) // Vezir nesnesini renk parametresiyle oluşturan yapıcı metod.
        {
            Renk = renk; // Vezirin rengini renk parametresinden alır.
        }
        #endregion

        #region Vezir nesnesinin bir kopyasını oluşturan metod
        public override Tas Kopya() // Vezir nesnesinin bir kopyasını oluşturan metod.
        {
            Vezir kopya = new Vezir(Renk); // Yeni bir Vezir nesnesi oluşturur ve rengini kopyalar.
            kopya.Tasindi = Tasindi; // Taşın hareket edip etmediği bilgisini kopyalar.
            return kopya; // Kopya Vezir nesnesini döndürür.
        }
        #endregion

        #region Vezirin yapabileceği tüm hamleleri döndüren metod
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta) // Vezirin yapabileceği tüm hamleleri döndüren metod.
        {
            return BelirliBirYondeUlasilabilirTumKonumlar(from, tahta, yonler).Select(to => new NormalHamle(from, to));
            // Vezirin tüm yönlerde gidebileceği pozisyonları hesaplar ve bu pozisyonlara NormalHamle nesneleri oluşturur.
        }
        #endregion
    }
}