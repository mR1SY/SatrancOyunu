using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    #region Tahta_İçin_Özel_Konumsal_Yapılanma
    public class DurumStringi
    {
        private readonly StringBuilder sb = new StringBuilder();

        public DurumStringi(Oyuncu mevcutOyuncu, Tahta tahta)
        {
            ParcaKonumuEkle(tahta);
            sb.Append(' ');
            MevcutOyuncuyuEkle(mevcutOyuncu);
            sb.Append(' ');
            RokHaklariEkle(tahta);
            sb.Append(' ');
            EnPassantEkle(tahta, mevcutOyuncu);
            sb.Append(' ');
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        private static char TasKarakteri(Tas tas)
        {
            char c = tas.Tur switch
            {
                TasTuru.Piyon => 'p',
                TasTuru.At => 'a',
                TasTuru.Kale => 'k',
                TasTuru.Fil => 'f',
                TasTuru.Vezir => 'v',
                TasTuru.Sah => 's',
                _ => ' '
            };

            if (tas.Renk == Oyuncu.Beyaz)
            {
                return char.ToUpper(c);
            }

            return c;
        }

        private void SatirVerisiEkle(Tahta tahta, int satir)
        {
            int bos = 0;

            for (int c = 0; c < 8; c++)
            {
                if (tahta[satir, c] == null)
                {
                    bos++;
                    continue;
                }

                if (bos > 0)
                {
                    sb.Append(bos);
                    bos = 0;
                }

                sb.Append(TasKarakteri(tahta[satir, c]));
            }

            if (bos > 0)
            {
                sb.Append(bos);
            }
        }

        private void ParcaKonumuEkle(Tahta tahta)
        {
            for (int r = 0; r < 8; r++)
            {
                if (r != 0)
                {
                    sb.Append('/');
                }

                SatirVerisiEkle(tahta, r);
            }
        }

        private void MevcutOyuncuyuEkle(Oyuncu mevcutOyuncu)
        {
            if (mevcutOyuncu == Oyuncu.Beyaz)
            {
                sb.Append('b');
            }
            else
            {
                sb.Append('s');
            }
        }

        private void RokHaklariEkle(Tahta tahta)
        {
            bool rokBeyazSahKanadi = tahta.RokHakkiSahKanadi(Oyuncu.Beyaz);
            bool rokBeyazVezirKanadi = tahta.RokHakkiVezirKanadi(Oyuncu.Beyaz);
            bool rokSiyahSahKanadi = tahta.RokHakkiSahKanadi(Oyuncu.Siyah);
            bool rokSiyahVezirKanadi = tahta.RokHakkiVezirKanadi(Oyuncu.Siyah);

            if (!(rokBeyazSahKanadi || rokBeyazVezirKanadi || rokSiyahSahKanadi || rokSiyahVezirKanadi))
            {
                sb.Append('-');
                return;
            }

            if (rokBeyazSahKanadi)
            {
                sb.Append('S');
            }
            if (rokBeyazVezirKanadi)
            {
                sb.Append('V');
            }
            if (rokSiyahSahKanadi)
            {
                sb.Append('s');
            }
            if (rokSiyahVezirKanadi)
            {
                sb.Append('v');
            }
        }

        private void EnPassantEkle(Tahta tahta, Oyuncu mevcutOyuncu)
        {
            if (!tahta.EnPassantYakalayabilirMi(mevcutOyuncu))
            {
                sb.Append('-');
                return;
            }

            Pozisyon poz = tahta.PiyonAtlamaPozisyonunuAl(mevcutOyuncu.Rakip());
            char dosya = (char)('a' + poz.Sutun);
            int siralama = 8 - poz.Satir;
            sb.Append(dosya);
            sb.Append(siralama);
        }
    }
    #endregion
}
