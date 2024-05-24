namespace SatrancMantigi
{
    public class At : Tas
    {
        #region At_Özellikleri
        
        //Atın tür özelliği
        public override TasTuru Tur => TasTuru.At;

        //Atın renk özelliği
        public override Oyuncu Renk { get; }

        #endregion

        #region At_Renk_Tanımlaması
        public At(Oyuncu renk)
        {
            Renk = renk;
        }
        #endregion

        #region At_Kopyalama
        public override Tas Kopya()
        {
            At kopya = new At(Renk);
            kopya.Tasindi = Tasindi;
            return kopya;
        }
        #endregion

        #region At_Yön_Tayini
        //Burası atın potansiyel olarak hareket edebileceği tüm konumları döndürüyor
        private static IEnumerable<Pozisyon> PozisyonlaraYonelikPotansiyel(Pozisyon from)
        {
            //Burada dikey yönü döndürüyoruz
            foreach (Yon vyon in new Yon[] { Yon.Kuzey, Yon.Guney })
            {
                //Burada yatay yönü döndürüyoruz
                foreach (Yon hyon in new Yon[] { Yon.Bati, Yon.Dogu })
                {
                    //Burası sekiz potansiyel konumun tamamını verir
                    yield return from + 2 * vyon + hyon;
                    yield return from + 2 * hyon + vyon;
                }
            }
        }
        #endregion
        
        #region At_Hamle_Koleksiyonu

        //Burada tüm hamle yapabilme potansiyellerini koleksiyona topluyoruz
        private IEnumerable<Pozisyon> HamlePozisyonlari(Pozisyon from, Tahta tahta)
        {
            return PozisyonlaraYonelikPotansiyel(from).Where(poz => Tahta.IcerideMi(poz) && (tahta.BosMu(poz) || tahta[poz].Renk != Renk));
        }
        #endregion

        #region At_Hamle_Uygulama_Koleksiyonu

        //Burada da tüm hamleleri normal hamle olarak atıyoruz
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta)
        {
            return HamlePozisyonlari(from,tahta).Select(to => new NormalHamle(from,to));
        }
        #endregion
    }
}
