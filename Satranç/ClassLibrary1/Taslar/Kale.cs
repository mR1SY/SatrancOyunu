namespace SatrancMantigi
{
    public class Kale : Tas
    {
        #region Kale_Özellikleri
        //Kalenin tür özelliği
        public override TasTuru Tur => TasTuru.Kale;
        
        //Kalenin renk özelliği
        public override Oyuncu Renk { get; }
        #endregion

        #region Kale_YönTayini
        //Tüm dikey ve yatay yönleri içeren bir yön dizisi tanımlıyoruz
        private static readonly Yon[] yonler = new Yon[]
        {
            Yon.Kuzey,
            Yon.Guney,
            Yon.Dogu,
            Yon.Bati
        };
        #endregion

        #region Kale_Renk_Tanımlaması
        public Kale(Oyuncu renk)
        {
            Renk = renk;
        }
        #endregion

        #region Kale_Kopyalama
        public override Tas Kopya()
        {
            Kale kopya = new Kale(Renk);
            kopya.Tasindi = Tasindi;
            return kopya;
        }
        #endregion

        #region Kale_Hamle_Uygulama_Koleksiyonu
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta)
        {
            return BelirliBirYondeUlasilabilirTumKonumlar(from, tahta, yonler).Select(to => new NormalHamle(from, to));
            //From, tahta ve yonler şartları sağlandığında parçayı oraya hareket ettiren normal bir hareket yaratmalıyız bunu da Select ile yapıyoruz.
        }
        #endregion
    }
}
