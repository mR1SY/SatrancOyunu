namespace SatrancMantigi
{
    #region Tüm_Hamlelerin_Özellik_Kalıpları
    //Tüm somut hamleler için temel sınıf olduğundan ana özellik kalıplarını burda tanımlayacağız
    public abstract class Hamle
    {
        public abstract HamleTuru Tur { get; }
        public abstract Pozisyon FromPos {  get; }
        public abstract Pozisyon ToPos { get; }
        //Bu parçanın hareket ettiği yerdir ve her hareketin de bir yürütmesi olacaktır
        public abstract bool Execute(Tahta tahta);

        //Ancak ve ancak bu hamleyi gerçekleştirmek mevcut oyuncunun şahını tehdit altında bırakmazsa true dönmelidir. Yani bir oyuncu karşı hamle oynayacaksa bunun yasal olması için kendi şahının tehdit altında olmaması gerekir

        public virtual bool Yasal(Tahta tahta)
        {
            //İlk önce hareket edecek taşın rengini kontrol ederek hareket eden oyuncuyu elde edeceğiz
            Oyuncu oyuncu = tahta[FromPos].Renk;
            //Sonra tahtayı kopyalarız
            Tahta tahtaKopya = tahta.Kopya();
            //Ve kopya üzerinde hamleyi gerçekleştiririz
            Execute(tahtaKopya);
            //Ve eğer oyuncunun şahı hamleden sonra şahta değilse true değerini döndürürüz
            return !tahtaKopya.TehditAltinda(oyuncu);
        }
        public bool TasSilindi { get; protected set; } = false;

    }
    #endregion
}
