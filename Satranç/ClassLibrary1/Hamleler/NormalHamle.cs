namespace SatrancMantigi
{
    //Bu sınıfı bir taşı başka bir konuma basitçe(temel) hareket ettirmek için oluşturduk.
    public class NormalHamle : Hamle
    {
        #region Normal_Hamle_Özellikleri
        public override HamleTuru Tur => HamleTuru.Normal;
        public override Pozisyon FromPos { get; }
        public override Pozisyon ToPos { get; }
        public NormalHamle(Pozisyon from, Pozisyon to)
        {
            FromPos = from;
            ToPos = to;
        }
        #endregion

        #region Taş_Hareket
        //Hareketi yapan asıl aşama burası
        public override bool Execute(Tahta tahta)
        {
            Tas tas = tahta[FromPos];  //Önce hareket ettiriyoruz.
            
            bool yakala = !tahta.BosMu(ToPos);//Burada hamlenin bir taş yakalayıp yakalaymaadığını kontrol ediyoruz

            tahta[ToPos] = tas;  //Sonra gittiği yere kopyalıyoruz
            tahta[FromPos] = null;  //Eski yerinden taşı kaldırıyoruz
            tas.Tasindi = true;  //Hareket ettiğini doğruluyoruz

            TasSilindi = yakala;

            return yakala || tas.Tur == TasTuru.Piyon;//Eğer bir yakalama varsa ve piyon üzerindeyse
        }
        #endregion
    }
}
