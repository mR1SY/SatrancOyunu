using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; // Dosya işlemleri için eklendi

namespace SatrancMantigi
{
    // Oyunun mevcut durumunu (tahta, mevcut oyuncu, sonuç vb.) temsil eden sınıf.
    public class OyunDurumu
    {
        #region Özellikler
        public Tahta Tahta { get; } // Oyun tahtasını tutar.
        public Oyuncu MevcutOyuncu { get; private set; } // Mevcut oyuncuyu tutar.
        public Sonuc Sonuc { get; private set; } = null; // Oyunun sonucunu tutar (başlangıçta null).
        #endregion

        #region Tanımlamalar
        private int YakalamaVeyaPiyonHamlesiYok = 0; // 50 hamle kuralı için sayaç(Toplam sayı/2).
        private string durumStringi; // Oyun durumunu temsil eden dize (FEN notasyonu).
        private readonly Dictionary<string, int> durumGecmisi = new Dictionary<string, int>(); // Oyun durumu geçmişini tutar (üç katlı tekrar kuralı için).
        #endregion

        #region Yapıcı Metod
        public OyunDurumu(Oyuncu oyuncu, Tahta tahta) // OyunDurumu nesnesini oyuncu ve tahta bilgileriyle oluşturan yapıcı metod.
        {
            MevcutOyuncu = oyuncu; // Mevcut oyuncuyu ayarlar.
            Tahta = tahta; // Tahtayı ayarlar.
            durumStringi = new DurumStringi(MevcutOyuncu, tahta).ToString(); // Oyun durumunu temsil eden dizeyi oluşturur.
            durumGecmisi[durumStringi] = 1; // Oyun durumu geçmişine ekler.
        }
        #endregion

        #region Verilen pozisyondaki taş için yasal hamleleri döndüren metod. Taş düzenleme modunda ise tüm hamleleri döndürür
        public IEnumerable<Hamle> TaslarIcinYasalHamleler(Pozisyon poz, bool tasDuzenlemeModu = false)
        // Verilen pozisyondaki taş için yasal hamleleri döndüren metod. Taş düzenleme modunda ise tüm hamleleri döndürür.
        {
            if (tasDuzenlemeModu) // Taş düzenleme modunda ise...
            {
                Tas tas = Tahta[poz]; // Pozisyondaki taşı alır.
                if (tas != null) // Taş varsa...
                {
                    return tas.HamleYapmak(poz, Tahta); // Taşın tüm hamlelerini döndürür.
                }
                return Enumerable.Empty<Hamle>(); // Boş bir hamle listesi döndürür.
            }

            if (Tahta.BosMu(poz) || Tahta[poz].Renk == MevcutOyuncu) // Pozisyon boşsa veya taş mevcut oyuncuya aitse...
            {
                Tas tas = Tahta[poz]; // Pozisyondaki taşı alır.
                if (tas != null) // Taş varsa...
                {
                    IEnumerable<Hamle> taslariHareketettir = tas.HamleYapmak(poz, Tahta); // Taşın tüm hamlelerini alır.
                    return taslariHareketettir.Where(hamle => hamle.Yasal(Tahta)); // Yasal olan hamleleri filtreler ve döndürür.
                }
            }

            return Enumerable.Empty<Hamle>(); // Boş bir hamle listesi döndürür.
        }
        #endregion

        #region Verilen oyuncu için tüm yasal hamleleri döndüren metod
        public IEnumerable<Hamle> ButunYasalHamlelerIcin(Oyuncu oyuncu)
        // Verilen oyuncu için tüm yasal hamleleri döndüren metod.
        {
            return Tahta.TasPozisyonlariIcin(oyuncu).SelectMany(pos => // Oyuncunun taşlarının bulunduğu pozisyonlar için...
            {
                Tas tas = Tahta[pos]; // Pozisyondaki taşı alır.
                return tas.HamleYapmak(pos, Tahta); // Taşın tüm hamlelerini alır.
            }).Where(hamle => hamle.Yasal(Tahta)); // Yasal olan hamleleri filtreler ve döndürür.
        }
        #endregion

