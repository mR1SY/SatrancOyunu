namespace SatrancMantigi
{
    public class Sonuc
    {
        #region Sonuç_Özellikleri

        //Oyunun kazananını belli eden özellik
        public Oyuncu Kazanan { get; }

        //Oyunun bitiş sebebini söyleyen özellik
        public BitisSebebi Sebep { get; }

        public Sonuc(Oyuncu kazanan, BitisSebebi sebep)
        {
            Kazanan = kazanan;
            Sebep = sebep;
        }

        #endregion

        #region Şah_Mat
        //Kazanan oyuncuyu alır ve şahmat nedenini içeren yeni bir sonuç döndürür
        public static Sonuc Kazanmak(Oyuncu kazanan)
        {
            return new Sonuc(kazanan, BitisSebebi.SahMat);
        }
        #endregion

        #region Berabere

        //Bir nedenden dolayı berabere sonuçlanma kızmı
        public static Sonuc Beraberlik(BitisSebebi sebep)
        {
            //Kazanan oyuncuya ayarlanmadığı için sonuç döndürüyoruz
            return new Sonuc(Oyuncu.Bos, sebep);
        }

        #endregion
    }
}
