using SatrancMantigi;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SatrancUI
{
    // Satranç taşlarının resimlerini yüklemek ve almak için yardımcı sınıf.
    public static class Resimler
    {
        private static readonly Dictionary<TasTuru, ImageSource> BeyazKaynaklar = new() // Beyaz taşların resim kaynaklarını tutan sözlük.
        {
            { TasTuru.Piyon, ResimYukle("Assets/BeyazPiyon.png") }, // Beyaz piyon resmi.
            { TasTuru.Fil, ResimYukle("Assets/BeyazFil.png") }, // Beyaz fil resmi.
            { TasTuru.At, ResimYukle("Assets/BeyazAt.png") }, // Beyaz at resmi.
            { TasTuru.Kale, ResimYukle("Assets/BeyazKale.png") }, // Beyaz kale resmi.
            { TasTuru.Vezir, ResimYukle("Assets/BeyazVezir.png") }, // Beyaz vezir resmi.
            { TasTuru.Sah, ResimYukle("Assets/BeyazSah.png") } // Beyaz şah resmi.
        };

        private static readonly Dictionary<TasTuru, ImageSource> SiyahKaynaklar = new() // Siyah taşların resim kaynaklarını tutan sözlük.
        {
            { TasTuru.Piyon, ResimYukle("Assets/SiyahPiyon.png") }, // Siyah piyon resmi.
            { TasTuru.Fil, ResimYukle("Assets/SiyahFil.png") }, // Siyah fil resmi.
            { TasTuru.At, ResimYukle("Assets/SiyahAt.png") }, // Siyah at resmi.
            { TasTuru.Kale, ResimYukle("Assets/SiyahKale.png") }, // Siyah kale resmi.
            { TasTuru.Vezir, ResimYukle("Assets/SiyahVezir.png") }, // Siyah vezir resmi.
            { TasTuru.Sah, ResimYukle("Assets/SiyahSah.png") } // Siyah şah resmi.
        };

        public static ImageSource ResimYukle(string DosyaYolu) // Verilen dosya yolundan bir ImageSource nesnesi oluşturur.
        {
            return new BitmapImage(new Uri(DosyaYolu, UriKind.Relative)); // BitmapImage nesnesi oluşturur ve döndürür.
        }

        public static ImageSource ResimAl(Oyuncu renk, TasTuru tur) // Verilen renkte ve türdeki taşın resmini döndürür.
        {
            return renk switch // Renk bilgisine göre uygun resim kaynağını döndürür.
            {
                Oyuncu.Beyaz => BeyazKaynaklar[tur], // Beyaz taşlar için.
                Oyuncu.Siyah => SiyahKaynaklar[tur], // Siyah taşlar için.
                _ => null // Diğer durumlarda null.
            };
        }

        public static ImageSource ResimAl(Tas tas) // Verilen taşın resmini döndürür.
        {
            if (tas == null) // Taş null ise...
            {
                return null; // Null döndürür.
            }
            return ResimAl(tas.Renk, tas.Tur); // Taşın rengine ve türüne göre resmini alır ve döndürür.
        }
    }
}