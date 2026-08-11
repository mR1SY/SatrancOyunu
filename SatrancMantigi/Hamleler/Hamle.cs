using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{

    public abstract class Hamle // Tüm somut hamle sınıflarının temel sınıfı.
    {
        #region Özellikler
        public abstract HamleTuru Tur { get; } // Hamlenin türünü belirten özellik (abstract).
        public abstract Pozisyon FromPos { get; } // Hamlenin başlangıç pozisyonunu belirten özellik (abstract).
        public abstract Pozisyon ToPos { get; } // Hamlenin bitiş pozisyonunu belirten özellik (abstract).
        #endregion

        #region Yürütme Metodu Soyut Ana Kısım
        public abstract bool Execute(Tahta tahta); // Hamleyi tahta üzerinde uygulayan metod (abstract).

        #endregion

        #region Yasal Hamleleri Kontrol Eden Ana Metod
        public virtual bool Yasal(Tahta tahta) // Hamlenin yasal olup olmadığını kontrol eden metod.
        {
            Oyuncu oyuncu = tahta[FromPos].Renk; // Hamleyi yapan oyuncunun rengini alır.

            Tahta tahtaKopya = tahta.Kopya(); // Tahtanın bir kopyasını oluşturur.

            Execute(tahtaKopya); // Hamleyi kopya tahta üzerinde uygular.

            return !tahtaKopya.TehditAltinda(oyuncu); // Kopya tahtada oyuncunun şahı tehdit altında değilse true döner.
        }
        #endregion

        #region Txt'ye Aktarılırken Alınan Taşın X'e Aktarılma Ana Kısmı
        public bool TasSilindi { get; protected set; } = false; // Hamle sonucunda bir taşın silinip silinmediğini belirten özellik.
        #endregion

        #region Hamleyi UCI Formatına (Stockfish formatı) Çeviren Metod
        public virtual string UciFormatinaCevir()
        {
            // Sütunları harfe (0 -> a, 1 -> b), satırları sayıya (0 -> 8, 7 -> 1) çevirir
            string baslangic = $"{(char)('a' + FromPos.Sutun)}{8 - FromPos.Satir}";
            string bitis = $"{(char)('a' + ToPos.Sutun)}{8 - ToPos.Satir}";

            return baslangic + bitis; // Örn: "e2e4"
        }
        #endregion
    }
}