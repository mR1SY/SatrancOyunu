namespace SatrancMantigi
{
    //Bu sınıfı bir taşı başka bir konuma basitçe(temel) hareket ettirmek için oluşturduk.
    public class NormalHamle : Hamle // Taşı normal bir şekilde hareket ettiren hamle sınıfı.
    {
        #region Özellikler
        public override HamleTuru Tur => HamleTuru.Normal; // Hamle türünü Normal olarak tanımlar.
        public override Pozisyon FromPos { get; } // Hamlenin başlangıç pozisyonunu tutar.
        public override Pozisyon ToPos { get; } // Hamlenin bitiş pozisyonunu tutar.
        #endregion

        #region Yapıcı Metod
        public NormalHamle(Pozisyon from, Pozisyon to) // NormalHamle nesnesini başlangıç ve bitiş pozisyonlarıyla oluşturan yapıcı metod.
        {
            FromPos = from; // Başlangıç pozisyonunu from parametresinden alır.
            ToPos = to; // Bitiş pozisyonunu to parametresinden alır.
        }
        #endregion

        #region Yürütme Metodu Genel Somut Kısım
        public override bool Execute(Tahta tahta) // NormalHamle'yi tahta üzerinde uygulayan metod.
        {
            Tas tas = tahta[FromPos];  // Başlangıç pozisyonundaki taşı alır.

            bool yakala = !tahta.BosMu(ToPos);// Bitiş pozisyonunda taş olup olmadığını kontrol eder (yakalama durumu).

            tahta[ToPos] = tas;   // Taşı bitiş pozisyonuna taşır.
            tahta[FromPos] = null;   // Taşı başlangıç pozisyonundan kaldırır.
            tas.Tasindi = true;   // Taşın hareket ettiğini işaretler.

            TasSilindi = yakala; // Taş yakalandıysa TasSilindi özelliğini true olarak ayarlar.

            return yakala || tas.Tur == TasTuru.Piyon; // Eğer taş yakalandıysa veya taş bir piyonsa true döner (oyun durumunu etkileyen durumlar).
        }
        #endregion
    }
}