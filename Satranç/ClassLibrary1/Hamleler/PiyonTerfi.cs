using SatrancMantigi.Taslar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SatrancMantigi
{
    public class PiyonTerfi : Hamle
    {
        #region Piyon_Terfi_Özellik_Kalıpları

        public override HamleTuru Tur => HamleTuru.PiyonTerfi;
        public override Pozisyon FromPos { get; }
        public override Pozisyon ToPos { get; }

        private readonly TasTuru yeniTur;//Piyonun terfi ettileceği taş türü

        public PiyonTerfi(Pozisyon fromPos, Pozisyon to, TasTuru yeniTur)

        {
            FromPos = fromPos;
            ToPos = to;
            this.yeniTur = yeniTur;
        }
        #endregion

        #region Terfi_Taşı_Oluşturma
        //Terfi taşını bu kısımda oluşturuyoruz ve oyuncu rengiyle aynı girdiyi alıyoruz
        private Tas TerfiTasiOlusturma(Oyuncu renk)
        {
            //Saklanan yeni türle yeni bir parça döndürür ve gövdede verilen renkle
            return yeniTur switch
            {
                TasTuru.At => new At(renk),
                TasTuru.Fil => new Fil(renk),
                TasTuru.Kale => new Kale(renk),
                _ => new Vezir(renk)
            };
        }
        #endregion

        #region Terfi_Taşı_Ana_Çalıştırma_Aşaması
        public override bool Execute(Tahta tahta)
        {
            //Önce hareketli piyonu kaydediyoruz
            Tas piyon = tahta[FromPos];
            //Ön pozisyonu boşaltıyoruz
            tahta[FromPos] = null;

            //Terfi taşını oluşturuyoruz ve hareketli piyonla aynı renkte olmalı
            Tas terfiTasi = TerfiTasiOlusturma(piyon.Renk);
            terfiTasi.Tasindi = true;

            TasSilindi = !tahta.BosMu(ToPos);

            //Son olarak mevcut pozisyonda seçilen taş görüntülenir
            tahta[ToPos] = terfiTasi;

            return true;
        }
        #endregion
    }
}
