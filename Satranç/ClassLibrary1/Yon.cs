namespace SatrancMantigi
{
    // Satranç tahtasındaki hareket yönlerini temsil eden sınıf.
    public class Yon
    {
        #region Özellikler
        public int SatirAlfa { get; } // Yönün satır bileşeni (değişiklik miktarı).
        public int SutunAlfa { get; } // Yönün sütun bileşeni (değişiklik miktarı).
        #endregion

        #region Tanımlamalar
        public readonly static Yon Kuzey = new Yon(-1, 0); // Kuzey yönü (satırda -1, sütunda 0 değişiklik).
        public readonly static Yon Guney = new Yon(1, 0); // Güney yönü (satırda 1, sütunda 0 değişiklik).
        public readonly static Yon Dogu = new Yon(0, 1); // Doğu yönü (satırda 0, sütunda 1 değişiklik).
        public readonly static Yon Bati = new Yon(0, -1); // Batı yönü (satırda 0, sütunda -1 değişiklik).
        public readonly static Yon KuzeyDogu = Kuzey + Dogu; // Kuzeydoğu yönü (Kuzey ve Doğu yönlerinin toplamı).
        public readonly static Yon KuzeyBati = Kuzey + Bati; // Kuzeybatı yönü (Kuzey ve Batı yönlerinin toplamı).
        public readonly static Yon GuneyDogu = Guney + Dogu; // Güneydoğu yönü (Güney ve Doğu yönlerinin toplamı).
        public readonly static Yon GuneyBati = Guney + Bati; // Güneybatı yönü (Güney ve Batı yönlerinin toplamı).
        #endregion

        #region Yapıcı metod
        public Yon(int satirAlfa, int sutunAlfa) // Yon nesnesini satır ve sütun bileşenleriyle oluşturan yapıcı metod.
        {
            SatirAlfa = satirAlfa; // Satır bileşenini ayarlar.
            SutunAlfa = sutunAlfa; // Sütun bileşenini ayarlar.
        }
        #endregion

        #region İki yönü toplayarak yeni bir yön döndüren operatör
        public static Yon operator +(Yon yon1, Yon yon2) // İki yönü toplayarak yeni bir yön döndüren operatör.
        {
            return new Yon(yon1.SatirAlfa + yon2.SatirAlfa, yon1.SutunAlfa + yon2.SutunAlfa);
            // Yeni yönün satır ve sütun bileşenlerini hesaplar.
        }
        #endregion

        #region Bir yönü bir skaler ile çarparak yeni bir yön döndüren operatör
        public static Yon operator *(int skaler, Yon yon) // Bir yönü bir skaler ile çarparak yeni bir yön döndüren operatör.
        {
            return new Yon(skaler * yon.SatirAlfa, skaler * yon.SutunAlfa);
            // Yeni yönün satır ve sütun bileşenlerini hesaplar.
        }
        #endregion
    }
}