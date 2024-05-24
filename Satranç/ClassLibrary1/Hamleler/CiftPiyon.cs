using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    public class CiftPiyon : Hamle
    {
        //Bu hamleyi(çift piyon) piyonun iki karesinde ilerlemek için kullancağız
        #region Çift_Piyon_Özellikleri
        public override HamleTuru Tur => HamleTuru.CiftPiyon;
        public override Pozisyon FromPos { get; }
        public override Pozisyon ToPos { get; }

        //En passant için bir değişken ekliyoruz
        private readonly Pozisyon atlanmisPoz;

        #endregion

        #region Çift_Piyon_Konum_İlerlemesi
        //Bu hareketi oluşturduğumuzda, başlangıç ve bitiş kanumlarını saklayan bir yapıcı ekliyoruz
        public CiftPiyon(Pozisyon from, Pozisyon to)
        {
            FromPos = from;
            ToPos = to;
            
            //Bu hareketi oluşturduğumuzda iki konum her zaman ön konum + iki adım ileriye eşit olacaktır bu nedenle atlama konumu arasındaki satırda olmalıdır
            atlanmisPoz = new Pozisyon((from.Satir + to.Satir) / 2, from.Sutun);
        }
        #endregion

        #region Çift_Piyon_Yürütme_Metodu
        public override bool Execute(Tahta tahta)
        {
            //Önce oyuncuyu alıyoruz 
            Oyuncu oyuncu = tahta[FromPos].Renk;
            //ve ardından atlama pozisyonunun panosunda saklıyoruz
            tahta.PiyonAtlamaPozisyonunuAyarla(oyuncu, atlanmisPoz);
            //Son olarak piyonu kullanarak hareket ettiriyoruz normal bir hamle olarak
            new NormalHamle(FromPos, ToPos).Execute(tahta);

            return true;
        }
        #endregion
    }
}
