using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    //Bu sınıfın amacı her türden kaç taşa sahip olduğunu saklamak için kullanılacak
    public class Sayma
    {
        #region Sayma_Sınıfının_Tanımlama_Ve_Özellikleri
        //Burada hem beyaz hem siyah taşlar için birer sözlük tanımlıyoruz
        private readonly Dictionary<TasTuru, int> beyazSayisi = new();
        private readonly Dictionary<TasTuru, int> siyahSayisi = new();

        //Anahtar bir parça türüdür ve değer bu türdeki parçaların sayısıdır yani belli bir türde kaç parça olduğuna bakabiliriz

        //Toplam parça sayısı için bir özellik ekliyoruz
        public int ToplamSayi { get; private set; }
        #endregion

        #region Belli_Bir_Türde_Beyaz_Ve_Siyah_Taş_Sayısını_Sayma_İşlemi
        //Bu yapıcının içine her taş türü için giriş ekleyceğiz. Her iki sözlükte de bunu gibi tüm taş türleri üzerinde döngü yapabiliriz
        public Sayma()
        {
            foreach (TasTuru tur in Enum.GetValues(typeof(TasTuru)))
            {
                //her biri için hem beyaz sayısını hem de siayh sayısını sıfıra ayarladık
                beyazSayisi[tur] = 0;
                siyahSayisi[tur] = 0;
            }
        }

        //Başlangıçta bir artış ekleyelim, bir renk ve taş türü alır ve içinde karşılık gelen sayıyı arttırır, böylece renk beyazsa, tür için girişi beyaz sayımda arttırırz
        public void Artis(Oyuncu renk, TasTuru tur)
        {
            if (renk == Oyuncu.Beyaz)
            {
                beyazSayisi[tur]++;
            }
            //Aksi takdirde siyahsa siyah sayımdaki girişi arttırırz
            else if (renk == Oyuncu.Siyah)
            {
                siyahSayisi[tur]++;
            }
            //Ve ayrıca toplam sayıyı da arttırmamız gerekiyor
            ToplamSayi++;
        }
        //Belli bir türde beyaz taşların sayısını veren metod
        public int Beyaz(TasTuru tur)
        {
            return beyazSayisi[tur];
        }
        
        //Belli bir türde siyah taşların sayısını veren metod
        public int Siyah(TasTuru tur)
        {
            return siyahSayisi[tur];
        }
        #endregion
    }
}
