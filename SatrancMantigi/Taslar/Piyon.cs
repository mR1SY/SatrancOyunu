using SatrancMantigi;

namespace SatrancMantigi
{
    // Piyon taşını temsil eden sınıf.
    public class Piyon : Tas
    {
        #region Özellikler
        public override TasTuru Tur => TasTuru.Piyon; // Taş türünü Piyon olarak tanımlar.
        public override Oyuncu Renk { get; } // Piyonun rengini tutar.

        private readonly Yon Ileri; // Piyonun ileri hareket yönünü tanımlar.
        #endregion

        #region Yapıcı metod
        public Piyon(Oyuncu renk) // Piyon nesnesini renk parametresiyle oluşturan yapıcı metod.
        {
            Renk = renk; // Piyonun rengini renk parametresinden alır.
            if (renk == Oyuncu.Beyaz) // Eğer renk beyaz ise...
            {
                Ileri = Yon.Kuzey; // İleri yönü kuzey olarak ayarlar.
            }
            else if (renk == Oyuncu.Siyah) // Eğer renk siyah ise...
            {
                Ileri = Yon.Guney; // İleri yönü güney olarak ayarlar.
            }
        }
        #endregion

        #region Piyon nesnesinin bir kopyasını oluşturan metod
        public override Tas Kopya() // Piyon nesnesinin bir kopyasını oluşturan metod.
        {
            Piyon kopya = new Piyon(Renk); // Yeni bir Piyon nesnesi oluşturur ve rengini kopyalar.
            kopya.Tasindi = Tasindi; // Taşın hareket edip etmediği bilgisini kopyalar.
            return kopya; // Kopya Piyon nesnesini döndürür.
        }
        #endregion

        #region Piyonun verilen pozisyona ilerleyip ilerleyemeyeceğini kontrol eden metod
        private static bool Ilerleyebilirmi(Pozisyon poz, Tahta tahta) // Piyonun verilen pozisyona ilerleyip ilerleyemeyeceğini kontrol eden metod.
        {
            return Tahta.IcerideMi(poz) && tahta.BosMu(poz); // Pozisyon tahtanın içindeyse ve boşsa true döner.
        }
        #endregion

        #region Piyonun verilen pozisyondaki taşı yakalayıp yakalayamayacağını kontrol eden metod
        private bool Yakalama(Pozisyon poz, Tahta tahta) // Piyonun verilen pozisyondaki taşı yakalayıp yakalayamayacağını kontrol eden metod.
        {
            if (!Tahta.IcerideMi(poz) || tahta.BosMu(poz)) // Pozisyon tahtanın içinde değilse veya boşsa false döner.
            {
                return false; // False döner.
            }
            return tahta[poz].Renk != Renk; // Pozisyondaki taşın rengi piyonun renginden farklıysa (rakip taş) true döner.
        }
        #endregion

        #region Piyon terfi durumunda olası hamleleri oluşturan metod
        private static IEnumerable<Hamle> TerfiHamleleri(Pozisyon from, Pozisyon to) // Piyon terfi durumunda olası hamleleri oluşturan metod.
        {
            yield return new PiyonTerfi(from, to, TasTuru.At); // At terfisi.
            yield return new PiyonTerfi(from, to, TasTuru.Fil); // Fil terfisi.
            yield return new PiyonTerfi(from, to, TasTuru.Kale); // Kale terfisi.
            yield return new PiyonTerfi(from, to, TasTuru.Vezir); // Vezir terfisi.
            //Ama terfi olayı sadece düz değil başka bir taşı da çapraz yiyebileceği için:
        }
        #endregion

