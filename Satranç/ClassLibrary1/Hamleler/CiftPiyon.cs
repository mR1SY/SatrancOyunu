using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    public class CiftPiyon : Hamle
    {
        #region Özellikler
        //Bu hamleyi(çift piyon) piyonun iki karesinde ilerlemek için kullancağız
        public override HamleTuru Tur => HamleTuru.CiftPiyon; // Hamle türünü CiftPiyon olarak tanımlar.
        public override Pozisyon FromPos { get; } // Hamlenin başlangıç pozisyonunu tutar.
        public override Pozisyon ToPos { get; } // Hamlenin bitiş pozisyonunu tutar.

        private readonly Pozisyon atlanmisPoz; // Piyonun çift hamlede atladığı pozisyonu tutar (en passant için).
        #endregion

        #region Yapıcı Metod
        public CiftPiyon(Pozisyon from, Pozisyon to) // ÇiftPiyon hamlesini başlangıç ve bitiş pozisyonlarıyla oluşturan yapıcı metod.
        {
            FromPos = from; // Başlangıç pozisyonunu from parametresinden alır.
            ToPos = to; // Bitiş pozisyonunu to parametresinden alır.

            atlanmisPoz = new Pozisyon((from.Satir + to.Satir) / 2, from.Sutun); // Atlanan pozisyonu hesaplar (başlangıç ve bitiş pozisyonlarının satırlarının ortalaması).
        }
        #endregion

        #region Çift Hamle Piyon Yürütme Metodu
        public override bool Execute(Tahta tahta) // ÇiftPiyon hamlesini tahta üzerinde uygulayan metod.
        {

            Oyuncu oyuncu = tahta[FromPos].Renk; // Hamleyi yapan oyuncunun rengini alır.

            tahta.PiyonAtlamaPozisyonunuAyarla(oyuncu, atlanmisPoz); // Atlama pozisyonunu tahtaya kaydeder (en passant için).

            new NormalHamle(FromPos, ToPos).Execute(tahta); // Piyonu normal bir hamle gibi hareket ettirir.

            return true; // Hamlenin başarılı olduğunu belirtir.
        }
        #endregion
    }
}