namespace SatrancMantigi
{
    // Tüm satranç taşları için temel sınıf (abstract). 
    public abstract class Tas
    {
        #region Özellikler
        public abstract TasTuru Tur { get; } // Taşın türünü belirten özellik (abstract).
        public abstract Oyuncu Renk { get; } // Taşın rengini belirten özellik (abstract).
        public bool Tasindi { get; set; } = false; // Taşın hareket edip etmediğini belirten özellik. Başlangıçta false olarak ayarlanır.
        #endregion

        #region Taşın bir kopyasını oluşturan soyut metod
        public abstract Tas Kopya(); // Taşın bir kopyasını oluşturan metod (abstract).
        public abstract IEnumerable<Hamle> HamleYapmak(Pozisyon from, Tahta tahta); // Taşın yapabileceği hamleleri döndüren metod (abstract).
        #endregion

        #region Taşın, verilen pozisyondan rakip şahı ele geçirip geçiremeyeceğini kontrol eden metod (virtual)
        public virtual bool RakipSahiEleGecirilebilir(Pozisyon from, Tahta tahta)
        // Taşın, verilen pozisyondan rakip şahı ele geçirip geçiremeyeceğini kontrol eden metod (virtual).
        {
            return HamleYapmak(from, tahta).Any(hamle => // Taşın yapabileceği hamleler arasında rakip şahı ele geçiren bir hamle olup olmadığını kontrol eder.
            {
                Tas tas = tahta[hamle.ToPos]; // Hedef pozisyondaki taşı alır.
                return tas != null && tas.Tur == TasTuru.Sah; // Taş şah ise true döner.
            });
        }
        #endregion

        #region Belirli bir yönde ulaşılabilen tüm konumları döndüren metod. Fil, Kale ve Vezir sınıfları tarafından kullanılır
        protected IEnumerable<Pozisyon> BelirliBirYondeUlasilabilirTumKonumlar(Pozisyon from, Tahta tahta, Yon yon)
        // Belirli bir yönde ulaşılabilen tüm konumları döndüren metod. Fil, Kale ve Vezir sınıfları tarafından kullanılır.
        {
            for (Pozisyon poz = from + yon; Tahta.IcerideMi(poz); poz += yon)
            // Başlangıç pozisyonundan başlayarak belirli bir yönde ilerler ve tahtanın içinde olan her pozisyonu kontrol eder.
            {
                if (tahta.BosMu(poz)) // Pozisyon boşsa...
                {
                    yield return poz; // Pozisyonu döndürür.
                    continue; // Döngünün bir sonraki adımına geçer.
                }

                Tas tas = tahta[poz]; // Pozisyondaki taşı alır.

                if (tas.Renk != Renk) // Taşın rengi farklıysa (rakip taş)...
                {
                    yield return poz; // Pozisyonu döndürür (yakalanabilir).
                }
                yield break; // Döngüyü sonlandırır (kendi taşı veya tahtanın sonu).
            }
        }
        #endregion

        #region Belirli yönlerde ulaşılabilen tüm konumları döndüren metod
        protected IEnumerable<Pozisyon> BelirliBirYondeUlasilabilirTumKonumlar(Pozisyon from, Tahta tahta, Yon[] yonler)
        // Belirli yönlerde ulaşılabilen tüm konumları döndüren metod. 
        {
            return yonler.SelectMany(yon => BelirliBirYondeUlasilabilirTumKonumlar(from, tahta, yon));
            // Verilen yönlerin her biri için ulaşılabilen konumları hesaplar ve birleştirir.
        }
        #endregion

    }
}