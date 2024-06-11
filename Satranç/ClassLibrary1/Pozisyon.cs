namespace SatrancMantigi
{
    // Satranç tahtasındaki bir kareyi (pozisyonu) temsil eden sınıf.
    public class Pozisyon
    {
        #region Özellikler
        public int Satir { get; } // Karenin satır numarasını tutar.
        public int Sutun { get; } // Karenin sütun numarasını tutar.
        #endregion

        #region Yapıcı metod
        public Pozisyon(int satir, int sutun) // Pozisyon nesnesini satır ve sütun numaralarıyla oluşturan yapıcı metod.
        {
            Satir = satir; // Satır numarasını satir parametresinden alır.
            Sutun = sutun; // Sütun numarasını sutun parametresinden alır.
        }
        #endregion

        #region İki Pozisyon nesnesinin eşit olup olmadığını kontrol eden metod
        public override bool Equals(object obj) // İki Pozisyon nesnesinin eşit olup olmadığını kontrol eden metod.
        {
            return obj is Pozisyon pozisyon &&
                   Satir == pozisyon.Satir &&
                   Sutun == pozisyon.Sutun;
            // obj bir Pozisyon nesnesi ise ve satır ve sütun numaraları eşitse true döner.
        }
        #endregion

        #region Pozisyon nesnesi için benzersiz bir hash kodu döndüren metod
        public override int GetHashCode() // Pozisyon nesnesi için benzersiz bir hash kodu döndüren metod.
        {
            return HashCode.Combine(Satir, Sutun); // Satır ve sütun numaralarını kullanarak bir hash kodu oluşturur.
        }
        #endregion

        #region İki Pozisyon nesnesinin eşit olup olmadığını kontrol eden operatör
        public static bool operator ==(Pozisyon left, Pozisyon right) // İki Pozisyon nesnesinin eşit olup olmadığını kontrol eden operatör.
        {
            return EqualityComparer<Pozisyon>.Default.Equals(left, right); // Pozisyon nesnelerinin varsayılan eşitlik karşılaştırıcısını kullanarak karşılaştırır.
        }
        #endregion

        #region İki Pozisyon nesnesinin eşit olup olmadığını kontrol eden operatör
        public static bool operator !=(Pozisyon left, Pozisyon right) // İki Pozisyon nesnesinin eşit olup olmadığını kontrol eden operatör.
        {
            return !(left == right); // Eşitlik operatörünün sonucunu tersine çevirir.
        }
        #endregion

        #region Pozisyon nesnesine bir yön ekleyerek yeni bir pozisyon döndüren operatör
        public static Pozisyon operator +(Pozisyon pozisyon, Yon yon) // Pozisyon nesnesine bir yön ekleyerek yeni bir pozisyon döndüren operatör.
        {
            return new Pozisyon(pozisyon.Satir + yon.SatirAlfa, pozisyon.Sutun + yon.SutunAlfa);
            // Yeni pozisyonun satır ve sütun numaralarını hesaplar.
        }
        #endregion

        #region Karenin rengini (Beyaz veya Siyah) döndüren metod
        public Oyuncu KareRengi() // Karenin rengini (Beyaz veya Siyah) döndüren metod.
        {
            if ((Satir + Sutun) % 2 == 0) // Satır ve sütun numaralarının toplamı çift ise...
            {
                return Oyuncu.Beyaz; // Kare rengi Beyaz'dır.
            }
            return Oyuncu.Siyah; // Kare rengi Siyah'dır.
        }
        #endregion
    }
}