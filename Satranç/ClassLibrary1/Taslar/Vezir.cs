namespace SatrancMantigi.Taslar
{
    public class Vezir : Tas
    {
        #region Vezir_Özellikleri
        //Vezirin tür özelliği
        public override TasTuru Tur => TasTuru.Vezir;

        //Vezirin renk özelliği
        public override Oyuncu Renk { get; }
        #endregion

        #region Vezir_Yön_Tayini
        private static readonly Yon[] yonler = new Yon[]
        {
           Yon.Kuzey,
           Yon.Guney,
           Yon.Dogu,
           Yon.Bati,
           Yon.KuzeyBati,
           Yon.KuzeyDogu,
           Yon.GuneyBati,
           Yon.GuneyDogu
        };
        #endregion

        #region Vezir_Renk_Tanımlaması
        public Vezir(Oyuncu renk)
        {
            Renk = renk;
        }
        #endregion

        #region Vezir_Kopyalama
        public override Tas Kopya()
        {
            Vezir kopya = new Vezir(Renk);
            kopya.Tasindi = Tasindi;
            return kopya;
        }
        #endregion

        #region Vezir_Hamle_Uygulama_Koleksiyonu
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta)
        {
            return BelirliBirYondeUlasilabilirTumKonumlar(from, tahta, yonler).Select(to => new NormalHamle(from, to));
            //From, tahta ve yonler şartları sağlandığında parçayı oraya hareket ettiren normal bir hareket yaratmalıyız bunu da Select ile yapıyoruz.
        }
        #endregion

    }
}
