namespace SatrancMantigi
{
    // Tahtada bulunan her taş türünden kaç adet olduğunu sayar.
    public class Sayma
    {
        #region Özellikler
        public int ToplamSayi { get; private set; } // Tahtadaki toplam taş sayısını tutar.
        #endregion

        #region Tanımlamar
        private readonly Dictionary<TasTuru, int> beyazSayisi = new(); // Beyaz taşların sayısını tutan sözlük.
        private readonly Dictionary<TasTuru, int> siyahSayisi = new(); // Siyah taşların sayısını tutan sözlük.
        #endregion

        #region Yapıcı metod
        public Sayma() // Sayma nesnesini oluşturan yapıcı metod.
        {
            foreach (TasTuru tur in Enum.GetValues(typeof(TasTuru))) // Tüm taş türleri üzerinde döngü yapar.
            {
                beyazSayisi[tur] = 0; // Beyaz taş sayısını başlangıçta 0 olarak ayarlar.
                siyahSayisi[tur] = 0; // Siyah taş sayısını başlangıçta 0 olarak ayarlar.
            }
        }
        #endregion

        #region Verilen renkteki verilen taş türünün sayısını artırır
        public void Artis(Oyuncu renk, TasTuru tur) // Verilen renkteki verilen taş türünün sayısını artırır.
        {
            if (renk == Oyuncu.Beyaz) // Renk beyaz ise...
            {
                beyazSayisi[tur]++; // Beyaz taş sayısını artırır.
            }
            else if (renk == Oyuncu.Siyah) // Renk siyah ise...
            {
                siyahSayisi[tur]++; // Siyah taş sayısını artırır.
            }
            ToplamSayi++; // Toplam taş sayısını artırır.
        }
        #endregion

        #region Verilen taş türündeki beyaz taşların sayısını döndürür
        public int Beyaz(TasTuru tur) // Verilen taş türündeki beyaz taşların sayısını döndürür.
        {
            return beyazSayisi[tur];
        }
        #endregion

        #region Verilen taş türündeki siyah taşların sayısını döndürür
        public int Siyah(TasTuru tur) // Verilen taş türündeki siyah taşların sayısını döndürür.
        {
            return siyahSayisi[tur];
        }
        #endregion
    }
}