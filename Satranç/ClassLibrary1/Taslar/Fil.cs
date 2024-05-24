
namespace SatrancMantigi
{
    public class Fil : Tas
    {
        #region Fil_Özellikleri
        
        //Filin tür özelliği
        public override TasTuru Tur => TasTuru.Fil;
        
        //Filin renk özelliği
        public override Oyuncu Renk { get; }
        #endregion

        #region Fil_Yön_Tayini
        //Tüm çapraz yönleri içeren bir yön dizisi tanımlıyoruz
        private static readonly Yon[] yonler = new Yon[]
        {
            Yon.KuzeyBati,
            Yon.KuzeyDogu,
            Yon.GuneyBati,
            Yon.GuneyDogu
        };
        #endregion

        #region Fil_Renk_Tanımlaması
        public Fil(Oyuncu renk)
        {
            Renk = renk;
        }
        #endregion

        #region Fil_Kopyalama
        public override Tas Kopya()
        {
            Fil kopya = new(Renk)
            {
                Tasindi = Tasindi
            };
            return kopya;
        }
        #endregion

        #region Fil_Hamle_Uygulama_Koleksiyonu
        //
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta)
        {
            return BelirliBirYondeUlasilabilirTumKonumlar(from, tahta,yonler).Select(to=>new NormalHamle(from,to));
            //From, tahta ve yonler şartları sağlandığında parçayı oraya hareket ettiren normal bir hareket yaratmalıyız bunu da Select ile yapıyoruz.
        }
        #endregion
    }
}
