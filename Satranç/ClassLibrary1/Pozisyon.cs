namespace SatrancMantigi
{
    #region Satır_VeSütun_Tanımlaması
    //Bu sınıf poziyonu veya kareyi temsil eder.
    public class Pozisyon
    {
        public int Satir { get; }
        public int Sutun { get; }
         

        //Bu yapıcı metod, satır ve sütun numaralarını parametre olarak alır ve yeni bir Pozisyon nesnesi oluşturur.
        public Pozisyon(int satir, int sutun)
        {
            Satir = satir;
            Sutun = sutun;
        }
        #endregion

        #region Kare_Rengi
        //Anlık olarak konumdaki karenin rengini temsil eder.
        public Oyuncu KareRengi()
        //KareRengi metodu, karenin rengini Oyuncu enum tipinin bir değeri olarak döndürür.
        {
            if ((Satir+Sutun) %2==0)
            {
                return Oyuncu.Beyaz;
            }
            return Oyuncu.Siyah;
        }
        #endregion

        #region Karma_Hash_Kodu
        //Hash kod: Nesnelerin bir koleksiyonda hızlı bir şekilde karşılaştırılmasına yardımcı olur.
        //Burada karma bir hash kodu alıyoruz.
        public override bool Equals(object obj)
        {
            return obj is Pozisyon pozisyon &&
                   Satir == pozisyon.Satir &&
                   Sutun == pozisyon.Sutun;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Satir, Sutun);
        }
        #endregion

        #region Konum_Karşılaştırma
        //Burada konumları karşılaştırıyoruz.
        public static bool operator ==(Pozisyon left, Pozisyon right)
        {
            return EqualityComparer<Pozisyon>.Default.Equals(left, right);
        }

        public static bool operator !=(Pozisyon left, Pozisyon right)
        {
            return !(left == right);
        }
        //== ve != operatörleri, iki Pozisyon nesnesinin eşit olup olmadığını kontrol eder.
        #endregion

        #region Pozisyon_İle_Yön_İlişkilendirilmesi
        //Parametre olarak bir pozisyon ve bir yön alıyor ve verilen yönde bir adım atarak elde ettiğimiz pozisyonu döndürüyoruz. Böylece satır seti ile yeni bir pozisyon döndürüyoruz
        public static Pozisyon operator +(Pozisyon pozisyon, Yon yon)
        {
            return new Pozisyon(pozisyon.Satir + yon.SatirAlfa, pozisyon.Sutun + yon.SutunAlfa);
        }
        #endregion
    }
}
