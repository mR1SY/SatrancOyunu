namespace SatrancMantigi
{
    // Oyuncuları (Beyaz, Siyah veya Boş) temsil eden enum.
    public enum Oyuncu
    {
        Bos, // Boş oyuncu (oyun tahtasındaki boş kareleri temsil etmek için kullanılır).
        Beyaz, // Beyaz oyuncu.
        Siyah // Siyah oyuncu.
    }

    public static class OyuncuUzantıları // Oyuncu enum'ı için yardımcı metodları içeren statik sınıf.
    {
        #region Verilen oyuncunun rakibini döndüren metod
        public static Oyuncu Rakip(this Oyuncu oyuncu) // Verilen oyuncunun rakibini döndüren metod.
        {
            return oyuncu switch // Oyuncuya göre rakibi döndürür.
            {
                Oyuncu.Beyaz => Oyuncu.Siyah, // Beyaz oyuncunun rakibi siyah oyuncudur.
                Oyuncu.Siyah => Oyuncu.Beyaz, // Siyah oyuncunun rakibi beyaz oyuncudur.
                _ => Oyuncu.Bos // Diğer durumlarda (Boş oyuncu) boş oyuncu döndürür.
            };
        }
        #endregion
    }
}