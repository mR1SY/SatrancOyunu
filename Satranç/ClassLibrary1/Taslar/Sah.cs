namespace SatrancMantigi
{
    public class Sah : Tas
    {
        #region Şah_Özellikleri
        //Şahın tür özelliği
        public override TasTuru Tur => TasTuru.Sah;
        //Şahın renk özelliği
        public override Oyuncu Renk { get; }
        #endregion

        #region Şah_Kök_Yön_Tayini
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

        #region Şah_Renk_Tanımlaması
        public Sah(Oyuncu renk)
        {
            Renk = renk;
        }
        #endregion

        #region Rok_Şatları
        
        //Burada rok'un ana kurallarından birisi olan kalenin hareket edip etmediğini kontrol ediyoruz zira kale hareket etmişse rok gerçekleşmeyecektir
        private static bool KaleHareketEttiMi(Pozisyon poz, Tahta tahta)
        {
            //Kale hareket ettiyse false yani kural bozulmuş olacaktır
            if (tahta.BosMu(poz))
            {
                return false;
            }
            
            //Aksi halde taşı o konumdan alırız
            Tas tas = tahta[poz];

            //Ve eğer bu bir kaleyse istenmeyen hareketleri gerçekleştirmemişse true değerini döndürür
            return tas.Tur == TasTuru.Kale && !tas.Tasindi;
        }
        //Burada kale ve şah rasındaki konumların boşluğunu kontrol ediyoruz
        private static bool HepsiBos(IEnumerable<Pozisyon> pozisyonlar, Tahta tahta)
        {
            return pozisyonlar.All(poz => tahta.BosMu(poz));
        }
        #endregion

        #region Şah_Kanadı_Rok
        private bool SahKanadiRokOlurMu(Pozisyon from, Tahta tahta)
        {
            //Şah eğer daha önce hareket etmişse rok gerçekleşmez
            if (Tasindi)
            {
                return false;
            }
            //Aksi halde rok gerçekleşir eğer diğer kuralları da karşılamışsa

            //Kale mevcut satırın 7. sütununda olmalıdır
            Pozisyon kalePoz = new Pozisyon(from.Satir, 7);

            //Ve bu iki konum kale arasındadır
            Pozisyon[] pozisyonlarArasinda = new Pozisyon[] { new(from.Satir, 5), new(from.Satir, 6) };

            //Kale hareket etmediyse ve konumlar boşsa roku döndür
            return KaleHareketEttiMi(kalePoz, tahta) && HepsiBos(pozisyonlarArasinda, tahta);
        }
        #endregion

        #region Vezir_Kanadı_Rok
        private bool VezirKanadiRokOlurMu(Pozisyon from, Tahta tahta)
        {
            //Şah eğer daha önce hareket etmişse rok gerçekleşmez
            if (Tasindi)
            {
                return false;
            }
            //Aksi halde rok gerçekleşir eğer diğer kuralları da karşılamışsa

            //Kale mevcut satırın 0. sütununda olmalıdır
            Pozisyon kalePoz = new Pozisyon(from.Satir, 0);

            //Ve bu üç konum kale arasındadır
            Pozisyon[] pozisyonlarArasinda = new Pozisyon[] { new(from.Satir, 1), new(from.Satir, 2), new(from.Satir, 3) };
            
            //Kale hareket etmediyse ve konumlar boşsa roku döndür
            return KaleHareketEttiMi(kalePoz, tahta) && HepsiBos(pozisyonlarArasinda, tahta);
        }
        #endregion

        #region Şah_Kopyalama
        public override Tas Kopya()
        {
            Sah kopya = new Sah(Renk);
            kopya.Tasindi = Tasindi;
            return kopya;
        }
        #endregion

        #region Şah_Gövde_Yön_Tayini
        private IEnumerable<Pozisyon> HamlePozisyonlari(Pozisyon from, Tahta tahta)
        {
            foreach (Yon yon in yonler)
            {
                //Burada tek bir adım atıyoruz
                Pozisyon to = from + yon;
                
                //Hamle tahtanın içerisinde mi
                if (!Tahta.IcerideMi(to))
                {
                    //Hamle içerde değilse hemen bir sonrakine geç
                    continue;
                }
                
                //Gideceği kare boşsa veya rakip taşı varsa oraya hareket edebilir
                if (tahta.BosMu(to) || tahta[to].Renk != Renk)
                {
                    yield return to;
                }
            }
        }
        #endregion

        #region Şah_Hamle_Uygulama_Koleksiyonu
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta)
        {
            //Sadece yasal hareket pozisyonları arasında döngü yapar
            foreach (Pozisyon to in HamlePozisyonlari(from, tahta))
            {
                //Her biri için normal hamle döndürür
                yield return new NormalHamle(from, to);
            }

            //Burada şah kanadında rok yapmanın mümkün olup olmadığını koleksiyon dahilinde kontrol ediyoruz
            if (SahKanadiRokOlurMu(from, tahta))
            {
                yield return new Rok(HamleTuru.KaleSahKanadi, from);
            }
            //Burada vezir kanadında rok yapmanın mümkün olup olmadığını koleksiyon dahilinde kontrol ediyoruz
            if (VezirKanadiRokOlurMu(from, tahta))
            {
                yield return new Rok(HamleTuru.KaleVezirKanadi, from);
            }
        }
        #endregion

        #region Rakip_Şahı_Ele_Geçirilebilir_Mi_OVERRİDE

        public override bool RakipSahiEleGecirilebilir(Pozisyon from, Tahta tahta)
        {
            return HamlePozisyonlari(from, tahta).Any(to =>
            {
                Tas tas = tahta[to];
                return tas != null && tas.Tur == TasTuru.Sah;
            });
        }
        #endregion
    }
}
