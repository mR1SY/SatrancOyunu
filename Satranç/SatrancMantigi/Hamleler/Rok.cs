using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    public class Rok : Hamle // Rok hamlesini temsil eden sınıf.
    {
        #region Özellikler
        public override HamleTuru Tur { get; } // Rok türünü (şah kanadı veya vezir kanadı) belirtir.
        public override Pozisyon FromPos { get; } // Şah'ın başlangıç pozisyonunu tutar.
        public override Pozisyon ToPos { get; } // Şah'ın bitiş pozisyonunu tutar.

        private readonly Yon sahMoveDir; // Şah'ın hareket yönünü (doğu veya batı) belirtir.
        private readonly Pozisyon kaleFromPos; // Kalenin başlangıç pozisyonunu tutar.
        private readonly Pozisyon kaleToPos; // Kalenin bitiş pozisyonunu tutar.
        #endregion

        #region Yapıcı Metod
        public Rok(HamleTuru tur, Pozisyon sahPos) // Rok hamlesini rok türü ve şah'ın pozisyonuyla oluşturan yapıcı metod.
        {
            Tur = tur; // Rok türünü (şah kanadı veya vezir kanadı) tur parametresinden alır.
            FromPos = sahPos; // Şah'ın başlangıç pozisyonunu sahPos parametresinden alır.

            if (tur == HamleTuru.RokSahKanadi) // Rok türü şah kanadı ise...
            {
                sahMoveDir = Yon.Dogu; // Şah'ın hareket yönünü doğu olarak ayarlar.
                ToPos = new Pozisyon(sahPos.Satir, 6); // Şah'ın bitiş pozisyonunu (satır, 6) olarak ayarlar.

                kaleFromPos = new Pozisyon(sahPos.Satir, 7); // Kalenin başlangıç pozisyonunu (satır, 7) olarak ayarlar.
                kaleToPos = new Pozisyon(sahPos.Satir, 5); // Kalenin bitiş pozisyonunu (satır, 5) olarak ayarlar.
            }

            else if (tur == HamleTuru.RokVezirKanadi) // Rok türü vezir kanadı ise...
            {
                sahMoveDir = Yon.Bati; // Şah'ın hareket yönünü batı olarak ayarlar.
                ToPos = new Pozisyon(sahPos.Satir, 2); // Şah'ın bitiş pozisyonunu (satır, 2) olarak ayarlar.

                kaleFromPos = new Pozisyon(sahPos.Satir, 0); // Kalenin başlangıç pozisyonunu (satır, 0) olarak ayarlar.
                kaleToPos = new Pozisyon(sahPos.Satir, 3); // Kalenin bitiş pozisyonunu (satır, 3) olarak ayarlar.
            }
        }
        #endregion

        #region Rok Yürütme Metodu
        public override bool Execute(Tahta tahta) // Rok hamlesini tahta üzerinde uygulayan metod.
        {
            new NormalHamle(FromPos, ToPos).Execute(tahta); // Şah'ı normal bir hamle gibi hareket ettirir.
            new NormalHamle(kaleFromPos, kaleToPos).Execute(tahta); // Kaleyi normal bir hamle gibi hareket ettirir.

            return false; // Rok hamlesinde taş silinmediği için false döner.
        }
        #endregion

        #region Rok Hamlesinin Yasal Olup Olmadığını Kontrol Eden Metod
        public override bool Yasal(Tahta tahta) // Rok hamlesinin yasal olup olmadığını kontrol eden metod.
        {
            Oyuncu oyuncu = tahta[FromPos].Renk; // Hamleyi yapan oyuncunun rengini alır.

            if (tahta.TehditAltinda(oyuncu)) // Şah tehdit altında ise rok yasal değildir.
            {
                return false; // False döner.
            }

            Tahta kopya = tahta.Kopya(); // Tahtanın bir kopyasını oluşturur.
            Pozisyon sahPozKopya = FromPos; // Şah'ın pozisyonunu kopyalar.


            for (int i = 0; i < 2; i++) // Şah'ın hareket edeceği iki kareyi kontrol eder.
            {
                new NormalHamle(sahPozKopya, sahPozKopya + sahMoveDir).Execute(kopya); // Şah'ı bir kare hareket ettirir.
                sahPozKopya += sahMoveDir; // Şah'ın yeni pozisyonunu günceller.

                if (kopya.TehditAltinda(oyuncu)) // Şah tehdit altında ise rok yasal değildir.
                {
                    return false; // False döner.
                }
            }

            return true; // Rok hamlesi yasal ise true döner.
        }
        #endregion
    }
}