using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    public class Rok : Hamle
    {
        #region Rok_Sınıfının_Özellikleri
        public override HamleTuru Tur { get; }
        public override Pozisyon FromPos { get; }
        public override Pozisyon ToPos { get; }

        private readonly Yon sahMoveDir;
        private readonly Pozisyon kaleFromPos;
        private readonly Pozisyon kaleToPos;
        #endregion

        #region Rok'un_Kurucusu
        public Rok(HamleTuru tur, Pozisyon sahPos)
        {
            Tur = tur;
            FromPos = sahPos;

            //Eğer şah kanadıysa
            if (tur == HamleTuru.KaleSahKanadi)
            {
                //Şah doğuya hareket etmeli
                sahMoveDir = Yon.Dogu;
                //Şah mevcut satırında 6. sütuna geçer
                ToPos = new Pozisyon(sahPos.Satir, 6);

                //Kale de 7. sütundan 5. sütuna gelir
                kaleFromPos = new Pozisyon(sahPos.Satir, 7);
                kaleToPos = new Pozisyon(sahPos.Satir, 5);
            }

            else if (tur == HamleTuru.KaleVezirKanadi)
            {
                //Şah batıya hareket etmeli
                sahMoveDir = Yon.Bati;
                //Şah mevcut satırında 2. sütuna geçer
                ToPos = new Pozisyon(sahPos.Satir, 2);

                //Kale de 0. sütundan 3. sütuna gelir
                kaleFromPos = new Pozisyon(sahPos.Satir, 0);
                kaleToPos = new Pozisyon(sahPos.Satir, 3);
            }
        }
        #endregion

        #region Rok'un_Tahtada_İşlemesi
        //Bu kısımda rok'un tahtada işlemesini yapıyoruz
        public override bool Execute(Tahta tahta)
        {
            new NormalHamle(FromPos, ToPos).Execute(tahta);
            new NormalHamle(kaleFromPos, kaleToPos).Execute(tahta);

            return false;
        }
        #endregion

        #region Rok'un_Yasal_Hamle_Tanımlaması
        //Şahın tehdit altında olmasını istemediğimizden dolayı rok hamlesinin izinlerini yasal metodunun üzerine yazıyoruz aynı metodu kullanarak
        public override bool Yasal(Tahta tahta)
        {
            //Şahın rengini kontrol ediyoruz
            Oyuncu oyuncu = tahta[FromPos].Renk;
            
            //Eğer tehdit altındaysa yasal değil
            if (tahta.TehditAltinda(oyuncu))
            {
                return false;
            }
            
            //Şahın hareket edeceği iki kareninde kontrol atında olup olmadığını kontrol edeceğiz

            //Bunu yapmak için tahtanın bir kopyasını oluşturuyoruz ve şahın mevcut konumunu
            Tahta kopya = tahta.Kopya();
            Pozisyon sahPozKopya = FromPos;


            for (int i = 0; i < 2; i++)
            {
                //Burada rok yapılırken şahın kopyasıyla nasıl konum işlemi yaptığını ifade ettik
                new NormalHamle(sahPozKopya, sahPozKopya + sahMoveDir).Execute(kopya);
                sahPozKopya += sahMoveDir;

                //Eğer şah taşındığı yerde kontrol altındaysa yine yasal hamle değildir ve false olarak döner
                if (kopya.TehditAltinda(oyuncu))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion
    }
}
