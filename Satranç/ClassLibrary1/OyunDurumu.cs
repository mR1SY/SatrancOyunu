using System.Diagnostics;

namespace SatrancMantigi
{
    public class OyunDurumu
    {
        #region Oyun_Durumu_Özellikleri
        public Tahta Tahta { get; }
        public Oyuncu MevcutOyuncu { get; private set; }

        //Başlangıçta boş ama oyun bitişinde şah-mat'ı ve pat'ı tespit etmek için gerçek sonucu burada saklayacağız
        public Sonuc Sonuc { get; private set; } = null;

        //50 Hamle kuralı için ana değişken
        private int YakalamaVeyaPiyonHamlesiYok = 0;

        private string durumStringi;

        private readonly Dictionary<string, int> durumGecmisi = new Dictionary<string, int>();

        #endregion

        public void OyunuBitir(Oyuncu kazanan, BitisSebebi sebep)
        {
            Sonuc = new Sonuc(kazanan, sebep);
            // Oyun bitişine dair diğer işlemleri burada gerçekleştirebilirsin.
        }

        #region Oyuncu_Ve_Tahta_Durumunu_Saklama
        public OyunDurumu(Oyuncu oyuncu, Tahta tahta)
        {
            MevcutOyuncu = oyuncu;  
            Tahta = tahta;

            durumStringi = new DurumStringi(MevcutOyuncu, tahta).ToString();
            durumGecmisi[durumStringi] = 1;
        }
        #endregion

        #region Oyuncunun_Rakip_Oyuncunun_Taşına_Erişim_Engelleme
        //Oyun sırası mevcut bir oyuncuyken karşı tarafın oynamasını engellediğimiz komut burası

        public IEnumerable<Hamle> TaslarIcinYasalHamleler(Pozisyon poz, bool tasDuzenlemeModu = false)
        {
            // Taş düzenleme modunda boş bir liste döndür
            if (tasDuzenlemeModu)
            {
                Tas tas = Tahta[poz];
                if (tas != null)
                {
                    return tas.HamleYapmak(poz, Tahta); // Yasal kontrolü yapma
                }
                return Enumerable.Empty<Hamle>();
            }

            // Eğer pozisyon boşsa veya taş mevcut oyuncuya aitse
            if (Tahta.BosMu(poz) || Tahta[poz].Renk == MevcutOyuncu)
            {
                Tas tas = Tahta[poz];
                if (tas != null)
                {
                    // Yapabileceği tüm hamleleri çağırıyoruz
                    IEnumerable<Hamle> taslariHareketettir = tas.HamleYapmak(poz, Tahta);
                    return taslariHareketettir.Where(hamle => hamle.Yasal(Tahta));
                }
            }

            // Diğer durumlarda boş bir hamle listesi döndür
            return Enumerable.Empty<Hamle>();
        }
        #endregion

        //50 hamle değişken artırımı bu bölgenin içinde
        #region Piyon_Hareket_Kontrol

        public void HareketEt(Hamle hamle)
        {
            //Basitce oyuncunun enpassant konumunu sıfır olarak ayarlıyoruz
            Tahta.PiyonAtlamaPozisyonunuAyarla(MevcutOyuncu, null);
            bool yakalaYaDaPiyon = hamle.Execute(Tahta);

            if (yakalaYaDaPiyon)
            {
                YakalamaVeyaPiyonHamlesiYok = 0;
                durumGecmisi.Clear();
            }
            else
            {
                YakalamaVeyaPiyonHamlesiYok++;
            }

            MevcutOyuncu = MevcutOyuncu.Rakip();
            DurumStringiniGuncelle();
            OyunBitisiKontrol();
        }
        #endregion

        #region Legal_Ve_İllegal_Hamlelerin_Tamamının_Üretimi

        //Mevcut oyuncunun yapabileceği tüm hamleleri koleksiyon olarak veriyor ve buna kural dışı olanlar da dahil
        public IEnumerable<Hamle> ButunYasalHamlelerIcin(Oyuncu oyuncu)
        {
            //Burada tüm aday hamleler için değişken oluşturacağız
            //Oyuncuya ait bir taş içeren tüm pozisyonları dikkate alacağız
            //Ve daha sonra birçok seç seçeneğini kullanarak her parça için hamleleri topluyoruz
            IEnumerable<Hamle> AdaylariTasima = Tahta.TasPozisyonlariIcin(oyuncu).SelectMany(pos =>
            {
                Tas tas = Tahta[pos];
                return tas.HamleYapmak(pos, Tahta);
            });
            //Kural dışı olanları yani yasal olmayanları filtreliyoruz
            return AdaylariTasima.Where(move => move.Yasal(Tahta));
        }

        #endregion

        #region Oyunun_Nasıl_Bittiğinin_Kontrolü

