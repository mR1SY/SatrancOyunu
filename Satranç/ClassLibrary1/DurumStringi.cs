using System.Text;

namespace SatrancMantigi
{
    // Satranç tahtasının durumunu Stockfish'in anladığı uluslararası FEN formatında oluşturur.
    public class DurumStringi
    {
        private readonly StringBuilder sb = new StringBuilder();

        #region Yapıcı metod
        public DurumStringi(Oyuncu mevcutOyuncu, Tahta tahta)
        {
            ParcaKonumuEkle(tahta);
            sb.Append(' ');
            MevcutOyuncuyuEkle(mevcutOyuncu);
            sb.Append(' ');
            RokHaklariEkle(tahta);
            sb.Append(' ');
            EnPassantEkle(tahta, mevcutOyuncu);
        }
        #endregion

        #region Durum dizesini döndürür
        public override string ToString()
        {
            return sb.ToString();
        }
        #endregion

        #region Taşların konumlarını dizeye ekler
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
        #endregion

        #region Verilen satırın taş bilgilerini dizeye ekler
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
        #endregion

        #region Taş türüne karşılık gelen FEN karakterini döndürür (Uluslararası Standart)
        private static char TasKarakteri(Tas tas)
        {
            // Stockfish standartları: p=piyon, n=at(knight), r=kale(rook), b=fil(bishop), q=vezir(queen), k=şah(king)
            char c = tas.Tur switch
            {
                TasTuru.Piyon => 'p',
                TasTuru.At => 'n',
                TasTuru.Kale => 'r',
                TasTuru.Fil => 'b',
                TasTuru.Vezir => 'q',
                TasTuru.Sah => 'k',
                _ => ' '
            };

            if (tas.Renk == Oyuncu.Beyaz)
            {
                return char.ToUpper(c); // Beyaz taşlar büyük harf (P, N, R, B, Q, K)
            }

            return c; // Siyah taşlar küçük harf (p, n, r, b, q, k)
        }
        #endregion

        #region Mevcut oyuncuyu FEN formatına uygun ekler
        private void MevcutOyuncuyuEkle(Oyuncu mevcutOyuncu)
        {
            if (mevcutOyuncu == Oyuncu.Beyaz)
            {
                sb.Append('w'); // Uluslararası FEN: w (White)
            }
            else
            {
                sb.Append('b'); // Uluslararası FEN: b (Black)
            }
        }
        #endregion

        #region Rok haklarını FEN formatına uygun ekler
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
                sb.Append('K'); // Beyaz şah kanadı
            }
            if (rokBeyazVezirKanadi)
            {
                sb.Append('Q'); // Beyaz vezir kanadı
            }
            if (rokSiyahSahKanadi)
            {
                sb.Append('k'); // Siyah şah kanadı
            }
            if (rokSiyahVezirKanadi)
            {
                sb.Append('q'); // Siyah vezir kanadı
            }
        }
        #endregion

        #region En passant bilgilerini dizeye ekler
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
        #endregion
    }
}