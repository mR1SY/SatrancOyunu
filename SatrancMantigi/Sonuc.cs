namespace SatrancMantigi
{
    // Oyunun sonucunu (kazanan ve bitiş sebebi) temsil eden sınıf.
    public class Sonuc
    {
        #region Özelllikler
        public Oyuncu Kazanan { get; } // Oyunun kazananını tutar (Beyaz, Siyah veya Boş).
        public BitisSebebi Sebep { get; } // Oyunun bitiş sebebini tutar.
        #endregion

        #region Yapıcı Metod
        public Sonuc(Oyuncu kazanan, BitisSebebi sebep) // Sonuc nesnesini kazanan ve bitiş sebebi ile oluşturan yapıcı metod.
        {
            Kazanan = kazanan; // Kazananı ayarlar.
            Sebep = sebep; // Bitiş sebebini ayarlar.
        }
        #endregion

        #region Şah mat ile oyun bitiş sonucunu oluşturan metod
        public static Sonuc Kazanmak(Oyuncu kazanan) // Şah mat ile oyun bitiş sonucunu oluşturan metod.
        {
            return new Sonuc(kazanan, BitisSebebi.SahMat); // Kazananı ve bitiş sebebini (ŞahMat) ayarlar ve yeni bir Sonuc nesnesi döndürür.
        }
        #endregion

        #region Beraberlik ile oyun bitiş sonucunu oluşturan metod
        public static Sonuc Beraberlik(BitisSebebi sebep) // Beraberlik ile oyun bitiş sonucunu oluşturan metod.
        {
            return new Sonuc(Oyuncu.Bos, sebep); // Kazananı Boş olarak ve bitiş sebebini ayarlar ve yeni bir Sonuc nesnesi döndürür.
        }
        #endregion
    }
}