        #region Hamleyi gerçekleştiren ve oyun durumunu güncelleyen metod
        public void HareketEt(Hamle hamle) // Hamleyi gerçekleştiren ve oyun durumunu güncelleyen metod.
        {
            if (hamle != null) // Hamle null değilse...
            {
                Tahta.PiyonAtlamaPozisyonunuAyarla(MevcutOyuncu, null); // En passant yakalama pozisyonunu sıfırlar.
                bool yakalaYaDaPiyon = hamle.Execute(Tahta); // Hamleyi uygular.

                if (yakalaYaDaPiyon) // Hamle sonucunda taş yakalandıysa veya piyon hareket ettiyse...
                {
                    YakalamaVeyaPiyonHamlesiYok = 0; // 50 hamle kuralı sayacını sıfırlar.
                    durumGecmisi.Clear(); // Oyun durumu geçmişini temizler.
                }
                else // Hamle sonucunda taş yakalanmadıysa veya piyon hareket etmediyse...
                {
                    YakalamaVeyaPiyonHamlesiYok++; // 50 hamle kuralı sayacını artırır.
                }

                MevcutOyuncu = MevcutOyuncu.Rakip(); // Mevcut oyuncuyu değiştirir.
                DurumStringiniGuncelle(); // Oyun durumu dizesini günceller.
                OyunBitisiKontrol(); // Oyun bitiş koşullarını kontrol eder.
            }
        }
        #endregion

        #region Oyun durumu dizesini günceller
        private void DurumStringiniGuncelle() // Oyun durumu dizesini günceller.
        {
            durumStringi = new DurumStringi(MevcutOyuncu, Tahta).ToString(); // Yeni durum dizesini oluşturur.

            if (!durumGecmisi.ContainsKey(durumStringi)) // Durum dizesi geçmişte yoksa...
            {
                durumGecmisi[durumStringi] = 1; // Geçmişe ekler.
            }
            else // Durum dizesi geçmişte varsa...
            {
                durumGecmisi[durumStringi]++; // Sayacını artırır.
            }
        }
        #endregion

        #region 50 hamle kuralını kontrol eden metod
        private bool ElliHamleKurali() // 50 hamle kuralını kontrol eden metod.
        {
            int tumHareketler = YakalamaVeyaPiyonHamlesiYok / 2; // Toplam hamle sayısını hesaplar.
            return tumHareketler == 50; // 50 hamle yapıldıysa true döner.
        }
        #endregion

        #region Üç katlı tekrar kuralını kontrol eden metod
        private bool UcKatliTekrar() // Üç katlı tekrar kuralını kontrol eden metod.
        {
            return durumGecmisi[durumStringi] == 3; // Aynı durum dizesi 3 kez tekrarlandıysa true döner.
        }
        #endregion

        #region Oyun bitiş koşullarını kontrol eden metod
        public void OyunBitisiKontrol() // Oyun bitiş koşullarını kontrol eden metod.
        {
            if (!ButunYasalHamlelerIcin(MevcutOyuncu).Any()) // Mevcut oyuncunun yasal hamlesi yoksa...
            {
                if (Tahta.TehditAltinda(MevcutOyuncu)) // Mevcut oyuncunun şahı tehdit altında ise...
                {
                    Sonuc = Sonuc.Kazanmak(MevcutOyuncu.Rakip()); // Rakip oyuncu kazanır (şah mat).
                }
                else // Mevcut oyuncunun şahı tehdit altında değilse...
                {
                    Sonuc = Sonuc.Beraberlik(BitisSebebi.Pat); // Oyun berabere biter (pat).
                }
            }
            else if (Tahta.YetersizMateryal()) // Yetersiz materyal varsa...
            {
                Sonuc = Sonuc.Beraberlik(BitisSebebi.YetersizTas); // Oyun berabere biter.
            }
            else if (ElliHamleKurali()) // 50 hamle kuralı gerçekleştiyse...
            {
                Sonuc = Sonuc.Beraberlik(BitisSebebi.ElliHamleKurali); // Oyun berabere biter.
            }
            else if (UcKatliTekrar()) // Üç katlı tekrar gerçekleştiyse...
            {
                Sonuc = Sonuc.Beraberlik(BitisSebebi.UcKatliTekrar); // Oyun berabere biter.
            }
        }
        #endregion

        #region Oyunun bitip bitmediğini kontrol eden metod
        public bool OyunBittiMi() // Oyunun bitip bitmediğini kontrol eden metod.
        {
            return Sonuc != null; // Sonuç null değilse oyun bitmiştir, true döner.
        }
        #endregion

        #region Oyunu bitiren ve sonucu kaydeden metod
        public void OyunuBitir(Oyuncu kazanan, BitisSebebi sebep) // Oyunu bitiren ve sonucu kaydeden metod.
        {
            Sonuc = new Sonuc(kazanan, sebep); // Sonuç nesnesini oluşturur.
        }
        #endregion

        #region Hamleyi txt dosyasına kaydeden metod
        public void HamleyiKaydet(Hamle hamle) // Hamleyi txt dosyasına kaydeden metod.
        {
            string hamleMetni = HamleMetniniOlustur(hamle, this); // Hamlenin metnini oluşturur.
            DosyayaYaz(hamleMetni); // Hamle metnini dosyaya yazar.
        }
        #endregion

