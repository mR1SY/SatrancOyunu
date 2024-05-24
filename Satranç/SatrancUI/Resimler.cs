using SatrancMantigi;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SatrancUI
{
    public static class Resimler
    {
        #region Resim_Yükleme_Kök

        //Gövdede parametre olarak bir görüntünün göreceli yolunu alır yeni bir bitmap görüntüsü döndürürüz.
        public static ImageSource ResimYukle(string DosyaYolu)
        {
            return new BitmapImage(new Uri(DosyaYolu, UriKind.Relative));
        }
        // Bitmap: (WPF) tarafından sağlanan ve bitmap resimleri yüklemek için kullanılan bir sınıftır.
        //Uri: bir dosyanın veya bir web kaynağının konumunu tanımlamak için kullanılan bir sınıftır.
        //UriKind.Relative: Uri'nin bir dosya yoluna göre yorumlanması gerektiğini belirtir.
        //ImageSource: WPF'de, bir görüntüyü temsil etmek için kullanılan bir sınıftır.
        #endregion


        #region Resim_Yükleme_Gövde

        //Görüntüleri sözlüğe yüklediğimiz kısım

        private static readonly Dictionary<TasTuru, ImageSource> BeyazKaynaklar = new()
        {
            { TasTuru.Piyon, ResimYukle("Assets/BeyazPiyon.png") },
            { TasTuru.Fil, ResimYukle("Assets/BeyazFil.png") },
            { TasTuru.At, ResimYukle("Assets/BeyazAt.png") },
            { TasTuru.Kale, ResimYukle("Assets/BeyazKale.png") },
            { TasTuru.Vezir, ResimYukle("Assets/BeyazVezir.png") },
            { TasTuru.Sah, ResimYukle("Assets/BeyazSah.png") }
        };

        private static readonly Dictionary<TasTuru, ImageSource> SiyahKaynaklar = new()
        {
            { TasTuru.Piyon, ResimYukle("Assets/SiyahPiyon.png") },
            { TasTuru.Fil, ResimYukle("Assets/SiyahFil.png") },
            { TasTuru.At, ResimYukle("Assets/SiyahAt.png") },
            { TasTuru.Kale, ResimYukle("Assets/SiyahKale.png") },
            { TasTuru.Vezir, ResimYukle("Assets/SiyahVezir.png") },
            { TasTuru.Sah, ResimYukle("Assets/SiyahSah.png") }
        };
        //Dictionary: Anahtar-değer çiftleri dediğimiz veri koleksiyonlarını saklamak için kullanılan güçlü bir veri yapıdır.

        #endregion


        #region Resimleri_Kütüphaneden_Çekme

        //Oyuncu beyazsa beyaz kaynakları, siyahsa siyah kaynakları çıkarırız
        public static ImageSource ResimAl(Oyuncu renk, TasTuru tur)
        {
            return renk switch
            {
                Oyuncu.Beyaz => BeyazKaynaklar[tur],
                Oyuncu.Siyah => SiyahKaynaklar[tur],
                _ => null
            };
        }
        
        //Bu duruma asla ulaşmayız ama bunu koymazsak VisualStudio şikayet eder :)
        public static ImageSource ResimAl(Tas tas)
        {
            if (tas == null)
            {
                return null;
            }
            return ResimAl(tas.Renk, tas.Tur); 
        }
        //Olası bir hata durumunda konum boşsa boştur boş değilse uygun renkteki ve türdeki görüntüyü al

        #endregion
    }
}
