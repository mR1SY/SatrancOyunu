using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    // Satranç tahtasının durumunu temsil eden bir dize oluşturur (FEN notasyonu).
    public class DurumStringi
    {

        private readonly StringBuilder sb = new StringBuilder(); // Dize oluşturmak için StringBuilder nesnesi.

        #region Yapıcı metod
        public DurumStringi(Oyuncu mevcutOyuncu, Tahta tahta) // DurumStringi nesnesini mevcut oyuncu ve tahta bilgileriyle oluşturan yapıcı metod.
        {
            ParcaKonumuEkle(tahta); // Taşların konumlarını dizeye ekler.
            sb.Append(' '); // Boşluk ekler.
            MevcutOyuncuyuEkle(mevcutOyuncu); // Mevcut oyuncuyu dizeye ekler.
            sb.Append(' '); // Boşluk ekler.
            RokHaklariEkle(tahta); // Rok haklarını dizeye ekler.
            sb.Append(' '); // Boşluk ekler.
            EnPassantEkle(tahta, mevcutOyuncu); // En passant bilgilerini dizeye ekler.
            sb.Append(' '); // Boşluk ekler.
        }
        #endregion

        #region Durum dizesini döndürür
        public override string ToString() // Durum dizesini döndürür.
        {
            return sb.ToString(); // StringBuilder nesnesindeki dizeyi döndürür.
        }
        #endregion

        #region Taşların konumlarını dizeye ekler
        private void ParcaKonumuEkle(Tahta tahta) // Taşların konumlarını dizeye ekler.
        {
            for (int r = 0; r < 8; r++) // Satırlar üzerinde döngü yapar.
            {
                if (r != 0) // İlk satır değilse...
                {
                    sb.Append('/'); // Satırları ayırmak için "/" karakterini ekler.
                }

                SatirVerisiEkle(tahta, r); // Satırın taş bilgilerini dizeye ekler.
            }
        }
        #endregion

        #region Verilen satırın taş bilgilerini dizeye ekler
        private void SatirVerisiEkle(Tahta tahta, int satir) // Verilen satırın taş bilgilerini dizeye ekler.
        {
            int bos = 0; // Boş kare sayacı.

            for (int c = 0; c < 8; c++) // Sütunlar üzerinde döngü yapar.
            {
                if (tahta[satir, c] == null) // Kare boşsa...
                {
                    bos++; // Boş kare sayacını artırır.
                    continue; // Döngünün bir sonraki adımına geçer.
                }

                if (bos > 0) // Boş kare varsa...
                {
                    sb.Append(bos); // Boş kare sayısını dizeye ekler.
                    bos = 0; // Boş kare sayacını sıfırlar.
                }

                sb.Append(TasKarakteri(tahta[satir, c])); // Taşın karakterini dizeye ekler.
            }

            if (bos > 0) // Boş kare varsa...
            {
                sb.Append(bos); // Boş kare sayısını dizeye ekler.
            }
        }
        #endregion

        #region Taş türüne karşılık gelen karakteri döndürür
        private static char TasKarakteri(Tas tas) // Taş türüne karşılık gelen karakteri döndürür.
        {
            char c = tas.Tur switch // Taş türüne göre karakter seçer.
            {
                TasTuru.Piyon => 'p', // Piyon
                TasTuru.At => 'a', // At
                TasTuru.Kale => 'k', // Kale
                TasTuru.Fil => 'f', // Fil
                TasTuru.Vezir => 'v', // Vezir
                TasTuru.Sah => 's', // Şah
                _ => ' ' // Boş kare
            };

            if (tas.Renk == Oyuncu.Beyaz) // Taş beyaz ise...
            {
                return char.ToUpper(c); // Karakteri büyük harfe dönüştürür.
            }

            return c; // Karakteri küçük harf olarak döndürür.
        }
        #endregion

        #region Mevcut oyuncuyu dizeye ekler
        private void MevcutOyuncuyuEkle(Oyuncu mevcutOyuncu) // Mevcut oyuncuyu dizeye ekler.
        {
            if (mevcutOyuncu == Oyuncu.Beyaz) // Mevcut oyuncu beyaz ise...
            {
                sb.Append('b'); // "b" karakterini ekler.
            }
            else // Mevcut oyuncu siyah ise...
            {
                sb.Append('s'); // "s" karakterini ekler.
            }
        }
        #endregion

        #region Rok haklarını dizeye ekler
        private void RokHaklariEkle(Tahta tahta) // Rok haklarını dizeye ekler.
        {
            bool rokBeyazSahKanadi = tahta.RokHakkiSahKanadi(Oyuncu.Beyaz); // Beyazın şah kanadı rok hakkı.
            bool rokBeyazVezirKanadi = tahta.RokHakkiVezirKanadi(Oyuncu.Beyaz); // Beyazın vezir kanadı rok hakkı.
            bool rokSiyahSahKanadi = tahta.RokHakkiSahKanadi(Oyuncu.Siyah); // Siyahın şah kanadı rok hakkı.
            bool rokSiyahVezirKanadi = tahta.RokHakkiVezirKanadi(Oyuncu.Siyah); // Siyahın vezir kanadı rok hakkı.

            if (!(rokBeyazSahKanadi || rokBeyazVezirKanadi || rokSiyahSahKanadi || rokSiyahVezirKanadi)) // Hiçbir rok hakkı yoksa...
            {
                sb.Append('-'); // "-" karakterini ekler.
                return; // Metodu sonlandırır.
            }

            if (rokBeyazSahKanadi) // Beyazın şah kanadı rok hakkı varsa...
            {
                sb.Append('S'); // "S" karakterini ekler.
            }
            if (rokBeyazVezirKanadi) // Beyazın vezir kanadı rok hakkı varsa...
            {
                sb.Append('V'); // "V" karakterini ekler.
            }
            if (rokSiyahSahKanadi) // Siyahın şah kanadı rok hakkı varsa...
            {
                sb.Append('s'); // "s" karakterini ekler.
            }
            if (rokSiyahVezirKanadi) // Siyahın vezir kanadı rok hakkı varsa...
            {
                sb.Append('v'); // "v" karakterini ekler.
            }
        }
        #endregion

        #region En passant bilgilerini dizeye ekler
        private void EnPassantEkle(Tahta tahta, Oyuncu mevcutOyuncu) // En passant bilgilerini dizeye ekler.
        {
            if (!tahta.EnPassantYakalayabilirMi(mevcutOyuncu)) // En passant yakalama mümkün değilse...
            {
                sb.Append('-'); // "-" karakterini ekler.
                return; // Metodu sonlandırır.
            }

            Pozisyon poz = tahta.PiyonAtlamaPozisyonunuAl(mevcutOyuncu.Rakip()); // En passant yakalama pozisyonunu alır.
            char dosya = (char)('a' + poz.Sutun); // Sütun bilgisini karaktere dönüştürür.
            int siralama = 8 - poz.Satir; // Satır bilgisini sayısal sıraya dönüştürür.
            sb.Append(dosya); // Sütun karakterini dizeye ekler.
            sb.Append(siralama); // Satır numarasını dizeye ekler.
        }
        #endregion
    }
}