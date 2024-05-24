using SatrancMantigi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SatrancUI
{
    public partial class OyunBitisMenusu : UserControl
    {
        #region Seçili_Seçenek_Tanımlama
        //SeciliSecenek adlı bir olay tanımlar. Bu olay, kullanıcının menüdeki bir seçeneği seçtiğinde tetiklenir ve Secenek enum tipinde bir parametre alır.
        public event Action<Secenek> SeciliSecenek;
        #endregion

        #region Menüde_Oyun_Durumu
        public OyunBitisMenusu(OyunDurumu oyunDurumu)
        {
            InitializeComponent();

            //Oyun sonucunu yakalıyoruz
            Sonuc sonuc = oyunDurumu.Sonuc;
            //Oyun matla bitmişse kazananın metnini çekiyoruz
            WinnerText.Text = KazananınMetniniAl(sonuc.Kazanan);
            //Oyun berabere bitmişse berabere durmunu çekiyoruz ve sebebini bildiriyoruz
            ReasonText.Text = SebepMetniniAl(sonuc.Sebep, oyunDurumu.MevcutOyuncu);
        }
        #endregion

        #region Kazanma_Metnini_Alma
        private static string KazananınMetniniAl(Oyuncu kazanan)
        {

            return kazanan switch
            {
                //Eğer kazanan beyazsa geri dön beyaz kazandı
                Oyuncu.Beyaz => "BEYAZ KAZANDI!",
                
                //Eğer kazanan siyahsa geri dön beyaz kazandı
                Oyuncu.Siyah => "SİYAH KAZANDI!",
                
                //Bu durum hiç gerçekleşmeyecek ama bunu yapmadığımızda switch sikayet eder :)
                _ => "BERABERLİK"
            };
        }
        #endregion

        #region Ad_Döndürme
        //Bir oyuncuyu alır ve adını bir dize olarak döndürür
        private static string OyuncuStringi(Oyuncu oyuncu)
        {
            return oyuncu switch
            {
                Oyuncu.Beyaz => "BEYAZ",
                Oyuncu.Siyah => "SİYAH",
                _ => ""
            };
        }
        #endregion

        #region Oyun_Sonu_Metnini_Alma
        private static string SebepMetniniAl(BitisSebebi sebep, Oyuncu mevcutOyuncu)
        {
            return sebep switch
            {
                BitisSebebi.Pat => $"PAT - {OyuncuStringi(mevcutOyuncu)} HAMLE YAPAMIYOR",
                BitisSebebi.SahMat => $"ŞAH MAT - {OyuncuStringi(mevcutOyuncu)} HAMLE YAPAMIYOR",
                BitisSebebi.ElliHamleKurali => "ELLİ-HAMLE KURALI",
                BitisSebebi.YetersizTas => "YETERSİZ TAŞ",
                BitisSebebi.UcKatliTekrar => "ÜÇ KATLI TEKRAR",
                BitisSebebi.SureDoldu => $"{OyuncuStringi(mevcutOyuncu)}'IN SÜRESİ DOLDU!",
                _ => ""
            };
        }
        #endregion

        #region Yeniden_Başlat_Butonu
        //Yeniden başlata basıldığında içinden geçen olayı bulacağız
        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            //Buradaki soru işareti olayın yalnızca kayıtlı bir olay işleyicisi varsa ortaya çıkmasını sağlar
            SeciliSecenek?.Invoke(Secenek.YenidenBaslat);
        }
        #endregion

        #region Çıkış_Butonu
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SeciliSecenek?.Invoke(Secenek.Cikis);
        }
        #endregion
    }
}
