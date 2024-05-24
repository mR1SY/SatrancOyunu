namespace SatrancMantigi
{
    public abstract class Tas
    {
        #region Taş_Sınıfının_Temel_Özellikleri
        //Abstract kullanmamamızın sebebi belli bir parçayı temsil etmemesinden mütevellit. Bu sadece tüm somut taşların burdan miras alacağı bir temel sınıftır.
        public abstract TasTuru Tur { get; }
        public abstract Oyuncu Renk { get; }
        public bool Tasindi { get; set; } = false;
        public abstract Tas Kopya();
        public abstract IEnumerable<Hamle> HamleYapmak(Pozisyon from,Tahta tahta);
        #endregion

        #region Fil_Kale_Vezir_İçin_Hamleler
        //Fil,Kale ve Vezir belirli bir yönde istedikleri kadar hareket ettikleri için bunu tanımlıyoruz çünkü bunu kolaylaştırmaız gerekiyor.
        protected IEnumerable<Pozisyon> BelirliBirYondeUlasilabilirTumKonumlar(Pozisyon from, Tahta tahta, Yon yon)
        {
            for (Pozisyon poz = from + yon; Tahta.IcerideMi(poz); poz += yon)
            {
                //Verilen doğrultuda hareket eder;Konum tahtanın içindedir;Her yinelmeden sonra başka bir adım alır.
                if(tahta.BosMu(poz))//Burada mevcut konumun boşluğunu kontrol ettik
                {
                    yield return poz;
                    continue;
                }
                //Aksi halde pozisyonda bir taş vardır:
                Tas tas =tahta[poz];

                //Ve eğer bu denk gelinen taş rakibe aitse
                if (tas.Renk != Renk)
                {
                    yield return poz;
                    //Ulaşılabilir(ele geçirilebilir)
                }
                yield break;
                //Renkler aynı ise ulaşılamaz
            }
        }
        //Yield: Normal return gibi değil her sonucu yukarı gönderiyor

        //Ulaşılabilir tüm konumları içeren bir koleksiyon verir.
        protected IEnumerable<Pozisyon> BelirliBirYondeUlasilabilirTumKonumlar(Pozisyon from, Tahta tahta, Yon[] yonler)
        {
            return yonler.SelectMany(yon => BelirliBirYondeUlasilabilirTumKonumlar(from, tahta, yon));
        }
        #endregion

        #region Rakip_Şahı_Ele_Geçirilebilir_Mi_VİRTUAL
        //Ancak ve ancak rakip şah ele geçirilebilecek durumdaysa true döndürür. Ancak bu gerçek oyunda asla gerçekleşmez çünkü gerçek oyunda şah yenilebilen değil sıkıştıralarak yani hamlesiz bırakılarak(mat edilir). Ama bunu sahne arkasında şahı kontrol etmek için kullancağız

        //Sanal yapmamızın sebebi asla gerçekleşmeycek bir durum olması ama bunu şah çekme işlemleri için kullancağız
        public virtual bool RakipSahiEleGecirilebilir(Pozisyon from, Tahta tahta)
        {
            //Bu kısımda yapılan herhangi bir hamlenin rakip şahını ele geçirip geçiremediğini kontrol ederiz.
            return HamleYapmak(from, tahta).Any(hamle =>
            {
                Tas tas = tahta[hamle.ToPos];
                //Konumdaki taşı alırız ve bunu şah olup olmadğını kontrol ederiz. Şahın rakibe ait olup olmadığını kontrol etmeye gerek yoktur. Çünkü asla şahı ele geçiren bir hamle üretilemeyecek.
                return tas != null && tas.Tur == TasTuru.Sah;
            });
        }
        #endregion
    }
}