        #region Hamlenin metnini oluşturan metod
        private string HamleMetniniOlustur(Hamle hamle, OyunDurumu oyunDurumu) // Hamlenin metnini oluşturan metod.
        {
            string oyuncu = MevcutOyuncu == Oyuncu.Beyaz ? "Siyah-" : "Beyaz-"; // Hamleyi yapan oyuncuyu belirler.

            if (hamle.Tur == HamleTuru.RokSahKanadi) // Şah kanadı rok ise...
            {
                return oyuncu + "0-0"; // "0-0" metnini döndürür.
            }
            else if (hamle.Tur == HamleTuru.RokVezirKanadi) // Vezir kanadı rok ise...
            {
                return oyuncu + "0-0-0"; // "0-0-0" metnini döndürür.
            }

            string tas = TasKisaltmasi(hamle.ToPos); // Taşın kısaltmasını alır.
            string hedefKare = KareAdi(hamle.ToPos); // Hedef karenin adını alır.
            string yakalama = hamle.TasSilindi ? "x" : ""; // Taş yakalandıysa "x" ekler.
            string terfi = ""; // Terfi bilgisi (başlangıçta boş).

            if (hamle.Tur == HamleTuru.PiyonTerfi) // Piyon terfi ise...
            {
                PiyonTerfi terfiHamle = (PiyonTerfi)hamle; // Hamleyi PiyonTerfi türüne dönüştürür.
                tas = "Piyon "; // Taş kısaltmasını "Piyon" olarak ayarlar.
                terfi = "=" + TasKisaltmasi(terfiHamle.ToPos); // Terfi edilen taşın kısaltmasını ekler.
            }

            string sahCekme = !oyunDurumu.OyunBittiMi() && Tahta.TehditAltinda(MevcutOyuncu) ? "+" : ""; // Şah çekme durumu varsa "+" ekler.
            string sahMat = oyunDurumu.OyunBittiMi() && oyunDurumu.Sonuc.Sebep == BitisSebebi.SahMat ? "#" : ""; // Şah mat durumu varsa "#" ekler.

            return oyuncu + tas + yakalama + hedefKare + terfi + sahCekme + sahMat; // Hamle metnini oluşturur ve döndürür.
        }
        #endregion

        #region Taşın kısaltmasını döndüren metod
        private string TasKisaltmasi(Pozisyon poz) // Taşın kısaltmasını döndüren metod.
        {
            if (Tahta.BosMu(poz)) // Pozisyon boşsa...
            {
                return ""; // Boş dize döndürür.
            }

            Tas tas = Tahta[poz]; // Pozisyondaki taşı alır.
            string c = tas.Tur switch // Taş türüne göre kısaltma seçer.
            {
                TasTuru.Piyon => "Piyon ", // Piyon
                TasTuru.At => "At ", // At
                TasTuru.Kale => "Kale ", // Kale
                TasTuru.Fil => "Fil ", // Fil
                TasTuru.Vezir => "Vezir ", // Vezir
                TasTuru.Sah => "Şah ", // Şah
                _ => " " // Diğer durumlarda boşluk
            };
            return c; // Taş kısaltmasını döndürür.
        }
        #endregion

        #region Karenin adını (örneğin "e4") döndüren metod
        private string KareAdi(Pozisyon poz) // Karenin adını (örneğin "e4") döndüren metod.
        {
            char dosya = (char)('a' + poz.Sutun); // Sütun bilgisini karaktere dönüştürür.
            int siralama = 8 - poz.Satir; // Satır bilgisini sayısal sıraya dönüştürür.
            return dosya.ToString() + siralama; // Karenin adını döndürür.
        }
        #endregion

        #region Metni dosyaya yazan metod
        private void DosyayaYaz(string metin) // Metni dosyaya yazan metod.
        {
            string dosyaYolu = "hamleler.txt"; // Dosya yolu.
            using (StreamWriter writer = new StreamWriter(dosyaYolu, true)) // Dosyayı açar ve yazma moduna geçirir.
            {
                writer.WriteLine(metin); // Metni dosyaya yazar.
            }
        }
        #endregion

        #region Hamle dosyasını silen metod
        public void HamleDosyasiniSil() // Hamle dosyasını silen metod.
        {
            string dosyaYolu = "hamleler.txt"; // Dosya yolu.
            if (File.Exists(dosyaYolu)) // Dosya varsa...
            {
                File.Delete(dosyaYolu); // Dosyayı siler.
            }
        }
        #endregion

        #region Tahta durumunu FEN formatında döndüren metod (Stockfish için)
        public string TahtaDurumunuFenYap()
        {
            // Zaten yazmış olduğun DurumStringi sınıfını kullanarak FEN üretir ve döndürür
            return new DurumStringi(MevcutOyuncu, Tahta).ToString();
        }
        #endregion
    }
}