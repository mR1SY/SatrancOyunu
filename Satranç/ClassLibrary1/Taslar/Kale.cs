namespace SatrancMantigi
{
    // Kale taşını temsil eden sınıf.
    public class Kale : Tas
    {
        #region Özellikler
        public override TasTuru Tur => TasTuru.Kale; // Taş türünü Kale olarak tanımlar.
        public override Oyuncu Renk { get; } // Kalenin rengini tutar.
        #endregion

        #region Kalenin hareket edebileceği yönleri tanımlar
        private static readonly Yon[] yonler = new Yon[] // Kalenin hareket edebileceği yönleri tanımlar.
        {
            Yon.Kuzey, // Kuzey yönü.
            Yon.Guney, // Güney yönü.
            Yon.Dogu, // Doğu yönü.
            Yon.Bati  // Batı yönü.
        };
        #endregion

        #region Yapıcı metod
        public Kale(Oyuncu renk) // Kale nesnesini renk parametresiyle oluşturan yapıcı metod.
        {
            Renk = renk; // Kalenin rengini renk parametresinden alır.
        }
        #endregion

        #region Kale nesnesinin bir kopyasını oluşturan metod
        public override Tas Kopya() // Kale nesnesinin bir kopyasını oluşturan metod.
        {
            Kale kopya = new Kale(Renk); // Yeni bir Kale nesnesi oluşturur ve rengini kopyalar.
            kopya.Tasindi = Tasindi; // Taşın hareket edip etmediği bilgisini kopyalar.
            return kopya; // Kopya Kale nesnesini döndürür.
        }
        #endregion

        #region Kalenin yapabileceği tüm hamleleri döndüren metod
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta) // Kalenin yapabileceği tüm hamleleri döndüren metod.
        {
            return BelirliBirYondeUlasilabilirTumKonumlar(from, tahta, yonler).Select(to => new NormalHamle(from, to));
            // Kalenin dikey ve yatay yönlerde gidebileceği tüm pozisyonları hesaplar ve bu pozisyonlara NormalHamle nesneleri oluşturur.
        }
        #endregion
    }
}