        #region Piyonun ileri hamlelerini hesaplayan metod
        private IEnumerable<Hamle> IleriHamleler(Pozisyon from, Tahta tahta) // Piyonun ileri hamlelerini hesaplayan metod.
        {
            Pozisyon birHamlePozisyonu = from + Ileri; // Bir kare ilerideki pozisyonu hesaplar.

            if (Ilerleyebilirmi(birHamlePozisyonu, tahta)) // Bir kare ilerideki pozisyona ilerlenebiliyorsa...
            {
                if (birHamlePozisyonu.Satir == 0 || birHamlePozisyonu.Satir == 7) // Piyon terfi satırına ulaştıysa...
                {
                    foreach (Hamle trfHamlesi in TerfiHamleleri(from, birHamlePozisyonu)) // Terfi hamleleri üzerinde döngü yapar.
                    {
                        yield return trfHamlesi; // Terfi hamlesini döndürür.
                    }
                }

                else // Piyon terfi satırına ulaşmadıysa...
                {
                    yield return new NormalHamle(from, birHamlePozisyonu); // Normal ileri hamleyi döndürür.
                }

                Pozisyon ikiHamlePozisyonu = birHamlePozisyonu + Ileri; // İki kare ilerideki pozisyonu hesaplar.

                if (!Tasindi && Ilerleyebilirmi(ikiHamlePozisyonu, tahta)) // Piyon daha önce hareket etmediyse ve iki kare ilerlenebiliyorsa...
                {
                    yield return new CiftPiyon(from, ikiHamlePozisyonu); // Çift piyon hamlesini döndürür.
                }
            }
        }
        #endregion

        #region Piyonun çapraz hamlelerini hesaplayan metod
        private IEnumerable<Hamle> CaprazHamleler(Pozisyon from, Tahta tahta) // Piyonun çapraz hamlelerini hesaplayan metod.
        {
            foreach (Yon yon in new Yon[] { Yon.Bati, Yon.Dogu }) // Batı ve doğu yönleri (çapraz) üzerinde döngü yapar.
            {
                Pozisyon to = from + Ileri + yon; // Çaprazdaki pozisyonu hesaplar.

                if (to == tahta.PiyonAtlamaPozisyonunuAl(Renk.Rakip())) // Çaprazdaki pozisyon en passant yakalama pozisyonuna eşitse...
                {
                    yield return new EnPassant(from, to); // En passant hamlesini döndürür.
                }

                else if (Yakalama(to, tahta)) // Çaprazdaki pozisyonda rakip taş varsa...
                {
                    if (to.Satir == 0 || to.Satir == 7) // Piyon terfi satırına ulaştıysa...
                    {
                        foreach (Hamle trfHamlesi in TerfiHamleleri(from, to)) // Terfi hamleleri üzerinde döngü yapar.
                        {
                            yield return trfHamlesi; // Terfi hamlesini döndürür.
                        }
                    }
                    else // Piyon terfi satırına ulaşmadıysa...
                    {
                        yield return new NormalHamle(from, to); // Normal çapraz yakalama hamlesini döndürür.
                    }

                }
            }
        }
        #endregion

        #region Piyonun yapabileceği tüm hamleleri (ileri ve çapraz) döndüren metod
        public override IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta) // Piyonun yapabileceği tüm hamleleri (ileri ve çapraz) döndüren metod.
        {
            return IleriHamleler(from, tahta).Concat(CaprazHamleler(from, tahta)); // İleri hamleler ve çapraz hamleleri birleştirir.
        }
        #endregion

        #region Piyonun rakip şahı ele geçirip geçiremeyeceğini kontrol eden metod
        public override bool RakipSahiEleGecirilebilir(Pozisyon from, Tahta tahta) // Piyonun rakip şahı ele geçirip geçiremeyeceğini kontrol eden metod.
        {
            return CaprazHamleler(from, tahta).Any(hamle => // Çapraz hamleler arasında şahı ele geçiren bir hamle olup olmadığını kontrol eder.
            {
                Tas tas = tahta[hamle.ToPos]; // Hedef pozisyondaki taşı alır.
                return tas != null && tas.Tur == TasTuru.Sah; // Taş şah ise true döner.
            });
        }
        #endregion
    }
}