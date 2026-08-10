using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    public class EnPassant : Hamle
    {
        #region Özellikler
        public override HamleTuru Tur => HamleTuru.EnPassant; // Hamle türünü EnPassant olarak tanımlar.
        public override Pozisyon FromPos { get; } // Hamlenin başlangıç pozisyonunu tutar.
        public override Pozisyon ToPos { get; } // Hamlenin bitiş pozisyonunu tutar.

        private readonly Pozisyon pozisyonYakala; // En passant ile yakalanacak piyonun pozisyonunu tutar.
        #endregion

        #region Yapıcı Metod
        public EnPassant(Pozisyon from, Pozisyon to) // EnPassant hamlesini başlangıç ve bitiş pozisyonlarıyla oluşturan yapıcı metod.
        {
            FromPos = from; // Başlangıç pozisyonunu from parametresinden alır.
            ToPos = to; // Bitiş pozisyonunu to parametresinden alır.
            pozisyonYakala = new Pozisyon(from.Satir, to.Sutun); // Yakalanacak piyonun pozisyonunu hesaplar (başlangıç pozisyonunun satırı ve bitiş pozisyonunun sütunu).
        }
        #endregion

        #region EnPassant Yürütme Metodu
        public override bool Execute(Tahta tahta) // EnPassant hamlesini tahta üzerinde uygulayan metod.
        {
            new NormalHamle(FromPos, ToPos).Execute(tahta); // İlk olarak piyonu normal bir hamle gibi hareket ettirir.
            tahta[pozisyonYakala] = null; // Yakalanan piyonu tahtadan kaldırır.

            TasSilindi = true; // Bir taşın silindiğini (yakalandığını) belirtir.

            return true; // Hamlenin başarılı olduğunu belirtir.
        }
        #endregion
    }
}