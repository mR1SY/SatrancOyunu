using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    public class EnPassant : Hamle
    {
        # region EnPassant_Özellikleri
        public override HamleTuru Tur => HamleTuru.EnPassant;
        public override Pozisyon FromPos { get; }
        public override Pozisyon ToPos { get; }

        private readonly Pozisyon pozisyonYakala;
        #endregion

        #region Yakalanan_Piyonun_Konumunu_Saklama
        public EnPassant(Pozisyon from, Pozisyon to)
        {
            FromPos = from;
            ToPos = to;
            pozisyonYakala = new Pozisyon(from.Satir, to.Sutun);
        }
        #endregion

        #region EnPassant_Yürütme_Metodu
        public override bool Execute(Tahta tahta)
        {
            new NormalHamle(FromPos, ToPos).Execute(tahta);
            tahta[pozisyonYakala] = null;

            TasSilindi = true;

            return true;
        }
        #endregion
    }
}