        //Mevcut oyuncu değiştirildikten sonra sonra her turun sonunda çağrılacaktır
        public void OyunBitisiKontrol()
        {
            //Eğer yeni oyunun herhangi bir yasal hamlesi yoksa oyun kesinlikle biter
            if (!ButunYasalHamlelerIcin(MevcutOyuncu).Any())
            {
                //Ama bu bitiş bir şah-mat mı yoksa pat mı?
                //Eğer mevcut oyuncu şahtaysa
                if (Tahta.TehditAltinda(MevcutOyuncu))
                {
                    //O zaman mat olur ve diğer oyuncu kazanır
                    Sonuc = Sonuc.Kazanmak(MevcutOyuncu.Rakip());
                }

                //Aksi halde eğer mevcut oyuncu şahta değilse o zaman pat durumuna girer ve oyun berabere biter
                else
                {
                    //O zaman berabere biter
                    Sonuc = Sonuc.Beraberlik(BitisSebebi.Pat);
                }
            }
            //Ve bunu her hamle yaptıktan sonra çağıracağız ki kontrol etsin
            else if (Tahta.YetersizMateryal())
            {
                Sonuc = Sonuc.Beraberlik(BitisSebebi.YetersizTas);
            }
            //Ve bunu her hamle yaptıktan sonra çağıracağız ki kontrol etsin
            else if (ElliHamleKurali())
            {
                Sonuc = Sonuc.Beraberlik(BitisSebebi.ElliHamleKurali);
            }
            else if (UcKatliTekrar())
            {
                Sonuc = Sonuc.Beraberlik(BitisSebebi.UcKatliTekrar);
            }
        }
        #endregion

        #region Elli_Hamle_Kuralı
        private bool ElliHamleKurali()
        {
            int tumHareketler = YakalamaVeyaPiyonHamlesiYok / 2;
            return tumHareketler == 50;
        }

        #endregion

        #region Üç_Katlı_Tekrar_Kuralı

        private void DurumStringiniGuncelle()
        {
            durumStringi = new DurumStringi(MevcutOyuncu, Tahta).ToString();

            if (!durumGecmisi.ContainsKey(durumStringi))
            {
                durumGecmisi[durumStringi] = 1;
            }
            else
            {
                durumGecmisi[durumStringi]++;
            }
        }
        private bool UcKatliTekrar()
        {
            return durumGecmisi[durumStringi] == 3;
        }
        #endregion

        #region Oyun_Bitiş_Kontrolü
        //Eğer yukarıda fonksiyondan bir sonuç almışsa oyun zaten biter ve burada içeri girer ve sonucun 0'a eşit olmadğını doğrulayarak geriye döndürür.
        public bool OyunBittiMi()
        {
            return Sonuc != null;
        }
        #endregion

        #region Hamleleri_Txt'ye_Aktarma
        public void HamleyiKaydet(Hamle hamle)
        {
            string hamleMetni = HamleMetniniOlustur(hamle, this); // this ile OyunDurumu nesnesi geçiriliyor
            DosyayaYaz(hamleMetni);
        }

        private string HamleMetniniOlustur(Hamle hamle, OyunDurumu oyunDurumu)
        {
            string oyuncu = MevcutOyuncu == Oyuncu.Beyaz ? "Siyah-" : "Beyaz-";

            if (hamle.Tur == HamleTuru.KaleSahKanadi)
            {
                return oyuncu + "0-0";
            }
            else if (hamle.Tur == HamleTuru.KaleVezirKanadi)
            {
                return oyuncu + "0-0-0";
            }

            string tas = TasKisaltmasi(hamle.ToPos);
            string hedefKare = KareAdi(hamle.ToPos);
            string yakalama = hamle.TasSilindi ? "x" : "";
            string terfi = "";

            if (hamle.Tur == HamleTuru.PiyonTerfi)
            {
                PiyonTerfi terfiHamle = (PiyonTerfi)hamle;
                tas = "Piyon ";
                terfi = "=" + TasKisaltmasi(terfiHamle.ToPos);
            }

            string sahCekme = !oyunDurumu.OyunBittiMi() && Tahta.TehditAltinda(MevcutOyuncu) ? "+" : "";
            string sahMat = oyunDurumu.OyunBittiMi() && oyunDurumu.Sonuc.Sebep == BitisSebebi.SahMat ? "#" : "";

            return oyuncu + tas + yakalama + hedefKare + terfi + sahCekme + sahMat;
        }
        private string TasKisaltmasi(Pozisyon poz)
        {
            if (Tahta.BosMu(poz))
            {
                return ""; // Boş kare için boş string döndür
            }

            Tas tas = Tahta[poz];
            string c = tas.Tur switch
            {
                TasTuru.Piyon => "Piyon ",
                TasTuru.At => "At ",
                TasTuru.Kale => "Kale ",
                TasTuru.Fil => "Fil ",
                TasTuru.Vezir => "Vezir ",
                TasTuru.Sah => "Şah ",
                _ => " "
            };
            return c;
        }

        private string KareAdi(Pozisyon poz)
        {
            char dosya = (char)('a' + poz.Sutun);
            int siralama = 8 - poz.Satir;
            return dosya.ToString() + siralama;
        }

        private void DosyayaYaz(string metin)
        {
            string dosyaYolu = "hamleler.txt";
            using (StreamWriter writer = new StreamWriter(dosyaYolu, true))
            {
                writer.WriteLine(metin);
            }
        }
        public void HamleDosyasiniSil()
        {
            string dosyaYolu = "hamleler.txt";
            if (File.Exists(dosyaYolu))
            {
                File.Delete(dosyaYolu);
            }
        }
        #endregion

    }
}
