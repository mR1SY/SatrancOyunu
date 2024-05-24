using SatrancMantigi;

namespace SatrancMantigi
{
    public class Piyon : Tas
    {
        #region Piyon_Özellikleri

        //Piyonun tür özelliği
        public override TasTuru Tur => TasTuru.Piyon;

        //Piyonun renk özelliği
        public override Oyuncu Renk { get; }

        private readonly Yon Ileri;
        #endregion

        #region Piyon_Renk_Tanımlaması

        public Piyon(Oyuncu renk)
        {
            Renk = renk;
            if (renk == Oyuncu.Beyaz)
            {
                Ileri = Yon.Kuzey;
            }
            else if (renk == Oyuncu.Siyah)
            {
                Ileri = Yon.Guney;
            }
        }
        #endregion

        #region Piyon_Kopyalama
        //Burada taş taşınacağı için taşınma sırasında önce kopyalanacağından dolayı kopyalama metodunu çalıştırıyoruz
        public override Tas Kopya()
        {
            Piyon kopya = new Piyon(Renk);
            kopya.Tasindi = Tasindi;
            return kopya;
        }
        #endregion

        #region Piyon_İleri_Hamle

        private static bool Ilerleyebilirmi(Pozisyon poz, Tahta tahta)
        {
            //Konum tahta içinde mi ve konum boş mu. İkisini birlikse sağlarsa "true" döndürür.
            return Tahta.IcerideMi(poz) && tahta.BosMu(poz);
        }
        #endregion

        #region Piyon_Çapraz_Yakalama
        private bool Yakalama(Pozisyon poz, Tahta tahta)
        {
            //Eğer tahtanın içinde değilse veya pozisyon boşsa çapraz olarak alamaz 
            if (!Tahta.IcerideMi(poz) || tahta.BosMu(poz))
            {
                return false;
            }
            //Ancak doluysa ve rakibe(karşıt renk) aitse alabilir
            return tahta[poz].Renk != Renk;
        }
        #endregion

        #region Piyon_Terfi
        //Dört terfi oluşturan yardımcı yöntem ekliyoruz ve hamle yöntemi iki konum parametresi alır
        private static IEnumerable<Hamle> TerfiHamleleri(Pozisyon from, Pozisyon to)
        {
            //Burada taş türünü yield ile döndürüyoruz yani normal return yerine her değeri tek tek döndürüyor ve de ekstra olarak her döndürme sırasında fonksiyonu duraklatıyor ki bellek kullanımını azaltsın
            yield return new PiyonTerfi(from, to, TasTuru.At);
            yield return new PiyonTerfi(from, to, TasTuru.Fil);
            yield return new PiyonTerfi(from, to, TasTuru.Kale);
            yield return new PiyonTerfi(from, to, TasTuru.Vezir);
            //Ama terfi olayı sadece düz değil başka bir taşı da çapraz yiyebileceği için:
        }
        #endregion

        #region Piyon_İleri_Hamle_Koleksiyonu

        //Burası piyonun yapabileceği ileri veya ele geçiremediği hamleleri koleksiyon olarak tutacak

        private IEnumerable<Hamle> IleriHamleler(Pozisyon from, Tahta tahta)
        {
            //Piyon başlangıçta iki kare iletleyebilme özellğine sahip olduğundan mütevellit burada hemen önündeki konumu tanımlıyoruz
            Pozisyon birHamlePozisyonu = from + Ileri;

            //Burada piyonun hareket edip edemediğini kontrol ediyoruz
            if (Ilerleyebilirmi(birHamlePozisyonu, tahta))
            {
                //Piyonun 0. veya 7. satıra ilerleyip ilerlemediğini kontrol ediyoruz
                if (birHamlePozisyonu.Satir == 0 || birHamlePozisyonu.Satir == 7)
                {
                    //Eğer öyleyse dört terfi hamlesinin tümünü döndürürüz
                    foreach (Hamle trfHamlesi in TerfiHamleleri(from, birHamlePozisyonu))
                    {
                        yield return trfHamlesi;
                    }
                }
                //Eğer piyon tahtanın diğer ucuna ulaşmazsa
                else
                {
                    //O zaman sadece normal hamleye ihtiyacımız var
                    yield return new NormalHamle(from, birHamlePozisyonu);
                }

                Pozisyon ikiHamlePozisyonu = birHamlePozisyonu + Ileri;

                //Piyon ancak daha önce hareket etmediyse oraya hareket edebilir(yani ilk iki hamlelik olaydan bahsediyoruz)
                if (!Tasindi && Ilerleyebilirmi(ikiHamlePozisyonu, tahta))
                {
                    //Çift hamle için normal hareket sınıfını kullanıyoruz
                    yield return new CiftPiyon(from, ikiHamlePozisyonu);
                }
            }
        }
        #endregion

        #region Piyon_Çapraz_Hamle_Koleksiyon
        private IEnumerable<Hamle> CaprazHamleler(Pozisyon from, Tahta tahta)
        {
            foreach (Yon yon in new Yon[] { Yon.Bati, Yon.Dogu })
            {
                Pozisyon to = from + Ileri + yon;

                //Dönüşünde ikinci pzosiyonun rakip piyon tarafından atlanıp atlanmadığını kontrol etmelidir
                if (to == tahta.PiyonAtlamaPozisyonunuAl(Renk.Rakip()))
                {
                    //Eğer öyleyse piyon enpassant ile ele geçirilebilir ve değilse o zaman else if kısmına geçer
                    yield return new EnPassant(from, to);
                }

                //Bu kısımda çaprazda bir taş yakalayıp yakalayamayağını kontrol ediyoruz
                else if (Yakalama(to, tahta))
                {
                    if (to.Satir == 0 || to.Satir == 7)
                    {
                        foreach (Hamle trfHamlesi in TerfiHamleleri(from, to))
                        {
                            yield return trfHamlesi;
                        }
                    }
                    //Eğer piyon tahtanın diğer ucuna ulaşmazsa
                    else
                    {
                        //O zaman sadece normal hamleye ihtiyacımız var
                        yield return new NormalHamle(from, to);
                    }

                }
            }
        }
        #endregion

        #region İleri_Ve_Çapraz_Hamleleri_Uygulama_Koleksiyonu
        //Bu kısımda piyonun çapraz ve düz hamlelerini kütüphane şeklinde saklıyoruz
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta)
        {
            return IleriHamleler(from, tahta).Concat(CaprazHamleler(from, tahta));
        }
        #endregion

        #region Rakip_Şahı_Ele_Geçirilebilir_Mi_OVERRİDE

        //Rakip şahı ele geçirlebilir mi seçeneği varsayılan olarak burası için işe yarayacak ancak piyonlar yalnızca çapraz olarak ele geçirebildiği için tüm hamleleri kontrol etmek doğru gelmiyor

        public override bool RakipSahiEleGecirilebilir(Pozisyon from, Tahta tahta)
        {
            //Bu kısımda yapılan herhangi çapraz hamlenin şahı ele geçirip geçiremeyeceğini kontrol edelim
            return CaprazHamleler(from, tahta).Any(hamle =>
            {
                Tas tas = tahta[hamle.ToPos];
                //Konumdaki taşı alırız ve bunu şah olup olmadğını kontrol ederiz. Şahın rakibe ait olup olmadığını kontrol etmeye gerek yoktur. Çünkü asla şahı ele geçiren bir hamle üretilemeyecek.
                return tas != null && tas.Tur == TasTuru.Sah;
            });

        }
        #endregion
    }
}
